using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrainController : MonoBehaviour
{

    [SerializeField] private int madnessPenalty = 10;
    [SerializeField] private GameObject houseControllerObj;

    private HouseController houseController;


    private bool desposedOf = false;
    private bool eatingBrain = false;
    private float eatingTimer = 0.0f;

    void Start()
    {
        houseController = houseControllerObj.GetComponent<HouseController>();
    }

    /**
     Plant and remove insects, and update their locations */
    void Update()
    {
        if (eatingBrain)
            eatingTimer += Time.deltaTime;

        if (eatingTimer >= 0.5f)
        {
            houseController.madness += madnessPenalty;
            desposedOf = true;
            //gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }

    /**
     Players rewarded with 20 seconds sanity for putting apples in the toilet */
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("enter trigger :  " + other.name);

        if (other.gameObject.tag == "MainCamera")
        {
            Debug.Log("Exiting head trigger");
            eatingBrain = true;
        }
    }

    /**
    Players rewarded with 20 seconds sanity for putting apples in the toilet */
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "MainCamera")
        {
            Debug.Log("Exiting head trigger");
            eatingBrain = false;
            eatingTimer = 0.0f;
        }
    }
}
