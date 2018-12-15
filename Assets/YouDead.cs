using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class YouDead : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    void OnCollisionEnter(Collision col)
    {
        Debug.Log("Name " + col.transform.name);
        if (col.transform.tag == "MainCamera")
        {
            Debug.Log("test");
            SceneManager.LoadScene(0);
        }
    }
}
