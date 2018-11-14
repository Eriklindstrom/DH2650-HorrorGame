using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppleController : MonoBehaviour {

    [SerializeField] private GameObject antPrefab;
    [SerializeField] private GameObject houseControllerObj;

    private HouseController houseController;
    private float numberOfAnts;
    private List<KeyValuePair<GameObject, float>> appleAnts;

    void Start()
    {
        houseController = houseControllerObj.GetComponent<HouseController>();
        appleAnts = new List<KeyValuePair<GameObject, float>>();
    }
	
	void Update ()
    {
		if(numberOfAnts < houseController.madnessPercentage * 25)
        {
            GameObject ant = Instantiate(antPrefab);
            //ant.transform.localScale = new Vector3(5e-06f, 5e-06f, 5e-06f);
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
            //Vector3 destination = transform.GetChild(0).GetComponent<Renderer>().bounds.center + v3;
            Vector3 destination = transform.GetChild(0).GetComponent<Renderer>().bounds.center + v3;

            appleAnt.transform.position = destination;
            appleAnt.transform.rotation = Quaternion.FromToRotation(Vector3.up, v3);

            float rand = Random.Range(0.0f, 100.0f);
            if (rand > 99.0)
            {
                angle += 20;
                //Debug.Log(appleAnt.transform.rotation);                
                //appleAnt.transform.LookAt(appleAnt.transform.right);
                //appleAnt.transform.Rotate(appleAnt.transform.up, 90f);
            }
            appleAnt.transform.Rotate(appleAnt.transform.rotation * Vector3.up, angle);
            appleAnts[i] = new KeyValuePair<GameObject, float>(appleAnt, angle);
        }

	}
    

    void PlaceAntOnMesh(GameObject ant)
    {        
        Vector3 direction = Random.onUnitSphere;
        float angle = Vector3.Angle(Vector3.up, direction);
        float length = 2.0f;

        RaycastHit hit;
        Ray ray = new Ray(transform.position + direction * length, -direction);        
        Physics.Raycast(ray, out hit);

        Mesh mesh = transform.GetChild(0).GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;

        var q = Random.Range(0, vertices.Length);

        Vector3 centerToPoint = vertices[q] - transform.GetChild(0).GetComponent<Renderer>().bounds.center;
        ant.transform.rotation = Quaternion.FromToRotation(Vector3.up, centerToPoint);
        ant.transform.position = transform.TransformPoint(vertices[q]);
        
        //ant.transform.position = hit.point;        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "[VRTK][AUTOGEN][HeadsetColliderContainer]")
            Debug.Log("collision");
    }
}
