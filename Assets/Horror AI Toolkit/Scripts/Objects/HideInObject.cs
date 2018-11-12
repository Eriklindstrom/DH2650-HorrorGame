using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HideInObject : MonoBehaviour
{
    [HideInInspector]
    public GameObject player;
    [HideInInspector]
    public Camera playerCamera;
    GameObject door;
    GameObject doorHinge;
    [HideInInspector]
    public bool playerIsIn;

    [HideInInspector]
    public float doorYAngleMin;

    [Tooltip("The max amount the door will open by when opened. Default: 180.")]
    public float doorYAngleMax;

    [Tooltip("The max amount the door will open by when the player is peeking out of it. Default: 120.")]
    public float doorYAnglePeekMax;
    Vector3 doorStartPos;

    [HideInInspector]
    public float currentAngle;

    [HideInInspector]
    public bool isOpen;
    [HideInInspector]
    public bool isOpenFully;
    [HideInInspector]
    public bool isClosing;
    [HideInInspector]
    public bool isOpening;

    [HideInInspector]
    public bool playerRecentlyEntered;
    [HideInInspector]
    public float playerRecentlyEnteredTimerReset = 2.0f;

    GameObject hudPanel;
    GameObject closePanel;

    // Use this for initialization
    void Start ()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerCamera = player.transform.GetChild(0).GetComponent<Camera>();

        door = gameObject.transform.GetChild(0).gameObject;
        doorHinge = gameObject.transform.GetChild(1).gameObject;

        hudPanel = GameObject.Find("HUDCanvas").transform.GetChild(1).gameObject;
        closePanel = GameObject.Find("HUDCanvas").transform.GetChild(2).gameObject;

        doorYAngleMin = door.transform.localEulerAngles.y;
        doorStartPos = door.transform.localPosition;
    }

    void FixedUpdate()
    {
        if (door.transform.localEulerAngles.y > 90)
        {
            isOpen = true;
        }
        else if (door.transform.localEulerAngles.y <= 90)
        {
            isOpen = false;
        }

        if (door.transform.localEulerAngles.y > doorYAngleMax)
        {
            isOpenFully = true;
        }
        else
        {
            isOpenFully = false;
        }

        if (isOpening)
        {
            OpenDoor();
        }
        else if (isClosing)
        {
            CloseDoor();
        }

        if (playerIsIn)
        {
            if(Input.GetKey(KeyCode.W))
            {
                if (door.transform.localEulerAngles.y < doorYAnglePeekMax)
                {
                    OpenDoor();
                    playerCamera.gameObject.transform.Rotate(Vector3.up, -1);
                    playerCamera.gameObject.transform.Translate(new Vector3(0.005f,0,0.02f));
                }   

            }
            if (Input.GetKey(KeyCode.S))
            {
                if (door.transform.localEulerAngles.y > doorYAngleMin)
                {
                    CloseDoor();
                    playerCamera.gameObject.transform.Rotate(Vector3.up, 1);
                    playerCamera.gameObject.transform.Translate(new Vector3(-0.005f, 0,-0.02f));
                }

            }

            if(isOpenFully)
            {
                player.GetComponent<Player>().LeaveHideInObject();
            }

        }
    }

    void OnTriggerStay(Collider col)
    {
        if (col.tag == "Player" && !col.isTrigger)
        {
            if (InfiniteCameraCanSeePoint(Camera.main, this.transform.position))
            {
                if (closePanel != null && isOpen)
                {
                    closePanel.SetActive(true);
                    hudPanel.SetActive(false);
                }
                else if (hudPanel != null)
                {
                    hudPanel.SetActive(true);
                    closePanel.SetActive(false);
                }
            }
            else
            {
                hudPanel.SetActive(false);
                closePanel.SetActive(false);
            }

            if (InfiniteCameraCanSeePoint(Camera.main, this.transform.position))
            {
                RaycastHit hit;
                int layerMask = (1 << 10);
                layerMask = ~layerMask;
                if (Physics.Linecast(this.transform.position, player.transform.position, out hit, layerMask))
                {
                    if (hit.collider.gameObject == player)
                    {
                        if (GameController.sharedGameController.inputController.TestKeyDelay(KeyCode.E) && !player.GetComponent<Player>().isHiding && !isOpen)
                        {
                            player.GetComponent<Player>().StartHideInObject(this);
                            playerIsIn = true;
                            CloseDoorInstant();
                        }
                        else if (GameController.sharedGameController.inputController.TestKeyDelay(KeyCode.F) && !playerIsIn)
                        {
                            if (isOpen)
                                isClosing = true;
                            else
                                isOpening = true;
                        }
                    }
                }
            }
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.tag == "Player" && !col.isTrigger)
        {
            if (hudPanel != null)
            {
                hudPanel.SetActive(false);
            }
            if (closePanel != null)
            {
                closePanel.SetActive(false);
            }
        }
    }

    void OpenDoor()
    {
        if (door.transform.localEulerAngles.y < doorYAngleMax)
        {
            door.transform.RotateAround(doorHinge.transform.position, Vector3.up, 2);
        }
        else if (door.transform.localEulerAngles.y >= doorYAngleMax)
        {
            isOpening = false;
        }
    }

    void CloseDoor()
    {
        if (door.transform.localEulerAngles.y > doorYAngleMin)
        {
            door.transform.RotateAround(doorHinge.transform.position, Vector3.up, -2);
        }
        else if(door.transform.localEulerAngles.y <= doorYAngleMin)
        {
            door.transform.localEulerAngles = new Vector3(0, doorYAngleMin, 0);
            door.transform.localPosition = doorStartPos;
            isClosing = false;
        }
    }

    void CloseDoorInstant()
    {
        door.transform.localEulerAngles = new Vector3(0, doorYAngleMin, 0);
        door.transform.localPosition = doorStartPos;
    }

    public void OpenDoorInstant()
    {
        door.transform.RotateAround(doorHinge.transform.position, Vector3.up, doorYAngleMax);
    }

    bool InfiniteCameraCanSeePoint(Camera camera, Vector3 point)
    {
        Vector3 viewportPoint = camera.WorldToViewportPoint(point);
        return (viewportPoint.z > 0 && (new Rect(0, 0, 1, 1)).Contains(viewportPoint));
    }

}
