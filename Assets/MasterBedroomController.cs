using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterBedroomController : MonoBehaviour {

    [SerializeField]
    GameObject houseControllerObject;
    [SerializeField]
    private GameObject sheepPainting;

    private HouseController houseController;


    private Quaternion targetRotation;
    private bool paintingFlipped = false;

    // Use this for initialization
    void Start ()
    {
        targetRotation = sheepPainting.transform.rotation;
        houseController = houseControllerObject.GetComponent<HouseController>();
    }

    void Update()
    {
        Horrify();
    }

    void Horrify()
    {
        RotatePainting();
        if (houseController.madnessPercentage > 0.2 && !paintingFlipped)
        { }
            
    }

    void RotatePainting()
    {
        targetRotation *= Quaternion.AngleAxis(60, Vector3.up);
        Debug.Log(targetRotation);
        targetRotation = Quaternion.Lerp(transform.rotation, transform.rotation, 10 * Time.deltaTime);
        /* float minRotation = -45;
         float maxRotation = 45;
         Vector3 currentRotation = transform.localRotation.eulerAngles;
         currentRotation.y = Mathf.Clamp(currentRotation.y, minRotation, maxRotation);
         Debug.Log(currentRotation.y);
         sheepPainting.transform.localRotation = Quaternion.Euler(currentRotation);*/
    }
}
