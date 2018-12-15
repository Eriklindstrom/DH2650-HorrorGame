using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour {

    [SerializeField] private GameObject gameCamera;
    [SerializeField] private float delay;
    [SerializeField] private List<GameObject> blood;

    private float timeCounter;

    void Update()
    {
        RaycastHit hit;
        Debug.DrawLine(gameCamera.transform.position, gameCamera.transform.forward * 5000, Color.green);
        if (Physics.Raycast(gameCamera.transform.position, gameCamera.transform.forward, out hit, 5000))
        {
            Debug.Log("HIT");
            GameObject objectHit = hit.transform.gameObject;
            if (objectHit.CompareTag("PlayButton")) //Same counter is for all buttons right now...
            {
                objectHit.GetComponent<Button>().Select();
                timeCounter += Time.deltaTime;
                if (timeCounter >= delay)
                {
                    objectHit.GetComponent<Button>().onClick.Invoke();
                }

                foreach (GameObject splatter in blood)
                    splatter.SetActive(true);
            }
            else
            {
                timeCounter = 0;
                foreach (GameObject splatter in blood)
                    splatter.SetActive(false);
            }
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("VRScene");
    }
}
