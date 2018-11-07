using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class isColliding : MonoBehaviour
{

    public bool currentlyColliding = false;

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "RoomWalls")
        {
            currentlyColliding = true;
        }
        else
        {
            currentlyColliding = false;
        }
    }

    void OnCollisionStay(Collision col)
    {
        if (col.gameObject.tag == "RoomWalls")
        {
            currentlyColliding = true;
        }
        else
        {
            currentlyColliding = false;
        }
    }

        void OnCollisionExit(Collision col)
    {
        if (col.gameObject.name == "RoomWalls")
        {
            currentlyColliding = false;
        }
    }
}
