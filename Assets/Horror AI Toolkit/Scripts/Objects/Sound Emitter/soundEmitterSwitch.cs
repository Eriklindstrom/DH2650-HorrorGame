using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class soundEmitterSwitch : MonoBehaviour {

    [Tooltip("Whether or not the associated sound emitter is enabled. This object must be the first child of the sound emitter.")]
    public bool isOn;

    // Use this for initialization
    void Start ()
    {

    }

    void OnTriggerStay(Collider col)
    {
        if (col.tag == "Player" && !col.isTrigger)
        {
            if (GameController.sharedGameController.inputController.TestKeyDelay(KeyCode.E))
            {
                isOn = !isOn;
            }
        }
    }
}
