using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScaryPartStarts : MonoBehaviour {

    [SerializeField] private GameObject Mannequin1;
    [SerializeField] private GameObject Mannequin2;

    // Use this for initialization
    void Start () {
		
	}

    void OnTriggerEnter(Collider col)
    {
        Mannequin1.SetActive(true);
        Mannequin2.SetActive(true);
        StartCoroutine(waitForSec(4.0f));        
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private IEnumerator waitForSec(float sec)
    {
        /*float timer = 0.0f;
        while(timer < sec)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        */
        yield return new WaitForSeconds(sec);
        SceneManager.LoadScene(1);
        //yield break;
    }
}
