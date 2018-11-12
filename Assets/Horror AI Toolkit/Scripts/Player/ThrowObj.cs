using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowObj : MonoBehaviour
{
    soundEmitter sound;
    float soundDurationTimer = 0;
    float soundDurationLength = 1;
    float currentLifeTime = 0;
    float maxLifeTime = 10;
    bool hasCollided;

	// Use this for initialization
	void Start ()
    {
        sound = GetComponent<soundEmitter>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (this.gameObject.activeInHierarchy)
        {
            currentLifeTime += Time.deltaTime;
            if (currentLifeTime > maxLifeTime)
            {
                currentLifeTime = 0;
                this.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                this.gameObject.SetActive(false);
            }

            if (hasCollided)
            {
                soundDurationTimer += Time.deltaTime;

                sound.volume = 40;

                if (soundDurationTimer > soundDurationLength)
                {
                    hasCollided = false;
                    soundDurationTimer = 0;
                    sound.volume = 0;
                }
            }
        }
	}

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag != "Player")
        {
            hasCollided = true;
            soundDurationTimer = 0;
        }
    }

}
