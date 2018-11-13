using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppleController : MonoBehaviour {

    [SerializeField] private GameObject antPrefab;
    [SerializeField] private GameObject houseControllerObj;

    private HouseController houseController;
    private float numberOfAnts;
    private List<GameObject> appleAnts;

    void Start()
    {
        houseController = houseControllerObj.GetComponent<HouseController>();
        appleAnts = new List<GameObject>();
    }
	
	void Update ()
    {
		if(numberOfAnts < houseController.madnessPercentage * 25)
        {
            GameObject ant = Instantiate(antPrefab);
            ant.transform.localScale = new Vector3(5e-06f, 5e-06f, 5e-06f);
            ant.transform.SetParent(transform, false);
            PlaceAntOnMesh(ant);
            numberOfAnts++;
            appleAnts.Add(ant);
        }
        
        foreach(GameObject appleAnt in appleAnts)
        {
            Vector3 oldPos = appleAnt.transform.position;
            Vector3 newPos = oldPos + appleAnt.transform.rotation * Vector3.right * 0.01f * Time.deltaTime;
            
            Vector3 v1 = oldPos - transform.GetChild(0).GetComponent<Renderer>().bounds.center;
            Vector3 v2 = newPos - transform.GetChild(0).GetComponent<Renderer>().bounds.center;

            Vector3 v3 = Vector3.ClampMagnitude(v2, v1.magnitude);
            Vector3 destination = transform.GetChild(0).GetComponent<Renderer>().bounds.center + v3;

            appleAnt.transform.position = newPos;
            appleAnt.transform.rotation = Quaternion.FromToRotation(Vector3.up, v2);
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
}
