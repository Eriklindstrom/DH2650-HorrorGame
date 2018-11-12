using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AIGroup : MonoBehaviour
{
    public int maxSize = 5;
    public bool isBasicGroup = true;
    public List<GameObject> groupMembers = new List<GameObject>();
    public Vector3 currentDestination;
    public bool hasGotDestination = false;

    public WaypointNode currentNode;
    WaypointNode lastNode;
    public bool hasGotNode;

    public Vector3 averageGroupPosition;

    public Vector3 groupLastSeenPlayerPosition;
    public Vector3 groupLastHeardPosition;

    public int groupAvoidancePriority;

    public int maxNumberOfFlankers = 2;
    int currentNumberOfFlankers = 0;
	
    public bool CheckIfFull()
    {
        if(groupMembers.Count >= maxSize)
        {
            //Debug.Log("is full " + groupMembers.Count);
            return true;
        }
        return false;
    }

    public bool AddToGroup(GameObject obj)
    {
        if(!groupMembers.Contains(obj))
        {
            obj.GetComponent<AIactions>().agent.avoidancePriority = groupAvoidancePriority + Random.Range(-5,5);
            groupMembers.Add(obj);
            return true;
        }
        return false;
    }

    public void GetNewGroupDestination()
    {
        if (!hasGotNode && groupMembers.Count > 0)
        {
            currentNode = AIdirector.sharedAIdirector.GetActiveWaypointNode();
            if (currentNode != null && currentNode != lastNode)
            {
                bool eachMemberCanPath = true;
                foreach (GameObject ai in groupMembers)
                {
                    if(!ai.GetComponent<AIactions>().CheckCanPathToPosition(currentNode.transform.position))
                    {
                        eachMemberCanPath = false;
                    }
                }

                if (eachMemberCanPath)
                {
                    hasGotNode = true;
                    hasGotDestination = true;
                    currentNode.setVisitingGroup(this);
                    currentDestination = currentNode.transform.position;
                    return;
                    //Debug.Log("new dest got " + currentDestination);
                }
            }
        }

        if (!hasGotNode && groupMembers.Count > 0)
        {
            Vector3 tempPos;
            bool foundRandomPos = AIdirector.sharedAIdirector.GetPointInAPA(out tempPos, 1.0f);
            if (foundRandomPos)
            {
                bool eachMemberCanPath = true;
                foreach (GameObject ai in groupMembers)
                {

                    if(!ai.GetComponent<AIactions>().CheckCanPathToPosition(tempPos))
                    {
                        eachMemberCanPath = false;
                    }
                }

                if (eachMemberCanPath)
                {
                    hasGotDestination = true;
                    currentDestination = tempPos;
                    //Debug.Log("new dest got " + currentDestination);
                }
            }
        }
    }

    public void ResetGroupDestination()
    {
        hasGotDestination = false;
        hasGotNode = false;
        if(currentNode != null && currentNode != lastNode)
        {
            currentNode.RemoveVisitor();
            lastNode = currentNode;
        }
    }

    public Vector3 GetAveragePositionOfGroupMembers()
    {
        Vector3 meanPos = Vector3.zero;
        foreach(GameObject ai in groupMembers)
        {
            meanPos += ai.transform.position;
        }
        averageGroupPosition = (meanPos / groupMembers.Count);
        return averageGroupPosition;
    }

    public void SetMembersGroupDestination()
    {
        foreach(GameObject ai in groupMembers)
        {
            ai.GetComponent<AIactions>().groupTargetPosition = currentDestination;
        }
    }

    public List<GameObject> GetAllMembers()
    {
        return groupMembers;
    }

    public bool CanAGroupMemberSeePlayer()
    {
        foreach(GameObject obj in groupMembers)
        {
            if(obj.GetComponent<AIactions>().playerInSightForDuration)
            {
                groupLastSeenPlayerPosition = obj.GetComponent<BasicAI>().lastSeenPlayerLocation;
                return true;
            }
        }
        return false;
    }

    public void SetAllGroupMembersVision()
    {
        foreach(GameObject obj in groupMembers)
        {
            if(!obj.GetComponent<AIactions>().playerInSightForDuration)
            {
                //obj.GetComponent<BasicAI>().lastSeenPlayerLocation = groupLastSeenPlayerPosition;
                //obj.GetComponent<BasicAI>().hasRecentlySeenPlayer = true;
                obj.GetComponent<AIactions>().playerInSight = true;
                //obj.GetComponent<AIactions>().playerInSightForDuration = true;
                obj.GetComponent<AIactions>().playerFirstSighting = false;
            }
        }
    }

    public bool CanAGroupMemberHearSound()
    {
        foreach (GameObject obj in groupMembers)
        {
            if (obj.GetComponent<AIactions>().soundIsHeard)
            {
                groupLastHeardPosition = obj.GetComponent<BasicAI>().lastHeardSoundLocation;
                return true;
            }
        }
        return false;
    }

    public void SetAllGroupMembersSoundHeard()
    {
        foreach (GameObject obj in groupMembers)
        {
            if (!obj.GetComponent<AIactions>().soundIsHeard)
            {
                obj.GetComponent<BasicAI>().lastHeardSoundLocation = groupLastHeardPosition;
                obj.GetComponent<BasicAI>().hasRecentlyHeardSound = true;
            }
        }
    }

    public void ResetGroupFlankers()
    {
        foreach(GameObject obj in groupMembers)
        {
            obj.GetComponent<AIactions>().isFlanker = false;
        }
        currentNumberOfFlankers = 0;
    }

    public void SetFlankers(Vector3 playerPosition)
    {
        groupMembers = groupMembers.OrderBy(x => Vector3.Distance(playerPosition, x.transform.position)).ToList();

        for(int i = groupMembers.Count() - 1; i > -1; i--)
        {
            if (currentNumberOfFlankers < maxNumberOfFlankers)
            {
                groupMembers[i].GetComponent<AIactions>().isFlanker = true;
                //Debug.Log("set flanker");
                currentNumberOfFlankers++;

                //Check if the current number of flankers is odd or even.
                //Every even flanker will be set to flank to the right, every odd set to the left
                if (currentNumberOfFlankers % 2 == 0)
                {
                    groupMembers[i].GetComponent<AIactions>().flankDirection = 90;
                }
                else
                {
                    groupMembers[i].GetComponent<AIactions>().flankDirection = -90;
                }
            }
            else
            {
                groupMembers[i].GetComponent<AIactions>().isFlanker = false;
            }
        }
    }

    public bool hasSetAllFlankers()
    {
        return currentNumberOfFlankers == maxNumberOfFlankers;       
    }

    public bool hasSetSomeFlankers()
    {
        return currentNumberOfFlankers > 0;
    }

    public bool CheckIfDestinationIsOutsideAPA()
    {
        foreach(GameObject member in groupMembers)
        {
            if(member.GetComponent<AIactions>().GetIsDesinationOutsideAPA())
            {
                return true;
            }
        }
        return false;
    }

}
