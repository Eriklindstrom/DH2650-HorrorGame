using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ViveInput : MonoBehaviour {

    [SteamVR_DefaultAction("Squeeze")]
    public SteamVR_Action_Single squeezeAction;
    public SteamVR_Action_Vector2 touchPadAction;
    public GameObject Player;
    [SerializeField] private GameObject Camera;
    [SerializeField] private float moveSpeed = 0.1f;


    private Vector3 previousPos;

    public isColliding iscolliding;

	void Update ()
    {
        if (SteamVR_Input._default.inActions.Teleport.GetStateDown(SteamVR_Input_Sources.Any))
        {
            //Debug.Log("Teleport down");
        }
        if (SteamVR_Input._default.inActions.GrabPinch.GetStateUp(SteamVR_Input_Sources.Any))
        {
            //Debug.Log("GrabPinch up");
        }

        float triggerValue = squeezeAction.GetAxis(SteamVR_Input_Sources.Any);
        if(triggerValue >= 0.0f)
        {
            //Debug.Log(triggerValue);
        }

        Vector2 touchPadValue = touchPadAction.GetAxis(SteamVR_Input_Sources.Any);
        if(touchPadValue.magnitude >= 0.8)
        {         
            if(!iscolliding.currentlyColliding)
            {
                Vector3 movePos = new Vector3(touchPadValue.x * moveSpeed, 0, touchPadValue.y * moveSpeed);
                float cameraRot = Camera.transform.eulerAngles.y;
                movePos = Quaternion.AngleAxis(cameraRot, Vector3.up) * movePos;
                Player.transform.position += movePos * moveSpeed;
                //previousPos += movePos * moveSpeed * 0.9999f;
            }
            else
            {
                Vector3 movePos = new Vector3(touchPadValue.x * moveSpeed, 0, touchPadValue.y * moveSpeed);
                float cameraRot = Camera.transform.eulerAngles.y;
                movePos = Quaternion.AngleAxis(cameraRot, Vector3.up) * movePos;
                Player.transform.position -= movePos * moveSpeed;
                iscolliding.currentlyColliding = false;

                //iscolliding.currentlyColliding = false;
                /*
                Vector3 movePos = new Vector3(touchPadValue.x * moveSpeed, 0, touchPadValue.y * moveSpeed);
                float cameraRot = Camera.transform.eulerAngles.y;
                movePos = Quaternion.AngleAxis(cameraRot, Vector3.up) * movePos;
                Player.transform.position += movePos * moveSpeed;
                //Vector3 movePos = new Vector3(touchPadValue.x * moveSpeed, 0, touchPadValue.y * moveSpeed);
                //Player.transform.position -= Player.transform.position;*/
            }
        }
    }
}
