using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomPatrol : Leaf
{
    public RandomPatrol(AI aiagent) : base(aiagent)
    {
    }

    public override NodeStatus OnAction(BlackBoard state)
    {
        //Debug.Log("random patrol");
        AIagent.actionScript.Patrol();
        //AIagent.statusText.text = "RandomPatrol";
        return NodeStatus.SUCCESS;
    }

    public override void ResetAction()
    {
    }
}

public class CheckCanSeePlayer : Leaf
{
    public CheckCanSeePlayer(AI aiagent) : base(aiagent)
    {
    }

    public override NodeStatus OnAction(BlackBoard state)
    {
       if(AIagent.actionScript.playerInSight && !AIagent.actionScript.player.GetComponent<Player>().isHiding)
       {
            if (AIagent.actionScript.playerInSightForDuration || state.hasRecentlySeenPlayer)
            {
                //Debug.Log("Can see player");
                state.lastSeenPlayerLocation = AIagent.actionScript.player.transform.position;
                AIagent.actionScript.ResetCheatSearchTimer();
                AIagent.actionScript.ResetSearchTimer();
                state.hasRecentlySeenPlayer = true;
                state.hasRecentlyHeardSound = false;
                state.hasSelectedSearchPosition = false;

                state.lastSearchPosition = Vector3.zero;

                state.lastSeenPlayerDirection = AIagent.actionScript.GetDirectionToPosition(AIagent.actionScript.player.transform.position + AIagent.actionScript.player.transform.forward, AIagent.actionScript.player.transform.position);

                GameObject LastSeenNode;
                LastSeenNode = GameObject.Find("LastSeenPos");
                if (LastSeenNode != null)
                {
                    LastSeenNode.transform.position = state.lastSeenPlayerLocation;
                }

                AIagent.actionScript.ResetIdleTimers();
                return NodeStatus.SUCCESS;
            }
            else
            {
                AIagent.actionScript.StopMovement();
                AIagent.actionScript.LookAtPoint(AIagent.actionScript.player.transform.position);
                state.hasSelectedSearchPosition = false;
            }
       }
        return NodeStatus.FAILURE;
    }

    public override void ResetAction()
    {
    }
}

public class CheckCanHearSound : Leaf
{
    public CheckCanHearSound(AI aiagent) : base(aiagent)
    {
    }

    public override NodeStatus OnAction(BlackBoard state)
    {
        if (AIagent.actionScript.soundIsHeard && AIagent.actionScript.lastHeardSoundEmitter != null)
        {
            //Debug.Log("Can hear sound");
            state.lastHeardSoundLocation = AIagent.actionScript.lastHeardSoundEmitter.transform.position;

            if(!state.hasRecentlyHeardSound)
            {
                state.hasSelecetedSoundSearchPosition = false;
            }

            state.hasRecentlyHeardSound = true;

            //Check the direction that the sound is coming from. If it is behind the last seen player location then ignore it.
            //THIS IS VERY EXPENSIVE, NEEDS TO BE OPTIMISED
            //if (state.hasRecentlySeenPlayer && (AIagent.actionScript.lastHeardSoundEmitter.soundType != SoundType.PLAYER))
            //{
            //    //The last seen player position plus their last seen direction, their assumed new position
            //    Vector3 directionToPlayer = AIagent.actionScript.GetDirectionToPosition(state.lastSeenPlayerLocation + (state.lastSeenPlayerDirection * 5.0f), state.lastSeenPlayerLocation);
            //    Vector3 directionToSoundPos = AIagent.actionScript.GetDirectionToPosition(state.lastHeardSoundLocation, state.lastSeenPlayerLocation);
            //    float dot = Vector3.Dot(directionToPlayer, directionToSoundPos);
            //    //Debug.Log("dot " + dot);

            //    Debug.DrawLine(state.lastSeenPlayerLocation, state.lastSeenPlayerLocation + directionToPlayer * 10, Color.red, 5.0f);
            //    Debug.DrawLine(state.lastSeenPlayerLocation, state.lastSeenPlayerLocation + directionToSoundPos * 10, Color.green, 5.0f);

            //    if (dot < 0)
            //    {
            //        Debug.Log("SOUND WRONG DIRECTION");
            //        //Debug.Log("dot " + dot);
            //        state.hasRecentlyHeardSound = false;
            //        return NodeStatus.FAILURE;
            //    }
            //}

            GameObject LastHeardNode;
            LastHeardNode = GameObject.Find("LastHeardPos");
            if (LastHeardNode != null)
            {
                LastHeardNode.transform.position = state.lastHeardSoundLocation;
            }

            return NodeStatus.SUCCESS;
        }
        return NodeStatus.FAILURE;
    }

    public override void ResetAction()
    {
    }
}

public class LookAtLastHeardPosition : Leaf
{
    public LookAtLastHeardPosition(AI aiagent) : base(aiagent)
    {
    }

    public override NodeStatus OnAction(BlackBoard state)
    {
        AIagent.actionScript.LookAtPoint(state.lastHeardSoundLocation);
        //Debug.Log("Look at");
        return NodeStatus.SUCCESS;
    }

    public override void ResetAction()
    {
    }
}

public class ChasePlayer : Leaf
{
    public ChasePlayer(AI aiagent) : base(aiagent)
    {
    }

    public override NodeStatus OnAction(BlackBoard state)
    {
        if (AIagent.actionScript.GetDistanceToPlayer() <= AIagent.actionScript.attackRange)
        {
            if (AIagent.actionScript.AttackPlayer())
            {
                AIagent.actionScript.StopMovement();
            }
            AIagent.actionScript.StopMovement();
        }
        else
        {
            AIagent.actionScript.MoveToPosition(AIagent.actionScript.player.transform.position, AIagent.actionScript.chaseSpeed);
        }
        //Debug.Log("Chase player");
        //AIagent.statusText.text = "Chase";
        return NodeStatus.SUCCESS;
    }

    public override void ResetAction()
    {
    }
}

public class CheckHasRecentlySeenPlayer : Leaf
{
    public CheckHasRecentlySeenPlayer(AI aiagent) : base(aiagent)
    {
    }

    public override NodeStatus OnAction(BlackBoard state)
    {
       if(state.hasRecentlySeenPlayer)
       {
           //Debug.Log("has recently seen player");
           return NodeStatus.SUCCESS;
       }
       return NodeStatus.FAILURE;
    }

    public override void ResetAction()
    {
    }
}

public class CheckHasRecentlyHeardSound : Leaf
{
    public CheckHasRecentlyHeardSound(AI aiagent) : base(aiagent)
    {
    }

    public override NodeStatus OnAction(BlackBoard state)
    {
        if (state.hasRecentlyHeardSound)
        {
            //Debug.Log("has recently heard sound");
            return NodeStatus.SUCCESS;
        }
        return NodeStatus.FAILURE;
    }

    public override void ResetAction()
    {
    }
}

public class CheckIfCloseToLastSeenPosition : Leaf
{
    public CheckIfCloseToLastSeenPosition(AI aiagent) : base(aiagent)
    { 
    }

    public override NodeStatus OnAction(BlackBoard state)
    {
        if (AIagent.actionScript.CheckIfCloseToPosition(state.lastSeenPlayerLocation, AIagent.actionScript.searchRange))
        {
            //Debug.Log("MAIN SEARCH Close to seen position");
            return NodeStatus.SUCCESS;
        }
        return NodeStatus.FAILURE;
    }

    public override void ResetAction()
    {
    }
}

public class CheckIfCloseToLastHeardPosition : Leaf
{
    public CheckIfCloseToLastHeardPosition(AI aiagent) : base(aiagent)
    {
    }

    public override NodeStatus OnAction(BlackBoard state)
    {
        if(AIagent.actionScript.CheckIfCloseToPosition(state.lastHeardSoundLocation, AIagent.actionScript.searchSoundRange))
        {
            //Debug.Log("Close to heard position");
            AIagent.actionScript.LookAtPoint(state.lastHeardSoundLocation);
            return NodeStatus.SUCCESS;
        }
        return NodeStatus.FAILURE;
    }

    public override void ResetAction()
    {
    }
}

public class CheckIfCanSeeLastHeardPosition : Leaf
{
    public CheckIfCanSeeLastHeardPosition(AI aiagent) : base(aiagent)
    {
    }

    public override NodeStatus OnAction(BlackBoard state)
    {
        if (AIagent.actionScript.CheckIfCanSeePoint(state.lastHeardSoundLocation))
        {
            //Debug.Log("Can see heard position");
            return NodeStatus.SUCCESS;
        }
        return NodeStatus.FAILURE;
    }

    public override void ResetAction()
    {
    }
}

public class CheckSearchTimer : Leaf
{
    public CheckSearchTimer(AI aiagent) : base(aiagent)
    {
    }

    public override NodeStatus OnAction(BlackBoard state)
    {
        if (AIagent.actionScript.CheckSearchTimer())
        {
            state.hasRecentlySeenPlayer = false;
            state.lastSearchPosition = Vector3.zero;
            //state.hasRecentlyHeardSound = false;

            //if(AIagent.actionScript.lastHeardSoundEmitter != null && !AIagent.actionScript.lastHeardSoundEmitter.isPlayer)
            //{
            //    AIagent.actionScript.checkedSoundEmitters.Add(AIagent.actionScript.lastHeardSoundEmitter);
            //    AIagent.actionScript.lastHeardSoundEmitter = null;
            //}

            Debug.Log("Search over");
            return NodeStatus.SUCCESS;
        }
        return NodeStatus.FAILURE;
    }

    public override void ResetAction()
    {
    }
}

public class CheckSoundSearchTimer : Leaf
{
    public CheckSoundSearchTimer(AI aiagent) : base(aiagent)
    {
    }

    public override NodeStatus OnAction(BlackBoard state)
    {
        if (AIagent.actionScript.CheckSearchSoundTimer())
        {
            //state.hasRecentlySeenPlayer = false;
            state.hasRecentlyHeardSound = false;
            state.hasSelecetedSoundSearchPosition = false;

            if (AIagent.actionScript.lastHeardSoundEmitter != null && (AIagent.actionScript.lastHeardSoundEmitter.soundType != SoundType.PLAYER && AIagent.actionScript.lastHeardSoundEmitter.soundType != SoundType.PLAYERTHROWNOBJ))
            {
                //Debug.Log("" + AIagent.actionScript.lastHeardSoundEmitter.soundType.ToString());
                AIagent.actionScript.checkedSoundEmitters.Add(AIagent.actionScript.lastHeardSoundEmitter);

                int numberOfSameSoundTypes = 0;

                foreach (soundEmitter sound in AIagent.actionScript.checkedSoundEmitters)
                {
                    if(sound.soundType == AIagent.actionScript.lastHeardSoundEmitter.soundType)
                    {
                        numberOfSameSoundTypes++;
                    }
                }

                if(numberOfSameSoundTypes >= AIagent.actionScript.numberOfSoundChecksBeforeIgnore)
                {
                    AIagent.actionScript.ignoreSoundTypes.Add(AIagent.actionScript.lastHeardSoundEmitter.soundType);
                }

                AIagent.actionScript.lastHeardSoundEmitter = null;
            }

            //Debug.Log("Sound search over");
            AIagent.actionScript.ResetSoundSearchTimer();
            return NodeStatus.SUCCESS;
        }
        return NodeStatus.FAILURE;
    }

    public override void ResetAction()
    {
    }
}

public class MoveToLastSeenPostion : Leaf
{
    public MoveToLastSeenPostion(AI aiagent) : base(aiagent)
    {
    }

    public override NodeStatus OnAction(BlackBoard state)
    {
        AIagent.actionScript.MoveToPosition(state.lastSeenPlayerLocation, AIagent.actionScript.searchSpeed);
        Debug.Log("Move to last seen player position");
        //AIagent.statusText.text = "SearchSeen";
        return NodeStatus.SUCCESS;
    }

    public override void ResetAction()
    {
    }
}

//Select a random point around the last seen position of the player in a radius to move towards.
//Will only select points which are towards the player's last seen direction from their last seen position
public class SelectRandomPointNearLastSeen : Leaf
{
    public SelectRandomPointNearLastSeen(AI aiagent) : base(aiagent)
    {
    }

    public override NodeStatus OnAction(BlackBoard state)
    {
        float currentTimer;
        if (!AIagent.actionScript.CheckCheatSearchTimer(out currentTimer))
        {
            //Debug.Log("Cheat search");
            SearchPlayerCurrentLocation(state, currentTimer);
        }
        else
        {
            //Directional search is expensive and not very effective, makes AI look too erratic
            //Debug.Log("Direction search");
            //SearchTowardsLastSeenPlayerFacing(state);

            //Debug.Log("Normal search");
            SearchPlayerCurrentLocation(state, 15);
        }

        //If failed to select a point towards player's last seen direction, select a location near the player's last seen position
        if (!state.hasSelectedSearchPosition)
        {
            //Debug.Log("back up search");
            //SearchLastSeenPlayerLocation(state, 5.0f);
        }

        //If no position is selected stop movement, shouldn't get here
        if (state.hasSelectedSearchPosition)
        {
            return NodeStatus.SUCCESS;
        }
        else
        {
            Debug.Log("should not do this search");
            AIagent.actionScript.StopMovement();
            return NodeStatus.FAILURE;
        }
    }

    public override void ResetAction()
    {
    }

    public void SearchLastSeenPlayerLocation(BlackBoard state, float searchRadius)
    {
        Vector3 searchPos;
        if (AIagent.actionScript.SelectRandomPointInArea(state.lastSeenPlayerLocation, searchRadius, out searchPos))
        {
            state.hasSelectedSearchPosition = true;

            state.lastSearchPosition = state.currentSearchPosition;

            state.currentSearchPosition = searchPos;
        }
    }

    public void SearchPlayerCurrentLocation(BlackBoard state, float searchRadius)
    {
        Vector3 searchPos;
        if (AIagent.actionScript.SelectRandomPointInArea(AIagent.actionScript.player.transform.position, searchRadius, out searchPos))
        {
            state.hasSelectedSearchPosition = true;

            //Debug.Log("distance " + (Vector3.Distance(searchPos, state.currentSearchPosition)));
            state.lastSearchPosition = state.currentSearchPosition;

            state.currentSearchPosition = searchPos;
        }
    }

    public void SearchTowardsLastSeenPlayerFacing(BlackBoard state)
    {
        Vector3 searchPos;
        if (AIagent.actionScript.SelectRandomPointInArea(state.lastSeenPlayerLocation, AIagent.actionScript.searchRange, out searchPos))
        {
            //Get direction to player and direction to new search position from the last seen player location
            Vector3 directionToPlayer = AIagent.actionScript.GetDirectionToPosition(state.lastSeenPlayerLocation + (state.lastSeenPlayerDirection * 5.0f), state.lastSeenPlayerLocation);
            Vector3 directionToSearchPos = AIagent.actionScript.GetDirectionToPosition(searchPos, state.lastSeenPlayerLocation);

            Debug.DrawLine(state.lastSeenPlayerLocation, state.lastSeenPlayerLocation + (state.lastSeenPlayerDirection * 5.0f), Color.red, 5.0f);
            Debug.DrawLine(state.lastSeenPlayerLocation, state.lastSeenPlayerLocation + directionToSearchPos * 10, Color.green, 5.0f);

            //If the dot product between the two direction vectors is less than 0, then that means that the selected search position is AWAY from the player and a new position should be selected,
            //else if the dot product is more than 0, then that means that the selected search position is TOWARDS the player's last seen direction
            float dot = Vector3.Dot(directionToPlayer, directionToSearchPos);
            //Debug.Log("dot " + dot);

            if (dot > 0.5)
            {
                float maxDistance = AIagent.actionScript.searchRange;
                float minDistance = 5.0f;

                if (state.lastSearchPosition != Vector3.zero && ((Vector3.Distance(searchPos, state.lastSearchPosition) <= maxDistance) && (Vector3.Distance(searchPos, state.lastSearchPosition) >= minDistance)))
                {
                    //Debug.Log("USE LAST SEARCH POS");
                    state.hasSelectedSearchPosition = true;

                    state.lastSearchPosition = state.currentSearchPosition;

                    state.currentSearchPosition = searchPos;
                }
                else if ((Vector3.Distance(searchPos, state.currentSearchPosition) <= maxDistance) && (Vector3.Distance(searchPos, state.currentSearchPosition) >= minDistance))
                {
                    //Debug.Log("good point");
                    state.hasSelectedSearchPosition = true;

                    state.lastSearchPosition = state.currentSearchPosition;

                    state.currentSearchPosition = searchPos;
                }
            }
            else if (dot <= 0.5)
            {
                //Debug.Log("bad point");
            }        
        }
    }
}


public class IncrementCheatSearchTimer : Leaf
{
    public IncrementCheatSearchTimer(AI aiagent) : base(aiagent)
    {
    }

    public override NodeStatus OnAction(BlackBoard state)
    {
        float currentTimer;
        AIagent.actionScript.CheckCheatSearchTimer(out currentTimer);

        return NodeStatus.SUCCESS;
    }

    public override void ResetAction()
    {
    }
}

//Returns success if a search position has been selected
public class CheckHasSelectedSearchPos : Leaf
{
    public CheckHasSelectedSearchPos(AI aiagent) : base(aiagent)
    {
    }

    public override NodeStatus OnAction(BlackBoard state)
    {
        if(state.hasSelectedSearchPosition)
        {
            //Debug.Log("find new search pos");
            return NodeStatus.SUCCESS;
        }

        return NodeStatus.FAILURE;

    }

    public override void ResetAction()
    {
    }
}

//Moves agent to currently selected search position
public class MoveToCurrentSearchPostion : Leaf
{
    public MoveToCurrentSearchPostion(AI aiagent) : base(aiagent)
    {
    }

    public override NodeStatus OnAction(BlackBoard state)
    {
        AIagent.actionScript.MoveToPosition(state.currentSearchPosition, AIagent.actionScript.searchSpeed);
        //AIagent.actionScript.hideInObjectToCheck = AIagent.actionScript.RandomlySelectHideInObject();
        //Debug.Log("Move to search position");
        return NodeStatus.SUCCESS;
    }

    public override void ResetAction()
    {
    }
}

//If close to the current search position, select a new one.
public class CheckIfCloseToSearchPosition : Leaf
{
    public CheckIfCloseToSearchPosition(AI aiagent) : base(aiagent)
    {
    }

    public override NodeStatus OnAction(BlackBoard state)
    {
        if (AIagent.actionScript.CheckIfCloseToPosition(state.currentSearchPosition, AIagent.actionScript.closeToPositionRange))
        {
            //Debug.Log("SELECT NEW SEARCH POSITION");
            state.hasSelectedSearchPosition = false;
            return NodeStatus.SUCCESS;
        }
        return NodeStatus.FAILURE;
    }

    public override void ResetAction()
    {
    }
}

//Move towards the last heard sound position
public class MoveToLastHeardPostion : Leaf
{
    public MoveToLastHeardPostion(AI aiagent) : base(aiagent)
    {
    }

    public override NodeStatus OnAction(BlackBoard state)
    {
        //if (!state.hasSelecetedSoundSearchPosition)
        //{
        //    //Vector3 tempSearchPos;
        //    //if (AIagent.actionScript.SelectRandomPointInArea(state.lastHeardSoundLocation, AIagent.actionScript.searchSoundRange, out tempSearchPos))
        //    //{
        //    //    state.currentSoundSearchPosition = tempSearchPos;
        //    //    state.hasSelecetedSoundSearchPosition = true;
        //    //}

        //    UnityEngine.AI.NavMeshHit hit;

        //    if (UnityEngine.AI.NavMesh.SamplePosition(state.lastHeardSoundLocation, out hit, 2.0f, 1))
        //    {
        //        state.hasSelecetedSoundSearchPosition = true;
        //        state.currentSoundSearchPosition = hit.position;
        //    }

        //    Debug.Log("redo sound pos");
        //}

        AIagent.actionScript.MoveToPosition(state.lastHeardSoundLocation, AIagent.actionScript.searchSpeed);
        //Debug.Log("Move to last heard sound position");
        //AIagent.statusText.text = "SearchHeard";
        return NodeStatus.FAILURE;
    }

    public override void ResetAction()
    {
    }
}

public class CheckIfCheckingHideInObject : Leaf
{
    public CheckIfCheckingHideInObject(AI aiagent) : base(aiagent)
    {

    }

    public override NodeStatus OnAction(BlackBoard state)
    {
        if (AIagent.actionScript.hideInObjectToCheck != null)
        {
            Debug.Log("IS CHECKING");
            return NodeStatus.SUCCESS;
        }

        return NodeStatus.FAILURE;
    }

    public override void ResetAction()
    {
    }
}

public class MoveToHideInObject : Leaf
{
    public MoveToHideInObject(AI aiagent) : base(aiagent)
    {
    }

    public override NodeStatus OnAction(BlackBoard state)
    {      
        //Debug.Log("MOVE TO HIDE IN");
        AIagent.actionScript.MoveToPosition(AIagent.actionScript.hideInObjectToCheck.transform.position + (AIagent.actionScript.hideInObjectToCheck.transform.forward * 2), AIagent.actionScript.searchSpeed);
        return NodeStatus.SUCCESS;
    }

    public override void ResetAction()
    {
    }
}

public class CheckIfCloseToHideInObject : Leaf
{
    public CheckIfCloseToHideInObject(AI aiagent) : base(aiagent)
    {
    }

    public override NodeStatus OnAction(BlackBoard state)
    {
        if (Vector3.Distance(AIagent.actionScript.hideInObjectToCheck.transform.position + (AIagent.actionScript.hideInObjectToCheck.transform.forward * 2), AIagent.transform.position) <= 3)
        {
            //Debug.Log("CLOSE TO HIDE IN");
            return NodeStatus.SUCCESS;
        }

        return NodeStatus.FAILURE;

    }

    public override void ResetAction()
    {
    }
}

public class OpenHideInObject : Leaf
{
    public OpenHideInObject(AI aiagent) : base(aiagent)
    {

    }

    public override NodeStatus OnAction(BlackBoard state)
    {
        AIagent.actionScript.OpenHideInObject();
        //Debug.Log("OPEN DOOR");
        return NodeStatus.SUCCESS;
    }

    public override void ResetAction()
    {
    }
}

public class SelectRandomHideIn : Leaf
{
    public SelectRandomHideIn(AI aiagent) : base(aiagent)
    {
    }

    public override NodeStatus OnAction(BlackBoard state)
    {
        if (AIagent.actionScript.CheckOpenHideInTimer())
        {
            if (AIagent.actionScript.hideInObjectToCheck == null)
            {
                //Debug.Log("TRY SELECT NEW HIDE IN OBJECT");
                AIagent.actionScript.hideInObjectToCheck = AIagent.actionScript.RandomlySelectHideInObject();

                if (AIagent.actionScript.hideInObjectToCheck != null)
                {
                    Debug.Log("Has selected new hide in to open");
                    AIagent.actionScript.ResetOpenHideInTimer();
                    return NodeStatus.SUCCESS;
                }
            }
        }
        return NodeStatus.FAILURE;
    }

    public override void ResetAction()
    {
    }
}

public class idleCurrentPosition : Leaf
{
    public idleCurrentPosition(AI aiagent) : base(aiagent)
    {
    }

    public override NodeStatus OnAction(BlackBoard state)
    {
        if (AIagent.actionScript.isIdle && AIagent.actionScript.shouldRandomlyIdle)
        {
            //Debug.Log("idle");
            if (AIagent.actionScript.IdleCurrentPosition())
            {
                return NodeStatus.SUCCESS;
            }

            return NodeStatus.FAILURE;
   
        }
        else
        {
            return NodeStatus.FAILURE;
        }
            
    }

    public override void ResetAction()
    {
    }
}

public class CheckShouldIlde : Leaf
{
    public CheckShouldIlde(AI aiagent) : base(aiagent)
    {
    }

    public override NodeStatus OnAction(BlackBoard state)
    {
        AIagent.actionScript.CheckShouldIdle();
        return NodeStatus.SUCCESS;
    }

    public override void ResetAction()
    {
    }
}

public class CheckIsStunned : Leaf
{
    public CheckIsStunned(AI aiagent) : base(aiagent)
    {
    }

    public override NodeStatus OnAction(BlackBoard state)
    {
        if (AIagent.actionScript.isStunned)
        {
            Debug.Log("IM STUNNED");
            AIagent.actionScript.StopMovement();
            return NodeStatus.SUCCESS;
        }
        return NodeStatus.FAILURE;
    }

    public override void ResetAction()
    {
    }
}

public class CheckIsDead : Leaf
{
    public CheckIsDead(AI aiagent) : base(aiagent)
    {
    }

    public override NodeStatus OnAction(BlackBoard state)
    {
        if (!AIagent.actionScript.isAlive)
        {
            state.hasRecentlyHeardSound = false;
            state.hasRecentlySeenPlayer = false;
            AIagent.actionScript.Deactivate();
            if(AIagent.actionScript.CheckRespawnTimer())
            {
                //Debug.Log("respawn");
                Vector3 respawnPos;
                if(AIagent.actionScript.director.GetSpawnPosition(out respawnPos,1.5f))
                {
                    AIagent.actionScript.Respawn(respawnPos);
                }
                
            }
            return NodeStatus.SUCCESS;
        }
        return NodeStatus.FAILURE;
    }

    public override void ResetAction()
    {
    }
}

public class CheckCanOnlyMoveIfPlayerCantSee : Leaf
{
    public CheckCanOnlyMoveIfPlayerCantSee(AI aiagent) : base(aiagent)
    {
    }

    public override NodeStatus OnAction(BlackBoard state)
    {
        if (AIagent.actionScript.canOnlyMoveIfPlayerCantSee && AIagent.actionScript.CheckCanMove())
        {
            return NodeStatus.SUCCESS;
        }
        else if(!AIagent.actionScript.canOnlyMoveIfPlayerCantSee)
        {
            return NodeStatus.SUCCESS;
        }
        AIagent.actionScript.StopMovement();

        if (AIagent.actionScript.playerInSight && !AIagent.actionScript.player.GetComponent<Player>().isHiding)
        {
            if (AIagent.actionScript.playerInSightForDuration || state.hasRecentlySeenPlayer)
            {
                //Debug.Log("Can see player");
                state.lastSeenPlayerLocation = AIagent.actionScript.player.transform.position;
                AIagent.actionScript.ResetCheatSearchTimer();
                AIagent.actionScript.ResetSearchTimer();
                state.hasRecentlySeenPlayer = true;
                state.hasRecentlyHeardSound = false;
                state.hasSelectedSearchPosition = false;

                state.lastSearchPosition = Vector3.zero;

                state.lastSeenPlayerDirection = AIagent.actionScript.GetDirectionToPosition(AIagent.actionScript.player.transform.position + AIagent.actionScript.player.transform.forward, AIagent.actionScript.player.transform.position);

                GameObject LastSeenNode;
                LastSeenNode = GameObject.Find("LastSeenPos");
                if (LastSeenNode != null)
                {
                    LastSeenNode.transform.position = state.lastSeenPlayerLocation;
                }

                AIagent.actionScript.ResetIdleTimers();
            }
        }

        return NodeStatus.FAILURE;
    }

    public override void ResetAction()
    {
    }
}

