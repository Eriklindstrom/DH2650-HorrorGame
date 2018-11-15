using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppleController : MonoBehaviour {

    [SerializeField] private GameObject antPrefab;
    [SerializeField] private GameObject houseControllerObj;
    [SerializeField] private GameObject toilet;

    bool[] interactionTrigggers;

    private HouseController houseController;
    private float numberOfAnts;
    private List<KeyValuePair<GameObject, float>> appleAnts;
    private bool desposedOf = false;

    void Start()
    {
        houseController = houseControllerObj.GetComponent<HouseController>();
        appleAnts = new List<KeyValuePair<GameObject, float>>();
        interactionTrigggers = new bool[4];
    }
	
    /**
     Plant and remove insects, and update their locations */
	void Update ()
    {
		if(numberOfAnts < houseController.madnessPercentage * 20)
        {
            GameObject ant = Instantiate(antPrefab);
            //ant.transform.localScale = new Vector3(5e-06f, 5e-06f, 5e-06f); //Need to scale ants down
            ant.transform.SetParent(transform, false);
            PlaceAntOnMesh(ant);
            numberOfAnts++;

            appleAnts.Add(new KeyValuePair<GameObject, float>(ant, 0));
        }


        for(int i = 0; i < appleAnts.Count; i++)
        {
            GameObject appleAnt = appleAnts[i].Key;
            float angle = appleAnts[i].Value;

            Vector3 oldPos = appleAnt.transform.position;
            Vector3 newPos = oldPos + appleAnt.transform.rotation * Vector3.forward * 0.005f * Time.deltaTime;
            
            Vector3 v1 = oldPos - transform.GetChild(0).GetComponent<Renderer>().bounds.center;
            Vector3 v2 = newPos - transform.GetChild(0).GetComponent<Renderer>().bounds.center;
            Vector3 v3 = Vector3.ClampMagnitude(v2 * 2, transform.GetComponent<SphereCollider>().radius);
            Vector3 destination = transform.GetChild(0).GetComponent<Renderer>().bounds.center + v3;

            appleAnt.transform.position = destination;
            appleAnt.transform.rotation = Quaternion.FromToRotation(Vector3.up, v3);

            float rand = Random.Range(0.0f, 100.0f);
            if (rand > 95.0)
            {
                angle += 20;
            }

            appleAnt.transform.Rotate(appleAnt.transform.rotation * Vector3.up, angle);
            appleAnts[i] = new KeyValuePair<GameObject, float>(appleAnt, angle);
        }
	}
    

    /**
     Instanciate insect on random mesh vertex */ 
    void PlaceAntOnMesh(GameObject ant)
    {        
        Mesh mesh = transform.GetChild(0).GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        int randIdx = Random.Range(0, vertices.Length);

        Vector3 centerToPoint = vertices[randIdx] - transform.GetChild(0).GetComponent<Renderer>().bounds.center;
        ant.transform.rotation = Quaternion.FromToRotation(Vector3.up, centerToPoint);
        ant.transform.position = transform.TransformPoint(vertices[randIdx]);
    }

    /**
     Players rewarded with 20 seconds sanity for putting apples in the toilet */
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("enter trigger :  " + other.name);
        if (other.gameObject.tag == GameConstants.TAG_TOILET && !desposedOf)
        {
            Debug.Log("Apple's in the toilet");
            houseController.madness -= 20;
            desposedOf = true;
        }
        
                int beforeCheck = 0;
                foreach (bool trigg in interactionTrigggers)
                {
                    if (trigg) beforeCheck++;
                }
                switch(other.gameObject.name)
                {
                    case "Controller (left)":
                        interactionTrigggers[0] = true;
                        break;
                    case "Controller (right)":
                        interactionTrigggers[1] = true;
                        break;
                }
                int afterCheck = 0;
                foreach (bool trigg in interactionTrigggers)
                {
                    if (trigg) afterCheck++;
                }

                if(beforeCheck == 0 && afterCheck != 0 && !toilet.GetComponent<AudioSource>().isPlaying)
                {
                    toilet.GetComponent<AudioSource>().Play();
                }                
    }

    /**
    Players rewarded with 20 seconds sanity for putting apples in the toilet */
    void OnTriggerExit(Collider other)
    {
        Debug.Log("enter exit:  " + other.name);
        int beforeCheck = 0;
        foreach (bool trigg in interactionTrigggers)
        {
            if (trigg) beforeCheck++;
        }
        
        switch (other.gameObject.name)
        {
            case "Controller (left)":
                interactionTrigggers[0] = false;
            break;
            case "Controller (right)":
                interactionTrigggers[1] = false;
            break;
        }
        int afterCheck = 0;
        foreach (bool trigg in interactionTrigggers)
        {
            if (trigg) afterCheck++;
        }

        if (beforeCheck != 0 && afterCheck == 0)
        {
            toilet.GetComponent<AudioSource>().Stop();
        }
    }
}
