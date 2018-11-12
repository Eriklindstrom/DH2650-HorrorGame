using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableObjectTrigger : MonoBehaviour
{

    public GameObject TargetObject;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !other.isTrigger)
        {
            TargetObject.SetActive(true);
        }
    }
}
