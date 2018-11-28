using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookController : MonoBehaviour {

    [SerializeField] private bool mainBook = false;

    [SerializeField] private GameObject bookGameObj;

    private BookshelfPuzzle puzzleController;

    void Start()
    {
        puzzleController = bookGameObj.GetComponent<BookshelfPuzzle>();
    }

    void OnCollisionEnter(Collision other)
    {
        GetComponent<AudioSource>().Play();

        if(other.gameObject.tag == "Floor" && !puzzleController.IsPuzzleStarted())
        {
            puzzleController.StartPuzzle();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("trigger " + other.gameObject.name);
    }
}
