using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookController : MonoBehaviour {

    [SerializeField] private bool mainBook = false;

    [SerializeField] private GameObject bookGameObj;
    [SerializeField] private GameObject hallwayCtrlObj;

    private BookshelfPuzzle puzzleController;
    private HallwayController hallwayController;

    void Start()
    {
        puzzleController = bookGameObj.GetComponent<BookshelfPuzzle>();
        hallwayController = hallwayCtrlObj.GetComponent<HallwayController>();
    }

    void OnCollisionEnter(Collision other)
    {
        GetComponent<AudioSource>().volume = 0.1f;
        GetComponent<AudioSource>().Play();

        if(other.gameObject.tag == "Floor" && !puzzleController.IsPuzzleStarted())
        {
            puzzleController.StartPuzzle();
            hallwayController.bookPuzzleActive = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        //Debug.Log("trigger " + other.gameObject.name);
    }
}
