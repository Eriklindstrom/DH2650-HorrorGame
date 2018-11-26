using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterBedroomController : MonoBehaviour {

    [SerializeField] private GameObject houseControllerObject;
    [SerializeField] private GameObject sheepPainting;
    [SerializeField] private GameObject RotateAroundSheep;   //Empty object on door to rotate around
    [SerializeField] private GameObject PortraitPainting;

    private HouseController houseController;


    private Quaternion targetRotation;
    private bool paintingFlipped = false;
    private float time;

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
        if (houseController.madnessPercentage > 0.2f && !paintingFlipped)
        {
            RotatePainting();
            ScaryPainting();
        }
            
    }

    void RotatePainting()
    {
        time += Time.deltaTime;
        sheepPainting.transform.RotateAround(RotateAroundSheep.transform.position, Vector3.right, (5.0f * Time.deltaTime));
        if(time > 1.5f)
        {
            paintingFlipped = true;
        }
    }

    void ScaryPainting()
    {
        MeshRenderer portraitRend = PortraitPainting.GetComponent<MeshRenderer>();
        portraitRend.material.shader = Shader.Find("Standard");
        portraitRend.material.SetColor("_Color", Color.red);
    }
}
