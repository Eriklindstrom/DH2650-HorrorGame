using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class torchPickup : MonoBehaviour
{
    public float powerGain = 50.0f;
    RawImage iconImage;

    public bool displayToolTip = false;

    void Start()
    {
        iconImage = transform.GetChild(0).transform.GetChild(0).GetComponent<RawImage>();
        iconImage.enabled = false;
    }

    void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Player" && !other.isTrigger)
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
        if (!player.GetComponent<Player>().hasTorch)
        {
            player.GetComponent<Player>().hasTorch = true;
        }

        player.GetComponent<Player>().GainTorchPower(powerGain);
        player.GetComponent<Player>().TurnOnTorch();
        gameObject.SetActive(false);
    }
}
