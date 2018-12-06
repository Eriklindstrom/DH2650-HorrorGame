using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IfYouGetHereYouAreDead : MonoBehaviour {

    [SerializeField] private GameObject Mannequin;
    [SerializeField] private GameObject MannequinParent;
    [SerializeField] private GameObject PlayerCamera;
    [SerializeField] private Light lt; 

    private bool isFollowing = false;

    private float speed = 5f;

	
	// Update is called once per frame
	void Update () {
        Vector3 PlayerPos = new Vector3(PlayerCamera.transform.position.x, PlayerCamera.transform.position.y - 1.0f, PlayerCamera.transform.position.z);
        if(isFollowing)
        {
            Mannequin.transform.position = Vector3.MoveTowards(Mannequin.transform.position, PlayerPos, 0.02f);


            Vector3 targetDir = PlayerCamera.transform.position - MannequinParent.transform.position;
            // The step size is equal to speed times frame time.
            float step = speed * Time.deltaTime;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0f);
            Debug.DrawRay(transform.position, newDir, Color.red);
            // Move our position a step closer to the target.
            MannequinParent.transform.rotation = Quaternion.LookRotation(newDir);
        }
    }

    void OnTriggerEnter(Collider col)
    {
        isFollowing = true;
        lt.enabled = true;
    } 
}
