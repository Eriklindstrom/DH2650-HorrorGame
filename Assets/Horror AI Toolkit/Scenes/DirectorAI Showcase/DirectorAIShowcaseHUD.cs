using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DirectorAIShowcaseHUD : MonoBehaviour {

    Text CurrentStressTextBox;
    Text CurrentNoAITextBox;
    AIdirector director;

    // Use this for initialization
    void Start ()
    {
        CurrentStressTextBox = transform.GetChild(3).GetComponent<Text>();
        CurrentNoAITextBox = transform.GetChild(4).GetComponent<Text>();
        director = GameObject.FindGameObjectWithTag("AIdirector").GetComponent<AIdirector>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        CurrentStressTextBox.text = Mathf.Round(director.currentStressLevel).ToString();
        CurrentNoAITextBox.text = director.currentMaxNumberOfEnemySpawns.ToString();
	}
}
