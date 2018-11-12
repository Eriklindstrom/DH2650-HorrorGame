using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class Door : MonoBehaviour
{
    [HideInInspector]
    public GameObject player;
    [HideInInspector]
    public Camera playerCamera;
    GameObject door;
    GameObject doorHinge;
    [HideInInspector]
    public bool playerIsUsing;

    float doorYAngleStart;
    float doorYAngleMin = 0;
    float doorYPeekAngleMax = 40;
    float doorYAngleCurrent;
    Vector3 doorStartPos;

    float currentRotation;

    [HideInInspector]
    public GameObject snapPointFront;
    [HideInInspector]
    public GameObject snapPointBack;

    [HideInInspector]
    public bool isOpen;
    [HideInInspector]
    public bool isClosing;
    [HideInInspector]
    public bool frontOpen;
    [HideInInspector]
    public bool backOpen;
    [HideInInspector]
    public bool isOpening;

    [Tooltip("The player will not be able to operate locked doors. However, advanced AI may break them down.")]
    public bool isLocked;

    HingeJoint hinge;

    [HideInInspector]
    public UnityEngine.AI.OffMeshLink navLink;
    float initialCostOverride;

    public GameObject visitor;
    [HideInInspector]
    public List<GameObject> allNearbyAI;

    GameObject hudPanel;
    GameObject closePanel;
    GameObject usingPanel;

    // Use this for initialization
    void Start ()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerCamera = player.transform.GetChild(0).GetComponent<Camera>();

        door = gameObject.transform.GetChild(0).gameObject;
        doorHinge = gameObject.transform.GetChild(1).gameObject;
        snapPointFront = gameObject.transform.GetChild(2).gameObject;
        snapPointBack = gameObject.transform.GetChild(3).gameObject;

        doorYAngleStart = door.transform.localEulerAngles.y;
        doorStartPos = door.transform.localPosition;

        hudPanel = GameObject.Find("HUDCanvas").transform.GetChild(0).gameObject;
        closePanel = GameObject.Find("HUDCanvas").transform.GetChild(2).gameObject;
        usingPanel = GameObject.Find("HUDCanvas").transform.GetChild(3).gameObject;

        currentRotation = 0;

        door.GetComponent<Rigidbody>().isKinematic = true;

        hinge = door.GetComponent<HingeJoint>();
        navLink = GetComponent<UnityEngine.AI.OffMeshLink>();
        initialCostOverride = navLink.costOverride;
    }

    void FixedUpdate()
    {
        FindNearbyAI();

        if (visitor!=null)
        {
            visitor.GetComponent<AIactions>().agent.avoidancePriority = 0;
            if(!visitor.GetComponent<AIactions>().isAlive || (Vector3.Distance(this.transform.position, visitor.transform.position) > 5.0f) || !visitor.activeInHierarchy)
            {
                visitor = null;
            }
        }

        doorYAngleCurrent = door.transform.localEulerAngles.y;
        if (doorYAngleCurrent > 0)
            isOpen = true;
        else
            isOpen = false;

        if(isLocked)
        {
            navLink.costOverride = 5;
        }
        else
        {
            navLink.costOverride = initialCostOverride;
        }

        if(!isOpen)
        {
            door.GetComponent<UnityEngine.AI.NavMeshObstacle>().enabled = true;
            navLink.activated = true;
        }
        else if(!playerIsUsing)
        {
            door.GetComponent<UnityEngine.AI.NavMeshObstacle>().enabled = false;
            navLink.activated = false;
        }

        if (door.transform.localEulerAngles.y > 50 && door.transform.localEulerAngles.y < 180)
        {
            backOpen = true;
            frontOpen = false;
        }
        else if (door.transform.localEulerAngles.y < 310 && door.transform.localEulerAngles.y > 180)
        {
            frontOpen = true;
            backOpen = false;
        }

        if (isClosing)
            CloseDoor();
        else if (isOpening)
            OpenDoor();

        if (playerIsUsing)
        {
            if (backOpen && (door.transform.localEulerAngles.y > 50 && door.transform.localEulerAngles.y < 180))
            {
                player.GetComponent<Player>().StopUsingDoor();
            }
            else if (frontOpen && (door.transform.localEulerAngles.y < 310 && door.transform.localEulerAngles.y > 180))
            {

                player.GetComponent<Player>().StopUsingDoor();
            }

            if (Input.GetKey(KeyCode.W))
            {
                if (currentRotation <= doorYPeekAngleMax)
                {
                    currentRotation += 1;

                    float pushDirection = 0;

                    if(snapPointFront.GetComponent<DoorSnapPoint>().isHit)
                    {
                        pushDirection = 1;
                    }
                    else if (snapPointBack.GetComponent<DoorSnapPoint>().isHit)
                    {
                        pushDirection = -1;
                    }

                    door.transform.RotateAround(doorHinge.transform.position, Vector3.up, pushDirection);
                    playerCamera.gameObject.transform.Rotate(Vector3.up, -pushDirection * 0.5f);
                    playerCamera.gameObject.transform.Translate(new Vector3(0.00f, 0, 0.023f));
                }

            }
            else if (Input.GetKey(KeyCode.S))
            {
                if (currentRotation >= doorYAngleMin)
                {
                    currentRotation -= 1;

                    float pushDirection = 0;

                    if (snapPointFront.GetComponent<DoorSnapPoint>().isHit)
                    {
                        pushDirection = -1;
                    }
                    else if (snapPointBack.GetComponent<DoorSnapPoint>().isHit)
                    {
                        pushDirection = 1;
                    }

                    door.transform.RotateAround(doorHinge.transform.position, Vector3.up, pushDirection);
                    playerCamera.gameObject.transform.Rotate(Vector3.up, -pushDirection * 0.5f);
                    playerCamera.gameObject.transform.Translate(new Vector3(0.00f, 0, -0.023f));
                }
            }    
            else if(GameController.sharedGameController.inputController.TestKeyDelay(KeyCode.E))
            {
                if (snapPointFront.GetComponent<DoorSnapPoint>().isHit)
                {
                    frontOpen = true;
                }
                else if (snapPointBack.GetComponent<DoorSnapPoint>().isHit)
                {
                    backOpen = true;
                }
                StartClosing();
                player.GetComponent<Player>().StopUsingDoor();
            }
            else if (GameController.sharedGameController.inputController.TestKeyDelay(KeyCode.F))
            {
                StartOpening();
                door.GetComponent<Rigidbody>().isKinematic = false;
                player.GetComponent<Player>().StopUsingDoor();
            }

        }
    }

    void OnTriggerStay(Collider col)
    {
        if (col.tag == "Player" && !col.isTrigger)
        {
            if (InfiniteCameraCanSeePoint(Camera.main, this.transform.position))
            {
                if(usingPanel != null  && playerIsUsing)
                {
                    usingPanel.SetActive(true);
                    hudPanel.SetActive(false);
                    closePanel.SetActive(false);
                }
                else if (closePanel != null && isOpen)
                {
                    closePanel.SetActive(true);
                    hudPanel.SetActive(false);
                    usingPanel.SetActive(false);
                }
                else if (hudPanel != null)
                {
                    hudPanel.SetActive(true);
                    closePanel.SetActive(false);
                    usingPanel.SetActive(false);
                }
            }
            else
            {
                hudPanel.SetActive(false);
                closePanel.SetActive(false);
                //usingPanel.SetActive(false);
            }

            if (InfiniteCameraCanSeePoint(Camera.main, this.transform.position))
            {
                RaycastHit hit;
                if (Physics.Linecast(this.transform.position, player.transform.position, out hit))
                {
                    if (hit.collider.gameObject == player)
                    {
                        if (GameController.sharedGameController.inputController.TestKeyDelay(KeyCode.F) && !player.GetComponent<Player>().isUsingDoor)
                        {
                            if (isOpen)
                            {
                                //Debug.Log("close door");
                                StartClosing();
                            }
                            else if (!isOpen)
                            {
                                StartOpening();
                                if (frontOpen)
                                {
                                    OpenDoorForce(true);
                                }
                                else
                                {
                                    OpenDoorForce(false);
                                }
                            }
                        }

                        if (GameController.sharedGameController.inputController.TestKeyDelay(KeyCode.E) && !player.GetComponent<Player>().isUsingDoor && !isOpen)
                        {
                            if (isLocked)
                            {
                                Debug.Log("isLocked");
                            }
                            else
                            {
                                player.GetComponent<Player>().StartUsingDoor(this);

                                if (snapPointFront.GetComponent<DoorSnapPoint>().isHit)
                                {
                                    frontOpen = true;
                                }
                                else if (snapPointBack.GetComponent<DoorSnapPoint>().isHit)
                                {
                                    backOpen = true;
                                }

                                playerIsUsing = true;
                            }
                        }
                    }
                }
            }
        }

        if ((col.tag == "AdvancedAI" || col.tag == "BasicAI" || col.tag == "ScoutAI") && !col.isTrigger)
        {
            if (!allNearbyAI.Contains(col.gameObject))
            {
                allNearbyAI.Add(col.gameObject);
                allNearbyAI = allNearbyAI.OrderBy(x => Vector3.Distance(this.transform.position, x.transform.position)).ToList();
            }

            if (visitor == null)
            {
                visitor = allNearbyAI[0];
                visitor.GetComponent<AIactions>().ForceRecalculatePath();
                //parentDoor.visitor.gameObject.GetComponent<AIactions>().agent.avoidancePriority = 0;
            }
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.tag == "Player" && !col.isTrigger)
        {
            usingPanel.SetActive(false);
            hudPanel.SetActive(false);        
            closePanel.SetActive(false);
        }

        if ((col.tag == "AdvancedAI" || col.tag == "BasicAI" || col.tag == "ScoutAI") && !col.isTrigger)
        {
            if (allNearbyAI.Contains(col.gameObject))
            {
                allNearbyAI.Remove(col.gameObject);
                allNearbyAI = allNearbyAI.OrderBy(x => Vector3.Distance(this.transform.position, x.transform.position)).ToList();
            }

            if (visitor != null && visitor == col.gameObject)
            {
                visitor.gameObject.GetComponent<AIactions>().agent.avoidancePriority = visitor.gameObject.GetComponent<AIactions>().initialAvoidancePriority;
                visitor = null;
            }
        }
    }

    public void CloseDoor()
    {
        if (door.transform.localEulerAngles.y <= 10)
        {
            door.transform.localEulerAngles = new Vector3(0, doorYAngleStart, 0);
            door.transform.localPosition = doorStartPos;
            isClosing = false;
            frontOpen = false;
            backOpen = false;
            //Debug.Log("Door closed");
        }
        else
        {
            if (frontOpen)
            {
                door.transform.RotateAround(doorHinge.transform.position, Vector3.up, 5);
            }
            else if (backOpen)
            {
                door.transform.RotateAround(doorHinge.transform.position, Vector3.up, -5);
            }
        }

        currentRotation = 0;
    }

    public void OpenDoor()
    {
        //Debug.Log("DOOR SHOULD BE OPENNING");
        if (frontOpen && door.transform.localEulerAngles.y > 60)
        {
            //door.transform.localEulerAngles = new Vector3(0, doorYAngleStart, 0);
            //door.transform.localPosition = doorStartPos;
            isOpening = false;
            //Debug.Log("Door Opened front " +  door.transform.localEulerAngles.y);
            OpenDoorForce(true);
        }
        else if (backOpen && (door.transform.localEulerAngles.y != 0 && door.transform.localEulerAngles.y < 300))
        {
            isOpening = false;
            //Debug.Log("Door Opened back " + door.transform.localEulerAngles.y);
            OpenDoorForce(false);
        }
        else
        {
            if (frontOpen)
            {
                door.transform.RotateAround(doorHinge.transform.position, Vector3.up, 5);
            }
            else if (backOpen)
            {
                door.transform.RotateAround(doorHinge.transform.position, Vector3.up, -5);
            }
        }        
    }

    public void OpenDoorForce(bool isFront)
    {
        Rigidbody rb = door.GetComponent<Rigidbody>();

        if (isFront)
        {
            rb.AddForce(door.transform.forward * 20);
        }
        else
        {
            rb.AddForce(-door.transform.forward * 20);
        }
    }

    public void StartClosing()
    {
        isClosing = true;
        door.GetComponent<Rigidbody>().isKinematic = true;
    }

    public bool StartOpening()
    {
        //if(playerIsUsing)
        //{
        //    player.GetComponent<Player>().StopUsingDoor();
        //}

        if (isLocked)
        {
            Debug.Log("door is locked");
            return false;
        }
        else
        {
            //Debug.Log("open door");
            isOpening = true;
            door.GetComponent<Rigidbody>().isKinematic = false;

            if (snapPointFront.GetComponent<DoorSnapPoint>().isHit)
            {
                frontOpen = true;

                JointSpring spr = hinge.spring;
                spr.spring = 10;
                spr.targetPosition = 90;
                hinge.spring = spr;

                //JointLimits limits = hinge.limits;
                //limits.min = 30;
                //limits.max = 180;
                //hinge.limits = limits;
            }
            else if (snapPointBack.GetComponent<DoorSnapPoint>().isHit)
            {
                backOpen = true;

                JointSpring spr = hinge.spring;
                spr.spring = 10;
                spr.targetPosition = -90;
                hinge.spring = spr;

                //JointLimits limits = hinge.limits;
                //limits.min = -180;
                //limits.max = -30;
                //hinge.limits = limits;
            }
            return true;
        }
    }

    public void BreakDownDoor()
    {
        Debug.Log("break down");
        this.gameObject.SetActive(false);
    }

    public void FindNearbyAI()
    {
        foreach(GameObject obj in allNearbyAI)
        {
            if(obj != visitor)
            {
                obj.GetComponent<AIactions>().StopMovement();
                obj.GetComponent<AIactions>().isIdle = false;
            }          
        }
    }

    bool InfiniteCameraCanSeePoint(Camera camera, Vector3 point)
    {
        Vector3 viewportPoint = camera.WorldToViewportPoint(point);
        return (viewportPoint.z > 0 && (new Rect(0, 0, 1, 1)).Contains(viewportPoint));
    }


}
