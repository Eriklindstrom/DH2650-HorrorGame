using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseController : MonoBehaviour {

    /******************\
        Configurables
    \******************/

    public float madness;
    public AudioClip[] themeSongs;
    [SerializeField] float madnessThreshold = 10;
    [SerializeField] GameObject flamePrefab;

    /**********************\
        Non-configurable
    \**********************/
    
    [HideInInspector] public float madnessPercentage = 0.0f;
    [HideInInspector] public bool lightsOut = false;

    private KeyValuePair<GameObject, float>[] electricLights;
    private KeyValuePair<GameObject, float>[] candles;
    private GameObject[] candleFlames;
    private AudioSource[] themePlayer;
    private bool[] themePlaying;
    private float[] percentageSwitch;
    private int toggleMusic = 1;
    private double musicStarted;
    private double musicDuration;

    private float flickerTimer = 0.0f;

    /***************\
        Functions
    \***************/

    void Start ()
    {
        //Madness is zero to begin with        
        madness = 0.0f;

        //Initialize array of lights, also store their initial intensities        
        GameObject[] lightObjects = GameObject.FindGameObjectsWithTag(GameConstants.TAG_LIGHTS);
        electricLights = new KeyValuePair<GameObject, float>[lightObjects.Length];
        for (int i = 0; i < lightObjects.Length; i++)
        {
            Light currentLight = lightObjects[i].GetComponent<Light>();
            electricLights[i] = new KeyValuePair<GameObject, float>(lightObjects[i], currentLight.intensity);
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

        //Sound player for theme
        themePlayer = GameObject.FindGameObjectsWithTag(GameConstants.TAG_THEME_PLAYER)[0].GetComponents<AudioSource>();
        themePlaying = new bool[themeSongs.Length];
        for (int i = 0; i < themeSongs.Length; i++)
        {
            themePlaying[i] = false;
        }
        percentageSwitch = new float[themeSongs.Length];
        for (int i = 0; i < themeSongs.Length; i++)
        {
            percentageSwitch[i] = 1.0f / themeSongs.Length * i;
        }
        themePlayer[toggleMusic].clip = themeSongs[0];
        musicStarted = AudioSettings.dspTime;
        themePlayer[toggleMusic].PlayScheduled(AudioSettings.dspTime);
        musicDuration = (double)themeSongs[0].samples / themeSongs[0].frequency;
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
        PlayTheme();
    }

    void PlayTheme()
    {
        for (int i = themeSongs.Length-1; i > -1; i--)
        {
            //print(madnessPercentage);
            if (madnessPercentage >= 1f|| (madnessPercentage > percentageSwitch[i])) // Should play this song!
            {
                //if (themePlaying[i]) break;
                for (int p = 0; p < themePlaying.Length; p++) { themePlaying[p] = false; }
                //print(i);
                themePlaying[i] = true;
                if (musicStarted + musicDuration - AudioSettings.dspTime < 1) //Que up next song
                {
                    print("Next song queued");
                    toggleMusic = 1 - toggleMusic;
                    themePlayer[toggleMusic].clip = themeSongs[i];
                    themePlayer[toggleMusic].PlayScheduled(musicStarted + musicDuration);
                    musicStarted = musicStarted + musicDuration;
                    musicDuration = (double)themeSongs[i].samples / themeSongs[i].frequency;
                }
                /*
                if (themePlayer[toggleMusic].isPlaying)
                {
                    themePlaying[i] = false;
                    break;
                }*/
                
            }
        }
    }

    //Lights get gradully dimmer with increasing madness level
    void DimLights()
    {
        for(int i = 0; i < electricLights.Length; i++)
        {
            Light currentLight = electricLights[i].Key.GetComponent<Light>();
            float initialIntensity = electricLights[i].Value;
            currentLight.intensity = initialIntensity - (initialIntensity * madnessPercentage) / 1.2f;
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

    /**
     Lighst flicker out for the provided number of seconds */
    public IEnumerator LightsOut(float seconds)
    {
        float nextFlicker = seconds - (seconds / 3);
        lightsOut = true;
        flickerTimer = 0.0f;
        

        //Disable lights
        foreach (var currentLight in electricLights)
        {
            currentLight.Key.SetActive(false);
        }        

        //Wait
        while (flickerTimer <= seconds && lightsOut)
        {
            flickerTimer += Time.deltaTime;

            if(flickerTimer >= nextFlicker)
            {
                Debug.Log("Flickering");
                foreach (var currentLight in electricLights)
                {
                    currentLight.Key.SetActive(!currentLight.Key.activeSelf);
                }
                nextFlicker += ((seconds - flickerTimer) / 3);
            }            

            yield return null;
        }
        
        //Re-enable lights
        foreach (var currentLight in electricLights)
        {
            currentLight.Key.SetActive(true);            
        }
        
        lightsOut = false;
        yield break;
    }
}
