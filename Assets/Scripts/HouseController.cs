using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseController : MonoBehaviour {

    /******************\
        Configurables
    \******************/

    [SerializeField] float madness;
    [SerializeField] float madnessThreshold = 10;

    /**********************\
        Non-configurable
    \**********************/

    [HideInInspector] public float madnessPercentage = 0.0f;
    private KeyValuePair<Light, float>[] electricLights;

    /***************\
        Functions
    \***************/

    void Start ()
    {
        //Madness is zero to begin with        
        madness = 0.0f;

        //Initialize list of lights, also store their initial intensities        
        GameObject[] lightObjects = GameObject.FindGameObjectsWithTag(GameConstants.TAG_LIGHTS);
        electricLights = new KeyValuePair<Light, float>[lightObjects.Length];
        for (int i = 0; i < lightObjects.Length; i++)
        {
            Light currentLight = lightObjects[i].GetComponent<Light>();
            electricLights[i] = new KeyValuePair<Light, float>(currentLight, currentLight.intensity);
        }
    }
	
	void Update ()
    {
        madness += Time.deltaTime;
        madnessPercentage = madness / madnessThreshold;

        Horrify();
	}

    void Horrify()
    {
        DimLights();
        
        //TODO add sounds       
    }

    void DimLights()
    {
        for(int i = 0; i < electricLights.Length; i++)
        {
            Light currentLight = electricLights[i].Key;
            float initialIntensity = electricLights[i].Value;
            currentLight.intensity = initialIntensity - (initialIntensity * madnessPercentage);
        }
    }

    void FlickerCandles()
    {

    }
}
