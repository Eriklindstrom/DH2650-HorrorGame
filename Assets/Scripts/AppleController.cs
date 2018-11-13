using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppleController : MonoBehaviour {

    [SerializeField] private GameObject antPrefab;
    [SerializeField] private GameObject houseControllerObj;

    private HouseController houseController;
    private float numberOfAnts; 

    void Start()
    {
        houseController = houseControllerObj.GetComponent<HouseController>();
    }
	
	void Update ()
    {
		if(numberOfAnts < houseController.madnessPercentage * 10)
        {
            GameObject ant = Instantiate(antPrefab);
            ant.transform.localScale = new Vector3(1e-05f, 1e-05f, 1e-05f);
            ant.transform.SetParent(transform, false);
            PlaceAntOnMesh(ant);
            numberOfAnts++;
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
        ant.transform.position = transform.TransformPoint(vertices[q]);
        
        ant.transform.rotation = Quaternion.FromToRotation(Vector3.up, vertices[q]);
        //ant.transform.position = hit.point;        
    }
}
