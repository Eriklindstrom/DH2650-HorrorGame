using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{

    public InputController inputController;
    public static GameController sharedGameController;
    public ObjectPooler throwObjPooler;

    void Awake()
    {
        sharedGameController = this;
    }

    // Use this for initialization
    void Start ()
    {
        inputController = this.GetComponent<InputController>();
        throwObjPooler = gameObject.transform.GetChild(0).GetComponent<ObjectPooler>();
	}
	
}
