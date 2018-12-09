using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalPuzzle : MonoBehaviour {

    [SerializeField] private GameObject houseControllerObj;
    [SerializeField] private GameObject bunny;
    [SerializeField] private GameObject topDrawer;
    [SerializeField] float puzzleReward = 20.0f;


    private HouseController houseController;

    private bool bunnySpawned = true;
    private bool puzzleSolved;

    private int animalsOnShelf = 0;

    void Start ()
    {
        houseController = houseControllerObj.GetComponent<HouseController>();

        bunnySpawned = false;
        puzzleSolved = false;
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if(houseController.madnessPercentage > 0.3 && !bunnySpawned)
        {
            bunny.SetActive(true);
            bunnySpawned = true;
        }

        if(animalsOnShelf == 4 && !puzzleSolved)
        {
            houseController.madness -= puzzleReward;
            puzzleSolved = true;
            topDrawer.transform.position += new Vector3(0.0f, 0.0f, 11.0f);
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "StuffedAnimal")
        {
            animalsOnShelf++;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "StuffedAnimal")
        {
            animalsOnShelf--;
        }
    }
}
