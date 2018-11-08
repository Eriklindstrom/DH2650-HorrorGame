﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseController : MonoBehaviour {

    /******************\
        Configurables
    \******************/

    [SerializeField] float madness;
    [SerializeField] float madnessThreshold = 10;
    [SerializeField] GameObject flamePrefab;

    /**********************\
        Non-configurable
    \**********************/

    [HideInInspector] public float madnessPercentage = 0.0f;

    private KeyValuePair<Light, float>[] electricLights;
    private KeyValuePair<GameObject, float>[] candles;
    private GameObject[] candleFlames;

    /***************\
        Functions
    \***************/

    void Start ()
    {
        //Madness is zero to begin with        
        madness = 0.0f;

        //Initialize array of lights, also store their initial intensities        
        GameObject[] lightObjects = GameObject.FindGameObjectsWithTag(GameConstants.TAG_LIGHTS);
        electricLights = new KeyValuePair<Light, float>[lightObjects.Length];
        for (int i = 0; i < lightObjects.Length; i++)
        {
            Light currentLight = lightObjects[i].GetComponent<Light>();
            electricLights[i] = new KeyValuePair<Light, float>(currentLight, currentLight.intensity);
        }

        //Initialize array of candles along with time since last reconfiguration
        GameObject[] candleObjects = GameObject.FindGameObjectsWithTag(GameConstants.TAG_CANDLES);
        candles = new KeyValuePair<GameObject, float>[candleObjects.Length];
        candleFlames = new GameObject[candleObjects.Length];
        for(int i = 0; i < candleObjects.Length; i++)
        {            
            GameObject newFlame = Instantiate(flamePrefab, candleObjects[i].transform, false);
            newFlame.transform.localPosition = Vector3.zero;
            newFlame.transform.localPosition += new Vector3(0, candleObjects[i].GetComponent<MeshFilter>().mesh.bounds.max.y, 0);
            newFlame.SetActive(false);

            candles[i] = new KeyValuePair<GameObject, float>(candleObjects[i], Time.fixedTime);
            candleFlames[i] = newFlame;
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
        FlickerCandles();
        
        //TODO add sounds       
    }

    //Lights get gradully dimmer with increasing madness level
    void DimLights()
    {
        for(int i = 0; i < electricLights.Length; i++)
        {
            Light currentLight = electricLights[i].Key;
            float initialIntensity = electricLights[i].Value;
            currentLight.intensity = initialIntensity - (initialIntensity * madnessPercentage);
        }
    }

    //Candles randomly flicker with increasing madness level 
    void FlickerCandles()
    {
        //Turn any lit candles off
        if (madnessPercentage < 0.3)
        {
            for (int i = 0; i < candles.Length; i++)
            {
                GameObject currentCandle = candles[i].Key;
                float timeInactive = candles[i].Value;
                if (Time.fixedTime - timeInactive > 30)
                {
                    candleFlames[i].SetActive(false);
                    candles[i] = new KeyValuePair<GameObject, float>(currentCandle, Time.fixedTime);
                }
            }
        }

        //A steady normal flame
        else if (madnessPercentage < 0.6)
        {
            for (int i = 0; i < candles.Length; i++)
            {
                GameObject currentCandle = candles[i].Key;
                float timeInactive = candles[i].Value;
                if (Time.fixedTime - timeInactive > 30)
                {
                    candleFlames[i].SetActive(true);
                    candles[i] = new KeyValuePair<GameObject, float>(currentCandle, Time.fixedTime);
                }
            }
        }

        //Flickering blue flame
        else if (madnessPercentage < 0.8)
        {
            for (int i = 0; i < candles.Length; i++)
            {
                GameObject currentCandle = candles[i].Key;
                float timeInactive = candles[i].Value;
                
                if (Time.fixedTime - timeInactive > 2)
                {
                    candles[i] = new KeyValuePair<GameObject, float>(currentCandle, Time.fixedTime);

                    float reject = Random.Range(0, 10);
                    if (reject < 3)
                    {
                        continue;
                    }

                    //Candles go crazy 
                    candleFlames[i].SetActive(!candleFlames[i].activeSelf);                    
                    candleFlames[i].GetComponent<ParticleSystem>().startColor = new Color(0.1f, 0.1f, 0.7f);
                }
            }
        }

        //Solid red flame
        else
        {
            for (int i = 0; i < candles.Length; i++)
            {
                candleFlames[i].SetActive(true);
                candleFlames[i].GetComponent<ParticleSystem>().startColor = new Color(0.8f, 0.1f, 0.1f);
            }
        }
    }
}
