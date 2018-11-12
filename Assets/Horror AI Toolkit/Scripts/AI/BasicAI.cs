using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAI : MonoBehaviour
{

    public enum BasicAIStates
    {
        DEAD,
        STUNNED,
        IDLE,
        PATROL,
        SEARCHLASTSEEN,
        SEARCHLASTHEARD,
        LOOKATPLAYER,
        CHASE
    }

    public BasicAIStates currentState;

    //Contains AI actions
    public AIactions actionScript;

    [HideInInspector]
    public Vector3 lastHeardSoundLocation;
    [HideInInspector]
    public bool hasRecentlyHeardSound = false;

    [HideInInspector]
    public Vector3 lastSeenPlayerLocation;

    [HideInInspector]
    public bool hasRecentlySeenPlayer = false;

    [HideInInspector]
    Vector3 currentSearchPosition;
    [HideInInspector]
    bool hasSelectedSearchPosition = false;

    [HideInInspector]
    public bool hasSelecetedSoundSearchPosition = false;
    [HideInInspector]
    public Vector3 currentSoundSearchPosition;

    // Use this for initialization
    void Start ()
    {
        actionScript = this.gameObject.GetComponent<AIactions>();

        currentState = BasicAIStates.PATROL;

        // Start Finite State Machine
        StartCoroutine("BasicAIFSM");
    }

    void Update()
    {
        CheckCanHearSound();

        if(!actionScript.isAlive)
        {
            currentState = BasicAIStates.DEAD;
        }
        else if(actionScript.isStunned)
        {
            currentState = BasicAIStates.STUNNED;
        }
        else if ((actionScript.playerInSightForDuration || (CheckCanSeePlayer() && hasRecentlySeenPlayer)) && !actionScript.player.GetComponent<Player>().isHiding)
        {
            currentState = BasicAIStates.CHASE;
        }
        else if(CheckCanSeePlayer())
        {
            currentState = BasicAIStates.LOOKATPLAYER;
        }
        else if(hasRecentlyHeardSound)
        {
            currentState = BasicAIStates.SEARCHLASTHEARD;
        }
        else if(hasRecentlySeenPlayer)
        {
            currentState = BasicAIStates.SEARCHLASTSEEN;
        }
        else if (actionScript.isIdle && actionScript.shouldRandomlyIdle)
        {
            currentState = BasicAIStates.IDLE;
        }
        else
        {
            currentState = BasicAIStates.PATROL;
        }
    }

    public void RestartFSM()
    {
        currentState = BasicAIStates.PATROL;
        hasRecentlySeenPlayer = false;
        hasRecentlyHeardSound = false;
        StartCoroutine("BasicAIFSM");
    }

    //Controls the various states of the enemy character
    IEnumerator BasicAIFSM()
    {
        while (true && (actionScript != null))
        {
            switch (currentState)
            {
                case BasicAIStates.DEAD:
                    Dead();
                    break;
                case BasicAIStates.STUNNED:
                    Stunned();
                    break;
                case BasicAIStates.IDLE:
                    Idle();
                    break;
                case BasicAIStates.PATROL:
                    RandomPatrol();
                    break;
                case BasicAIStates.LOOKATPLAYER:
                    LookAtPlayer();
                    break;
                case BasicAIStates.CHASE:
                    ChasePlayer();
                    break;
                case BasicAIStates.SEARCHLASTSEEN:
                    SearchLastSeen();
                    break;
                case BasicAIStates.SEARCHLASTHEARD:
                    SearchLastHeard();
                    break;
            }
            yield return null;
        }
    }

    void RandomPatrol()
    {
        actionScript.CheckShouldIdle();
        if (actionScript.isGrouped)
        {
            actionScript.GroupPatrol();
        }
        else
        {
            actionScript.Patrol();
        }
    }

    void Idle()
    {
        if (!actionScript.IdleCurrentPosition())
        {
            actionScript.ForceFindNewDestination();
            actionScript.MoveToPosition(actionScript.player.transform.position, actionScript.chaseSpeed);
        }
    }

    void Dead()
    {
        hasRecentlySeenPlayer = false;
        hasRecentlyHeardSound = false;
        actionScript.Deactivate();
        if (actionScript.CheckRespawnTimer())
        {
            //Debug.Log("respawn");
            Vector3 respawnPos;
            if (actionScript.director.GetSpawnPosition(out respawnPos, 1.5f))
            {
                actionScript.Respawn(respawnPos);
            }

        }
    }

    void Stunned()
    {
        actionScript.StopMovement();
    }

    void LookAtPlayer()
    {
        //actionScript.MoveToPosition(actionScript.player.transform.position, actionScript.chaseSpeed);
        actionScript.StopMovement();
        actionScript.LookAtPoint(actionScript.player.transform.position);
        hasSelectedSearchPosition = false;
    }

    void ChasePlayer()
    {
        if(actionScript.GetDistanceToPlayer() <= actionScript.attackRange && actionScript.AttackPlayer())
        {
            //if(actionScript.AttackPlayer())
            //{
            //    actionScript.StopMovement();
            //}
            actionScript.StopMovement();
        }
        else
        {
            Vector3 moveToPosition;

            if (actionScript.isFlanker && !actionScript.CheckIfCloseToPositionCheap(actionScript.player.transform.position, actionScript.flankDistance))
            {
                moveToPosition = actionScript.randomPointInCircle(actionScript.player.transform, actionScript.flankDistance, actionScript.flankDirection);

                actionScript.MoveToPosition(moveToPosition, actionScript.flankSpeed);
            }
            else
            {
                moveToPosition = actionScript.player.transform.position;
                actionScript.MoveToPosition(moveToPosition, actionScript.chaseSpeed);
            }

            hasSelectedSearchPosition = false;

            //Debug.Log("Can see player");
            lastSeenPlayerLocation = actionScript.player.transform.position;
            actionScript.ResetCheatSearchTimer();
            actionScript.ResetSearchTimer();
            hasRecentlySeenPlayer = true;
            hasRecentlyHeardSound = false;

            GameObject LastSeenNode;
            LastSeenNode = GameObject.Find("LastSeenPos");
            if (LastSeenNode != null)
            {
                LastSeenNode.transform.position = lastSeenPlayerLocation;
            }
        }        
    }

    void SearchLastSeen()
    {
        actionScript.MoveToPosition(lastSeenPlayerLocation, actionScript.searchSpeed);

        UnityEngine.AI.NavMeshPath testPath = new UnityEngine.AI.NavMeshPath();
        actionScript.agent.CalculatePath(lastSeenPlayerLocation, testPath);
        if(testPath.status != UnityEngine.AI.NavMeshPathStatus.PathComplete)
        {
            hasRecentlySeenPlayer = false;
        }

        if (actionScript.CheckIfCloseToPosition(lastSeenPlayerLocation, actionScript.searchRange))
        {
            //Debug.Log("Close to seen position");

            hasRecentlySeenPlayer = false;
            actionScript.isIdle = true;

            //if (actionScript.CheckSearchTimer())
            //{
            //    hasRecentlySeenPlayer = false;
            //    actionScript.isIdle = true;
            //    Debug.Log("Search over");
            //}
        }
    }

    void SearchLastHeard()
    {
        if (actionScript.CheckIfCloseToPositionCheap(lastHeardSoundLocation, actionScript.searchSoundRange))
        {
            actionScript.LookAtPoint(lastHeardSoundLocation);

            if (actionScript.CheckSearchSoundTimer())
            {
                hasRecentlyHeardSound = false;
                hasSelecetedSoundSearchPosition = false;
                if (actionScript.lastHeardSoundEmitter != null && (actionScript.lastHeardSoundEmitter.soundType != SoundType.PLAYER && actionScript.lastHeardSoundEmitter.soundType != SoundType.PLAYERTHROWNOBJ))
                {
                    //Debug.Log("" + AIagent.actionScript.lastHeardSoundEmitter.soundType.ToString());
                    actionScript.checkedSoundEmitters.Add(actionScript.lastHeardSoundEmitter);

                    int numberOfSameSoundTypes = 0;

                    foreach (soundEmitter sound in actionScript.checkedSoundEmitters)
                    {
                        if (sound.soundType == actionScript.lastHeardSoundEmitter.soundType)
                        {
                            numberOfSameSoundTypes++;
                        }
                    }

                    if (numberOfSameSoundTypes >= actionScript.numberOfSoundChecksBeforeIgnore)
                    {
                        actionScript.ignoreSoundTypes.Add(actionScript.lastHeardSoundEmitter.soundType);
                    }
                    actionScript.lastHeardSoundEmitter = null;
                }
            }
        }
        else
        {
            if (!hasSelecetedSoundSearchPosition)
            {
                UnityEngine.AI.NavMeshHit hit;

                if (UnityEngine.AI.NavMesh.SamplePosition(lastHeardSoundLocation, out hit, 2.0f, 1))
                {
                    hasSelecetedSoundSearchPosition = true;
                    currentSoundSearchPosition = hit.position;
                }

                //Debug.Log("redo sound pos");
            }

            UnityEngine.AI.NavMeshPath testPath = new UnityEngine.AI.NavMeshPath();
            actionScript.agent.CalculatePath(lastHeardSoundLocation, testPath);
            if (testPath.status != UnityEngine.AI.NavMeshPathStatus.PathComplete)
            {
                hasRecentlyHeardSound = false;
            }

            actionScript.MoveToPosition(currentSoundSearchPosition, actionScript.searchSpeed);
        }
    }

    void RandomSearch()
    {
        if (!hasSelectedSearchPosition)
        {
            currentSearchPosition = actionScript.randomPointInCircle(this.transform, actionScript.searchRange, Random.Range(-45, 45));
            Vector3 directionToLookAt = actionScript.GetDirectionToPosition(currentSearchPosition, transform.position);
            RaycastHit hit;
            float minDistanceToObjects = 8.0f;
            if (Physics.Raycast(transform.position, directionToLookAt, out hit, minDistanceToObjects))
            {
                //Debug.Log("Looking at wall or something");
                hasSelectedSearchPosition = false;
            }
            else
            {
                hasSelectedSearchPosition = true;
            }
        }

        actionScript.MoveToPosition(currentSearchPosition, actionScript.searchSpeed);

        if (actionScript.CheckIfCloseToPosition(currentSearchPosition, 0.5f))
        {
            hasSelectedSearchPosition = false;
        }

        if (actionScript.CheckSearchTimer())
        {
            actionScript.isIdle = true;
            Debug.Log("Search over");
        }
    }

    bool CheckCanSeePlayer()
    {
        if (actionScript.playerInSight && !actionScript.player.GetComponent<Player>().isHiding)
        {
            actionScript.ResetIdleTimers();
            return true;
        }
        else
        {
            return false;
        }
    }

    bool CheckCanHearSound()
    {
        if (!hasRecentlySeenPlayer)
        {
            if (actionScript.soundIsHeard && actionScript.lastHeardSoundEmitter != null)
            {
                //Debug.Log("Can hear sound");
                lastHeardSoundLocation = actionScript.lastHeardSoundEmitter.transform.position;

                if (!hasRecentlyHeardSound)
                {
                    hasSelecetedSoundSearchPosition = false;
                }

                hasRecentlyHeardSound = true;

                GameObject LastHeardNode;
                LastHeardNode = GameObject.Find("LastHeardPos");
                if(LastHeardNode != null)
                { 
                LastHeardNode.transform.position = lastHeardSoundLocation;
                }
                return true;
            }
        }
        return false;
    }

}
