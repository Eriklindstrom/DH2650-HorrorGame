using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class gunPickup : MonoBehaviour {

    public int ammoGain = 10;
    RawImage iconImage;
    public bool displayToolTip = false;

    void Start()
    {

        iconImage = transform.GetChild(0).transform.GetChild(0).GetComponent<RawImage>();
        iconImage.enabled = false;
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player" && !other.isTrigger)
        {
            if (displayToolTip)
            {
                iconImage.enabled = true;
            }
            if (GameController.sharedGameController.inputController.TestKeyDelay(KeyCode.F))
            {
                pickUp(other.gameObject);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player" && !other.isTrigger)
        {
            if (displayToolTip)
            {
                iconImage.enabled = false;
            }
        }
    }

    void pickUp(GameObject player)
    {
        if (!player.GetComponent<Player>().hasGun)
        {
            player.GetComponent<Player>().PickUpGun();
        }

        player.GetComponent<Player>().GainAmmo(ammoGain);
        gameObject.SetActive(false);
    }
}
