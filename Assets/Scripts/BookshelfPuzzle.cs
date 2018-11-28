using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookshelfPuzzle : MonoBehaviour {

    [SerializeField] private int rewardPerBookReturned = 10;
    [SerializeField] private GameObject houseControllerObj;

    private HouseController houseController;

    private int currentNumberOfBooks;
    private int allTimeMaxNumberOfBooks;
    private bool puzzleStarted = false;

	void Start ()
    {
        currentNumberOfBooks = 0;
        allTimeMaxNumberOfBooks = 0;

        houseController = houseControllerObj.GetComponent<HouseController>();
	}
	
	void Update ()
    {
        if (!puzzleStarted) return;

		if(currentNumberOfBooks > allTimeMaxNumberOfBooks)
        {
            houseController.madness -= rewardPerBookReturned;
            allTimeMaxNumberOfBooks = currentNumberOfBooks;
        }
	}

    public void StartPuzzle()
    {
        puzzleStarted = true;
        allTimeMaxNumberOfBooks = currentNumberOfBooks;
    }

    public bool IsPuzzleStarted()
    {
        return puzzleStarted;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Book")
        {
            currentNumberOfBooks++;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Book")
        {
            currentNumberOfBooks--;
        }
    }
}
