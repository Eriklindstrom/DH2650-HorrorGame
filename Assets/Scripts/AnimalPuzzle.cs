using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalPuzzle : MonoBehaviour {

    [SerializeField] GameObject houseControllerObj;

    [SerializeField] GameObject bear;    
    [SerializeField] GameObject big;
    [SerializeField] GameObject bunny;
    [SerializeField] GameObject sheep;

    private HouseController houseController;

    private bool bunnySpawned;
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
            houseController.madness -= 20;
            puzzleSolved = true;
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
