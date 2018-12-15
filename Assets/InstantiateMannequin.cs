using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateMannequin : MonoBehaviour {

    [SerializeField] private GameObject Mannequin1;
    [SerializeField] private GameObject Mannequin2;

    private bool triggered = false;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(triggered)
        {
            Mannequin1.SetActive(true);
            Mannequin2.SetActive(true);
        }
	}

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "MainCamera")
            triggered = true;

    }
}
