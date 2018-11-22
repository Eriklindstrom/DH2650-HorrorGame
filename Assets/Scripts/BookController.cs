using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookController : MonoBehaviour {

    void OnCollisionEnter(Collision other)
    {
        GetComponent<AudioSource>().Play();
        Debug.Log("book" + other.gameObject.name);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("trigger " + other.gameObject.name);
    }
}
