using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoutAI : MonoBehaviour {

    public enum ScoutAIStates
    {
        DEAD,
        STUNNED,
        FLEE,
        IDLE,
        PATROL,
        SEARCHLASTSEEN,
        SEARCHLASTHEARD,
        LOOKATPLAYER,
        SIGNAL
    }

    public ScoutAIStates currentState;

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

    Vector3 currentSearchPosition;
    bool hasSelectedSearchPosition = false;


    [Range(0, 50)]
    [HideInInspector]
    public int segments = 50;
    LineRenderer line;

    // Use this for initialization
    void Start()
    {
        actionScript = this.gameObject.GetComponent<AIactions>();

        currentState = ScoutAIStates.PATROL;

        // Start Finite State Machine
        StartCoroutine("BasicAIFSM");

        line = gameObject.GetComponent<LineRenderer>();

        line.positionCount = segments + 1;
        line.useWorldSpace = false;
        line.material.color = Color.red;
        DrawAreaCircle();
        line.enabled = false;
    }

    void Update()
    {
        CheckCanHearSound();

        if (!actionScript.isAlive)
        {
            currentState = ScoutAIStates.DEAD;
        }
        else if (actionScript.isStunned)
        {
            currentState = ScoutAIStates.STUNNED;
        }
        else if (actionScript.isSignallingAllies)
        {
            currentState = ScoutAIStates.SIGNAL;
        }
        else if(actionScript.isFleeing)
        {
            currentState = ScoutAIStates.FLEE;
        }
        else if (CheckCanSeePlayer())
        {
            currentState = ScoutAIStates.LOOKATPLAYER;
        }
        else if (hasRecentlyHeardSound)
        {
            currentState = ScoutAIStates.SEARCHLASTHEARD;
        }
        else if (hasRecentlySeenPlayer)
        {
            currentState = ScoutAIStates.SEARCHLASTSEEN;
        }
        else if (actionScript.isIdle)
        {
            currentState = ScoutAIStates.IDLE;
        }
        else
        {
            currentState = ScoutAIStates.PATROL;
        }
    }

    //Controls the various states of the enemy character
    IEnumerator BasicAIFSM()
    {
        while (true)
        {
            switch (currentState)
            {
                case ScoutAIStates.DEAD:
                    Dead();
                    break;
                case ScoutAIStates.STUNNED:
                    Stunned();
                    break;
                case ScoutAIStates.FLEE:
                    Flee();
                    break;
                case ScoutAIStates.IDLE:
                    Idle();
                    break;
                case ScoutAIStates.PATROL:
                    RandomPatrol();
                    break;
                case ScoutAIStates.LOOKATPLAYER:
                    LookAtPlayer();
                    break;
                case ScoutAIStates.SIGNAL:
                    SignalAllies();
                    break;
                case ScoutAIStates.SEARCHLASTSEEN:
                    SearchLastSeen();
                    break;
                case ScoutAIStates.SEARCHLASTHEARD:
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

    void Idle()
    {
        if (!actionScript.IdleCurrentPosition())
        {
            actionScript.ForceFindNewDestination();
            actionScript.MoveToPosition(actionScript.player.transform.position, actionScript.chaseSpeed);
        }
    }

    void SignalAllies()
    {
        if(actionScript.isSignallingAllies)
        {
            line.enabled = true;
            actionScript.SignalNearbyAllies(30);
        }
        else
        {
            actionScript.isFleeing = true;
            line.enabled = false;
        }
    }

    void LookAtPlayer()
    {
        //actionScript.MoveToPosition(actionScript.player.transform.position, actionScript.chaseSpeed);
        actionScript.StopMovement();
        actionScript.LookAtPoint(actionScript.player.transform.position);
        hasSelectedSearchPosition = false;

        if(actionScript.playerInSightForDuration)
        {
            actionScript.isSignallingAllies = true;
        }
    }

    void SearchLastSeen()
    {
        actionScript.MoveToPosition(lastSeenPlayerLocation, actionScript.searchSpeed);

        if (actionScript.CheckIfCloseToPosition(lastSeenPlayerLocation, actionScript.searchRange))
        {
            Debug.Log("Close to seen position");

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
        actionScript.MoveToPosition(lastHeardSoundLocation, actionScript.searchSpeed);

        if (actionScript.CheckIfCloseToPosition(lastHeardSoundLocation, actionScript.searchSoundRange))
        {
            Debug.Log("Close to seen position");

            if (actionScript.CheckSearchSoundTimer())
            {
                hasRecentlyHeardSound = false;
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

    void Flee()
    {
        actionScript.RunAwayFromPosition(actionScript.player.transform.position);
        hasRecentlySeenPlayer = false;
    }

    bool CheckCanSeePlayer()
    {
        if (actionScript.playerInSight && !actionScript.player.GetComponent<Player>().isHiding)
        {
            //Debug.Log("Can see player");
            lastSeenPlayerLocation = actionScript.player.transform.position;
            actionScript.ResetCheatSearchTimer();
            actionScript.ResetSearchTimer();
            hasRecentlySeenPlayer = true;
            hasRecentlyHeardSound = false;

            GameObject LastSeenNode;
            LastSeenNode = GameObject.Find("LastSeenPos");
            LastSeenNode.transform.position = lastSeenPlayerLocation;

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
                hasRecentlyHeardSound = true;

                GameObject LastHeardNode;
                LastHeardNode = GameObject.Find("LastHeardPos");
                LastHeardNode.transform.position = lastHeardSoundLocation;
                return true;

            }
        }
        return false;
    }

    void DrawAreaCircle()
    {
        float x;
        float y;
        float z;

        float angle = 20f;

        for (int i = 0; i < (segments + 1); i++)
        {
            x = (Mathf.Sin(Mathf.Deg2Rad * angle) * actionScript.signalAlliesRange);// + transform.position.x;
            z = (Mathf.Cos(Mathf.Deg2Rad * angle) * actionScript.signalAlliesRange);// + transform.position.z;

            line.SetPosition(i, new Vector3(x, 1, z));

            angle += (360f / segments);
        }
    }
}
