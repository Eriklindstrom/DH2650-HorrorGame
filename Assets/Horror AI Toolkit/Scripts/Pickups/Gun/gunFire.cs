using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gunFire : MonoBehaviour
{
    Player player;

    // Use this for initialization
    void Start ()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (GameController.sharedGameController.inputController.TestKeyDelay(KeyCode.Mouse0))
        {
            if(player.CheckAmmoInMag())
            {
                player.FireBullet();
            }
        }
        else if (GameController.sharedGameController.inputController.TestKeyDelay(KeyCode.R))
        {
            if (player.CheckAmmoTotal())
            {
                player.ReloadMag();
            }
        }

    }
}
