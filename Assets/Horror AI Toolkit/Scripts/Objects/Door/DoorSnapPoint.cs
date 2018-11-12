using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSnapPoint : MonoBehaviour
{
    public bool isHit;
    public bool isHitPlayer;

    Door parentDoor;

    // Use this for initialization
    void Start ()
    {
        parentDoor = gameObject.transform.parent.gameObject.GetComponent<Door>();
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    void OnTriggerStay(Collider col)
    {
        if(col.tag == "Player" &&  !col.isTrigger)
        {
            isHitPlayer = true;
        }
        if ((col.tag == "Player" || col.tag == "AdvancedAI" || col.tag == "BasicAI" || col.tag == "ScoutAI") && !col.isTrigger)
        {
            isHit = true;
        }

        //if((col.tag == "AdvancedAI" || col.tag == "BasicAI" || col.tag == "ScoutAI") && !col.isTrigger)
        //{
        //    if (!parentDoor.allNearbyAI.Contains(col.gameObject))
        //    {
        //        parentDoor.allNearbyAI.Add(col.gameObject);
        //    }

        //    if(parentDoor.visitor == null)
        //    {
        //        parentDoor.visitor = col.gameObject;
        //        //parentDoor.visitor.gameObject.GetComponent<AIactions>().agent.avoidancePriority = 0;
        //    }
        //}
    }

    void OnTriggerExit(Collider col)
    {
        if (col.tag == "Player" && !col.isTrigger)
        {
            isHitPlayer = false;
        }
        if ((col.tag == "Player" || col.tag == "AdvancedAI" || col.tag == "BasicAI" || col.tag == "ScoutAI") && !col.isTrigger)
        {
            isHit = false;
        }
        //if((col.tag == "AdvancedAI" || col.tag == "BasicAI" || col.tag == "ScoutAI") && !col.isTrigger)
        //{
        //    if (parentDoor.allNearbyAI.Contains(col.gameObject))
        //    {
        //        parentDoor.allNearbyAI.Remove(col.gameObject);
        //    }

        //    if (parentDoor.visitor != null && parentDoor.visitor == col.gameObject)
        //    {
        //        parentDoor.visitor.gameObject.GetComponent<AIactions>().agent.avoidancePriority = parentDoor.visitor.gameObject.GetComponent<AIactions>().initialAvoidancePriority;
        //        parentDoor.visitor = null;
        //    }
        //}
    }
}
