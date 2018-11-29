using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOffLightBedroom2 : MonoBehaviour {

    [SerializeField] private Light Light;

    bool collided;

    /*void Start()
    {
        lt = Light.GetComponent<Light>();
    }*/

    IEnumerator OnTriggerEnter(Collider collider)
    {
        collided = true;
        yield return new WaitForSeconds(2);
        if (collided)
        {
            //Light.SetActive(false);
            Light.intensity = 0.1f;
        }
    }

    void OnCollisionExit()
    {
        collided = false;
    }
}
