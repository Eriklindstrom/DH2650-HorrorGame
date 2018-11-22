using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRTK.Examples
{
    using UnityEngine;

    public class KeyUnlockDoor : VRTK_InteractableObject
    {
        [SerializeField]
        private GameObject DoorObject;
        //public Openable_Door openDoorScript;
        //public LockedDoor lockedDoorScript;

        // Use this for initialization
        void Start()
        {
            Openable_Door openDoorScript = DoorObject.GetComponent<Openable_Door>();
            LockedDoor lockedDoorScript = DoorObject.GetComponent<LockedDoor>();
        }

        void OnCollisionEnter(Collision col)
        {
            //Debug.Log(col.gameObject.name);
            if (col.gameObject.tag == "LockedDoor")
            {
                //Debug.Log("triggered!!!!");
                DoorObject.GetComponent<Openable_Door>().enabled = true;
                DoorObject.GetComponent<LockedDoor>().enabled = false;
                Destroy(gameObject);
                //openDoorScript.enabled = true;
                //col.GetComponent(Openable_Door).enabled = false;
                //openDoorScript.isActiveAndEnabled(true);
                //lockedDoorScript.enabled(false);
            }
        }
    }
}
