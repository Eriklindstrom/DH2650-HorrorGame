using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateMannequin : MonoBehaviour {

    [SerializeField] private GameObject Mannequin;
    [SerializeField] private GameObject PlayerCamera;
    // Use this for initialization

    /*private float damping = 2.0f;
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider col)
    {
        Vector3 newRot = new Vector3.RotateTowards(Mannequin.transform.position, PlayerPos, 0.02f);
        Mannequin.transform.position = Quaternion.LookRotation(newRot);
    }*/
}
