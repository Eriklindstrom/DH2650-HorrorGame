using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BathroomController : MonoBehaviour {

    [SerializeField] GameObject houseControllerObj;
    [SerializeField] GameObject showerControllerObj;
    [SerializeField] GameObject brain;
    [SerializeField] GameObject cutlery;

    private HouseController houseController;
    private VRTK.Examples.ShowerHandler showerHandler;

    private bool showerStarted;
    private bool bloodInShower;
    private bool bloodOnFloor;
    private bool brainActive;
    private bool cutleryActive;

    void Start ()
    {
        houseController = houseControllerObj.GetComponent<HouseController>();
        showerHandler = showerControllerObj.GetComponent<VRTK.Examples.ShowerHandler>();

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
        BloodSplatter();
        MainCourse();
    }

    void MainCourse()
    {
        if(houseController.madnessPercentage > 0.5 && !cutleryActive)
            cutlery.SetActive(true);

        if(houseController.madnessPercentage > 0.8 && !brainActive)
            brain.SetActive(true);
    }

    void BloodSplatter()
    {
        if(houseController.madnessPercentage > 0.85 && !bloodOnFloor)
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
        if (houseController.madnessPercentage > 0.62f && !showerStarted)
        {
            showerHandler.RemoteStart();
            showerStarted = true;
        }

        if (houseController.madnessPercentage > 0.85f && !bloodInShower)
        {
            Color myColor = new Color();
            ColorUtility.TryParseHtmlString("#330000", out myColor);
            showerHandler.SetColor(myColor);
            bloodInShower = true;
        }
    }
}
