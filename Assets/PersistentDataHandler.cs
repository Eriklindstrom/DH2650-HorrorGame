using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentDataHandler : MonoBehaviour {

    [SerializeField] private GameObject goUpCollider;
    [SerializeField] private GameObject HouseController;
    [SerializeField] private GameObject Player;
    public float madness;
    public Vector3 posVec;
    
    private float x, y, z;
    
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        madness = GetComponent<HouseController>().madness;
    }

    void position_load()
    {
        gameObject.GetComponent<PersistentDataHandler>().LoadData();
        SceneManager.LoadScene(0);
    }


    void OnTriggerEnter(Collider ChangeScene)
    {
        //anim.Play();        //Not set up
        position_load();
        //SceneManager.LoadScene(0);
    }

    void Update()
    {
        madness = GetComponent<HouseController>().madness;
        SaveData();
        posVec.x = Player.transform.position.x;
        posVec.y = Player.transform.position.y;
        posVec.z = Player.transform.position.z;
        //Vector3 posVec = new Vector3(Player.transform.position.x, Player.transform.position.y, Player.transform.position.z);

    }
        
    void SaveData()
    {
        PlayerPrefs.SetFloat("Madness", madness);
        PlayerPrefs.SetFloat("PlayerX", Player.transform.position.x);
        PlayerPrefs.SetFloat("PlayerY", Player.transform.position.y);
        PlayerPrefs.SetFloat("PlayerZ", Player.transform.position.z);
        //Debug.Log(Player.transform.position.x);
    }


    public void LoadData()
    {
        Debug.Log(PlayerPrefs.GetFloat("Madness"));
        PlayerPrefs.GetFloat("Madness"); //Default 0 if madness is null
        //posVec = new Vector3(PlayerPrefs.GetFloat("c"), PlayerPrefs.GetFloat("y"), PlayerPrefs.GetFloat("z"));
        Player.transform.position = new Vector3(PlayerPrefs.GetFloat("PlayerX"), PlayerPrefs.GetFloat("PlayerY"), PlayerPrefs.GetFloat("PlayerZ"));

    }
}
