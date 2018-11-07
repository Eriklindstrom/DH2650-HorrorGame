using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
* Handles rotation of doors. It would be cool to be able to open the doors with the vive controllers however. 
**/
public class rotateDoor : MonoBehaviour {

    [SerializeField] private GameObject RotateAround;   //Empty object on door to rotate around

    private bool rotate;                //Bool that handles opening of the door
    private bool rotateBack;            //Bool that handles closing of the door
    private float totAngle;             //Float to check when to stop rotating
	
	void Update () {
        if (Input.GetKeyDown("space") && totAngle > -90)    //Placeholder button, if the door is closed and you press a button
        {
            rotate = true;  
            rotateBack = false;
        }
        if(rotate)                                          //Rotate a total of -90 degrees
        {
            float angle = -90 * Time.deltaTime;
            totAngle += angle;
            transform.RotateAround(RotateAround.transform.position, Vector3.up, angle);
            if(totAngle < -90)
            {
                rotate = false;
            }
        }

        if (Input.GetKeyDown("space") && totAngle < -90)    //Placeholder button, if the door is opened and you press a button
        {
            rotateBack = true;
            rotate = false;
        }
        if (rotateBack)                                     //Rotate a total of 90 degrees
        {
            float angle = 90 * Time.deltaTime;
            totAngle += angle;
            transform.RotateAround(RotateAround.transform.position, Vector3.up, angle);
            if (totAngle > 0)
            {
                rotateBack = false;
            }
        }
    }
}
