using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyWall : MonoBehaviour
{
    float health = 100;
    GameObject topHalf;
    GameObject botHalf;
    bool topDestroyed;
    UnityEngine.AI.OffMeshLink navLink;
    float initialCostOverride;

	// Use this for initialization
	void Start ()
    {
        topHalf = transform.GetChild(2).gameObject;
        botHalf = transform.GetChild(3).gameObject;
        topDestroyed = false;

        navLink = GetComponent<UnityEngine.AI.OffMeshLink>();
        initialCostOverride = navLink.costOverride;
    }

    public void ReduceHealth(float amount)
    {
        health -= amount;

        if (health <= 50 && !topDestroyed)
        {
            topHalf.GetComponent<Rigidbody>().isKinematic = false;
            topHalf.GetComponent<Rigidbody>().AddForce(transform.forward * 300);
            topDestroyed = true;
        }

        if (health <= 0)
        {
            DestroySelf();
        }
    }
	
    void DestroySelf()
    {
        this.gameObject.SetActive(false);
    }

    public void SetCostOverride(float costOverride)
    {
        navLink.costOverride = costOverride;
    }

    public void ResetCostOverride()
    {
        navLink.costOverride = initialCostOverride;
    }
}
