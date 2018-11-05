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
            Vector3 movePos = new Vector3(touchPadValue.x * moveSpeed, 0, touchPadValue.y * moveSpeed);
            float cameraRot = Camera.transform.eulerAngles.y;
            movePos = Quaternion.AngleAxis(cameraRot, Vector3.up) * movePos;
            Player.transform.position += movePos * moveSpeed;
        }
        /*if(touchPadValue != Vector2.zero)
        {
            Player.transform.position = touchPadValue;
            Debug.Log(touchPadValue);
        }*/
    }
}
