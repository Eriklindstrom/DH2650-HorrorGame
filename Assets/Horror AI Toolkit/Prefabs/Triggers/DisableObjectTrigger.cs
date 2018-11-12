using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableObjectTrigger : MonoBehaviour
{
	public GameObject TargetObject;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !other.isTrigger)
        {
            TargetObject.SetActive(false);
        }
    }
}
