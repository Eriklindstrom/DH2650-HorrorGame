using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BathroomController : MonoBehaviour {

    [SerializeField] private GameObject houseControllerObj;
    [SerializeField] private GameObject showerControllerObj;
    [SerializeField] private GameObject brain;
    [SerializeField] private GameObject cutlery;
    [SerializeField] private GameObject door;
    [SerializeField] private AudioSource ticTocClip;
    [SerializeField] private AudioSource growlClip;
    [SerializeField] private AudioSource doorSlamClip;

    private HouseController houseController;
    private VRTK.Examples.Openable_Door openableDoor;
    private VRTK.Examples.ShowerHandler showerHandler;

    private bool showerStarted;
    private bool bloodInShower;
    private bool bloodOnFloor;
    private bool brainActive;
    private bool cutleryActive;
    private float horrorTimer;

    void Start ()
    {
        houseController = houseControllerObj.GetComponent<HouseController>();
        showerHandler = showerControllerObj.GetComponent<VRTK.Examples.ShowerHandler>();
        openableDoor = door.GetComponent<VRTK.Examples.Openable_Door>();

        doorSlamClip.clip = MakeSubclip(doorSlamClip.clip, 1.0f, 2.0f);
        growlClip.clip = MakeSubclip(growlClip.clip, 0.0f, 2.0f);

        showerStarted = false;
        bloodInShower = false;
        brainActive = false;
        cutleryActive = false;
	}
	
	void Update ()
    {
        Horrify();
	}

    void Horrify()
    {
        ShowerEffects();
        MainCourse();
    }

    void MainCourse()
    {
        if(houseController.madnessPercentage > 0.5 && !cutleryActive)
            cutlery.SetActive(true);

        if(houseController.madnessPercentage > 0.8 && !brainActive)
            brain.SetActive(true);
    }

    public void BloodSplatter()
    {
        if(!bloodOnFloor)
        {
            StartCoroutine(houseController.LightsOut(0.3f));

            GameObject bloodParent = GameObject.FindWithTag(GameConstants.TAG_BATHROOM_BLOOD);            
            foreach (Transform splatter in bloodParent.transform)
            {
                splatter.gameObject.SetActive(true);
            }
            bloodOnFloor = true;
        }
    }

    void ShowerEffects()
    {
        if (houseController.madnessPercentage > 0.62f)
            showerHandler.RemoteStart();
    }

    void SlamDoor()
    {
        openableDoor.RemoteSlam(showerControllerObj.transform.position);
    }


    public void TriggerHorror()
    {
        StartCoroutine("TriggerHorrorRoutine");
    }
    
    private IEnumerator TriggerHorrorRoutine()
    {
        //Wait, then slam door
        ticTocClip.Play();
        while (horrorTimer <= 1)
        {
            horrorTimer += Time.deltaTime;
            yield return null;
        }
        SlamDoor();
        ticTocClip.Stop();
        doorSlamClip.Play();

        //Wait, all seems quited, then trigger horror
        while (horrorTimer <= 4)
        {
            horrorTimer += Time.deltaTime;
            yield return null;
        }

        houseController.LightsOut(0.5f);
        BloodSplatter();
        Color bloodColor = new Color();
        ColorUtility.TryParseHtmlString("#330000", out bloodColor);
        showerHandler.SetColor(bloodColor);
        growlClip.Play();

        yield break;
    }

    private AudioClip MakeSubclip(AudioClip clip, float start, float stop)
    {
        /* Create a new audio clip */
        int frequency = clip.frequency;
        float timeLength = stop - start;
        int samplesLength = (int)(frequency * timeLength);
        AudioClip newClip = AudioClip.Create(clip.name + "-sub", samplesLength, 1, frequency, false);
        
        /* Create a temporary buffer for the samples */
        float[] data = new float[samplesLength];
        
        /* Get the data from the original clip */
        clip.GetData(data, (int)(frequency * start));
        
        /* Transfer the data to the new clip */
        newClip.SetData(data, 0);
        
        /* Return the sub clip */
        return newClip;
    }
}
