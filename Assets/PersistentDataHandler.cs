using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentDataHandler : MonoBehaviour {

    [SerializeField] private GameObject HouseController;
    [SerializeField] private GameObject Player;
    //public float madness = HouseController.GetComponent().Madness;
    //public float MadnessLevel;
    private float x, y, z;
    
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        //MadnessLevel = GetComponent<HouseController>().madnessPercentage;
    }
    void Update()
    {
        //Debug.Log(GetComponent<HouseController>().madnessPercentage);
        SaveData();
    }
        
    void SaveData()
    {
        PlayerPrefs.SetFloat("Madness", GetComponent<HouseController>().madnessPercentage);
        /*PlayerPrefs.SetFloat("xc", Player.translate.transform.x);
        PlayerPrefs.SetFloat("yc", Player.translate.transform.y);
        PlayerPrefs.SetFloat("zc", Player.translate.transform.z);*/
        x = PlayerPrefs.GetFloat("x");
        y = PlayerPrefs.GetFloat("y");
        z = PlayerPrefs.GetFloat("z");
        Vector3 posVec = new Vector3(x, y, z);
        Player.transform.position = posVec;
    }


    void LoadData()
    {
        PlayerPrefs.GetFloat("Madness", 0); //Default 0 if madness is null
        posVec = new Vector3(PlayerPrefs.GetFloat("c"), PlayerPrefs.GetFloat("y"), PlayerPrefs.GetFloat("z"));
    }
}
