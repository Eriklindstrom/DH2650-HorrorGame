using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeFloor : MonoBehaviour {

    [SerializeField] private GameObject goDownCollider;
    [SerializeField] private Animation anim;

    void OnTriggerEnter(Collider ChangeScene) // can be Collider HardDick if you want.. I'm not judging you
    {
        anim.Play();        //Not set up
        SceneManager.LoadScene(1);
            //Application.LoadLevelAdditive(1); //1 is the build order it could be 1065 for you if you have that many scenes
    }
}
