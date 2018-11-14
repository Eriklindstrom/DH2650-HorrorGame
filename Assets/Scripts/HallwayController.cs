using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HallwayController : MonoBehaviour {

    [SerializeField] GameObject houseControllerObject;
    [SerializeField] GameObject rugObject;
    [SerializeField] List<GameObject> books;
    [SerializeField] List<GameObject> armCharis;

    private HouseController houseController;

    private bool chairsFlipped = false;

	void Start ()
    {
        houseController = houseControllerObject.GetComponent<HouseController>();              
	}
    

	void Update ()
    {
        Horrify();
	}

    void Horrify()
    {
        RotateCarpet();
        MoveBook();

        if (houseController.madnessPercentage > 0.2 && !chairsFlipped) FlipChairs();
    }

    void RotateCarpet()
    {
        float rugAngle = rugObject.transform.rotation.y * (180 / Mathf.PI);

        if (houseController.madnessPercentage < 0.4 &&  rugAngle > 0.1)
        {
            rugObject.transform.Rotate(new Vector3(0, -0.01f, 0));
        }
        else if (houseController.madnessPercentage > 0.4 && rugAngle < 20)
        {
            rugObject.transform.Rotate(new Vector3(0, 0.01f, 0));
        }
    }

    void MoveBook()
    {
        GameObject topBook = books[books.Count - 1];
        if (houseController.madnessPercentage > 0.6 && topBook.transform.position.y >= 0.2)
            topBook.transform.position += new Vector3(0, 0,-0.0001f);
    }

    void FlipChairs()
    {
        StartCoroutine(houseController.LightsOut(0.3f));

        armCharis[0].transform.Rotate(new Vector3(0, 270, 0));
        armCharis[0].transform.position += new Vector3(1.4f, 0, 0);

        chairsFlipped = true;
    }
}
