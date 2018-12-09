using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRTK.Examples
{
    public class HammerSmashPiggy : MonoBehaviour
    {
        void OnCollisionEnter(Collision col)
        {
            if (col.gameObject.tag == "Hammer")
            {
                gameObject.GetComponent<Rigidbody>().isKinematic = false;
                transform.parent.GetChild(0).gameObject.SetActive(true);
                Destroy(gameObject, 10.0f);
            }
        }
    }
}
