using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class healthPickup : MonoBehaviour {

    public int healthGain = 50;
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
        player.GetComponent<Player>().GainHealth(healthGain);
        gameObject.SetActive(false);
    }
}
