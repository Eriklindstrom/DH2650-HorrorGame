using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickeringLight : MonoBehaviour {

    Light flickeringLight;
    public float minWaitTime = 1.0f;
    public float maxWaitTime = 2.0f;

	// Use this for initialization
	void Start ()
    {
        flickeringLight = GetComponent<Light>();
        StartCoroutine(Flashing());
	}
	
	IEnumerator Flashing()
    {
        while(true)
        {
            yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));
            flickeringLight.enabled = !flickeringLight.enabled;
        }
    }
}
