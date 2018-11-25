using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BathroomController : MonoBehaviour {

    [SerializeField] GameObject houseControllerObj;
    [SerializeField] GameObject showerControllerObj;

    private HouseController houseController;
    private VRTK.Examples.ShowerHandler showerHandler;

    private bool showerStarted;
    private bool bloodInShower;

    void Start ()
    {
        houseController = houseControllerObj.GetComponent<HouseController>();
        showerHandler = showerControllerObj.GetComponent<VRTK.Examples.ShowerHandler>();

        showerStarted = false;
        bloodInShower = false;
	}
	
	void Update ()
    {
        Horrify();
	}

    void Horrify()
    {
        ShowerEffects();
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
            ColorUtility.TryParseHtmlString("#990000", out myColor);
            showerHandler.SetColor(myColor);
            bloodInShower = true;
        }
    }
}
