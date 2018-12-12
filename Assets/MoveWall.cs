using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWall : MonoBehaviour {

    [SerializeField] private GameObject Wall;

    private float m_distanceTraveled = 0.0f;
    private bool moveWall = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(moveWall)
        {
            if (m_distanceTraveled < 10.0f)
            {
                Vector3 oldPosition = Wall.transform.position;
                Wall.transform.Translate(-8.0f * Time.deltaTime, 0, 0);
                m_distanceTraveled += Vector3.Distance(oldPosition, Wall.transform.position);
            }
            else
            {
                m_distanceTraveled = 0.0f;
                moveWall = false;
                Destroy(this);
            }

        }
    }

    void OnTriggerEnter(Collider col)
    {
        moveWall = true;
        
    }
}
