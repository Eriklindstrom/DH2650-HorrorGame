using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BedRoom2Controller : MonoBehaviour {

    [SerializeField] private GameObject Mannequin;
    [SerializeField] private GameObject CurtainsLeft;
    [SerializeField] private GameObject houseControllerObject;
    [SerializeField] private GameObject bedroomDoor;
    private HouseController houseController;

    private bool OpenedRoom = false;
    // Use this for initialization
    void Start () {
        houseController = houseControllerObject.GetComponent<HouseController>();
    }
	
    void Update()
    {
        Horrify();
        //Debug.Log(bedroomDoor.transform.localEulerAngles.z);
        if (bedroomDoor.transform.localEulerAngles.z > 280.0f || bedroomDoor.transform.localEulerAngles.z < 260.0f)
        {
            OpenedRoom = true;
        }
    }

    void Horrify()
    {
        if (houseController.madnessPercentage > 0.2f)// && OpenedRoom)
        {
            RemoveCurtains();
            Mannequin.SetActive(true);
        }

    }

    void RemoveCurtains()
    {
        CurtainsLeft.transform.localScale = new Vector3(0.5f, 1, 1);
    }
}
