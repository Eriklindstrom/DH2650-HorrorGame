using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{

    public float keyDelay = 0.25f;
    float timePassed = 0f;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        timePassed += Time.deltaTime;
	}

    public bool TestKeyDelay(KeyCode key)
    {
        if(Input.GetKey(key) && timePassed >= keyDelay)
        {
            timePassed = 0.0f;
            return true;
        }

        return false;
    }

    public bool TestKey(KeyCode key)
    {
        if(Input.GetKey(key))
        {
            return true;
        }

        return false;
    }
}
