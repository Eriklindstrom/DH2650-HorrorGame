using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Player : MonoBehaviour
{
    [HideInInspector]
    public UnityStandardAssets.Characters.FirstPerson.FirstPersonController firstPersonController;

    [HideInInspector]
    public soundEmitter sound;
    [HideInInspector]
    public bool isHiding;
    [HideInInspector]
    public GameObject objectHiddenIn;
    Vector3 OldPosition;
    Vector3 OldCameraPositon;
    Quaternion OldCameraRotation;

    [HideInInspector]
    public bool isUsingDoor;
    [HideInInspector]
    public GameObject doorBeingUsed;

    [HideInInspector]
    public Camera playerCamera;

    public delegate void EnterHideInAction();
    public static event EnterHideInAction EnterHideInBroadcast;

    [HideInInspector]
    public bool hasTorch = true;
    [HideInInspector]
    public bool torchOnStatus = false;

    [Tooltip("Assign this as the torch object which your player character will use.")]
    public GameObject torchObject;

    [Tooltip("The remaining power in the player's torch.")]
    public float torchRemainingPower = 0.0f;

    [Tooltip("The rate at which the player's torch drains power while on.")]
    public float torchPowerDrain = 1.0f;

    float torchFlickerTimer = 0.0f;
    float torchFlickerDuration = 1.0f;
    float torchFlickerModifier = 0.0f;
    float torchTimeBetweenFlickerTimer = 0.0f;
    float torchTimeBetweenFlickerDuration = 2.0f;
    [HideInInspector]
    public bool torchIsFlickering = false;

    [Tooltip("The audio clip which plays when the player turns their torch on or off.")]
    public AudioClip torchClickOnOff;

    [Tooltip("The audio clip which plays when the player takes damage.")]
    public AudioClip takeDamageSound;
    AudioSource playerAudioSource;

    [Tooltip("The audio clips which plays when the player picks up a gun.")]
    public AudioClip pickupGunSound;
    [Tooltip("The audio clips which plays when the player picks up a health kit.")]
    public AudioClip pickupHealthSound;
    [Tooltip("The audio clips which plays when the player picks up a torch.")]
    public AudioClip pickupTorchSound;

    [HideInInspector]
    public bool hasGun = false;

    [Tooltip("Assign this as the gun object which the player character will use.")]
    public GameObject gunObject;

    [HideInInspector]
    public int ammoCountTotal = 0;
    [HideInInspector]
    public int ammoCountMag = 0;
    [Tooltip("The max amount of bullets the player can store in one mag before reloading.")]
    public int ammoMagSize = 6;
    [Tooltip("The max range at which the player's gun can fire.")]
    public float gunMaxRange = 100.0f;
    [Tooltip("The damage the player will inflict when shooting at enemies.")]
    public float gunDamage = 50.0f;

    [Tooltip("The player's max health. When using a health kit their current health will not pass this value.")]
    public float healthMax = 100.0f;

    [Tooltip("The player's current health. At zero or lower they will die.")]
    public float healthCurrent = 0.0f;

    float normalWalkSpeed = 0.0f;
    float normalRunSpeed = 0.0f;
    float hurtWalkSpeed = 0.0f;
    float hurtRunSpeed = 0.0f;
    float normalHeadbob = 0.0f;
    float hurtHeadbob = 0.0f;

    GameObject toggleTorchHUDTip;
    bool hasDisplayedToggleTorchTip = false;
    GameObject reloadToolTip;
    bool hasDisplayedReloadToolTip = false;
    GameObject gameOverHUD;
    [Tooltip("Whether or not the player can die, causing the game over screen to appear.")]
    public bool playerCanDie = true;

    [Tooltip("The amount of sound the player will make when walking past the minWalkSoundThreshold. This sets the size of the player's sount emitter, and effects at what point the AI can hear the player walking. Use this for slow walking.")]
    public float minWalkSoundAmount = 2.0f;
    [Tooltip("The speed the player must be moving over in order to use the minWalkSoundAmount.")]
    public float minWalkSoundThreshold = 1.0f;
    [Tooltip("The amount of sound the player will make when walking past the midWalkSoundThreshold. This sets the size of the player's sount emitter, and effects at what point the AI can hear the player walking. Use this for medium pace walking.")]
    public float midWalkSoundAmount = 8.0f;
    [Tooltip("The speed the player must be moving over in order to use the midWalkSoundAmount.")]
    public float midWalkSoundThreshold = 3.0f;
    [Tooltip("The amount of sound the player will make when walking past the maxWalkSoundThreshold. This sets the size of the player's sount emitter, and effects at what point the AI can hear the player walking. Use this for sprinting.")]
    public float maxWalkSoundAmount = 12.0f;
    [Tooltip("The speed the player must be moving over in order to use the maxWalkSoundAmount.")]
    public float maxWalkSoundThreshold = 5.0f;

    // Use this for initialization
    void Start()
    {
        Time.timeScale = 1;
        firstPersonController = gameObject.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>();

        sound = GetComponent<soundEmitter>();
        isHiding = false;
        isUsingDoor = false;

        playerCamera = gameObject.transform.GetChild(0).GetComponent<Camera>();

        Random.InitState((int)System.DateTime.Now.Ticks);

        playerAudioSource = GetComponent<AudioSource>();   

        if (!hasTorch || (hasTorch && !torchOnStatus))
        {
            TurnOffTorch();
        }

        if(!hasGun && gunObject != null)
        {
            gunObject.SetActive(false);
        }

        healthCurrent = healthMax;
        normalRunSpeed = firstPersonController.m_RunSpeed;
        normalWalkSpeed = firstPersonController.m_WalkSpeed;
        hurtRunSpeed = normalRunSpeed / 1.2f;
        hurtWalkSpeed = normalWalkSpeed / 1.2f;

        normalHeadbob = firstPersonController.m_HeadBob.VerticalBobRange;
        hurtHeadbob = normalHeadbob + 0.03f;

        toggleTorchHUDTip = GameObject.Find("HUDCanvas").transform.GetChild(5).gameObject;
        reloadToolTip = GameObject.Find("HUDCanvas").transform.GetChild(6).gameObject;
        gameOverHUD = GameObject.Find("HUDCanvas").transform.GetChild(7).gameObject;
    }

    void FixedUpdate()
    {
        if(getCurrentSpeed() > maxWalkSoundThreshold)
        {
            sound.volume = maxWalkSoundAmount;
        }
        else if(getCurrentSpeed() > midWalkSoundThreshold)
        {
            sound.volume = midWalkSoundAmount;
        }
        else if(getCurrentSpeed() > minWalkSoundThreshold)
        {
            sound.volume = minWalkSoundAmount;
        }
        else
        {
            sound.volume = 0;
        }

        if(isHiding && GameController.sharedGameController.inputController.TestKeyDelay(KeyCode.E))
        {
            LeaveHideInObject();
        }

        if(GameController.sharedGameController.inputController.TestKeyDelay(KeyCode.T) && hasTorch && (torchObject != null))
        {
            ToggleTorch();
        }

        if(torchOnStatus == true)
        {
            torchRemainingPower -= torchPowerDrain * Time.deltaTime;

            if(torchRemainingPower <= 25)
            {
                torchIsFlickering = true;
            }
            else
            {
                torchIsFlickering = false;
            }

            if(torchIsFlickering)
            {
                FlickerTorch();
            }
            else
            {
                TurnOnTorch();
            }

            if(torchRemainingPower <= 0)
            {
                torchRemainingPower = 0.0f;
                TurnOffTorch();
            }
        }

        if(healthCurrent <= 50.0f)
        {
            firstPersonController.m_WalkSpeed = hurtWalkSpeed;
            firstPersonController.m_RunSpeed = hurtRunSpeed;
            firstPersonController.m_HeadBob.VerticalBobRange = hurtHeadbob;
        }
        else
        {
            firstPersonController.m_WalkSpeed = normalWalkSpeed;
            firstPersonController.m_RunSpeed = normalRunSpeed;
            firstPersonController.m_HeadBob.VerticalBobRange = normalHeadbob;
        }

        if((isUsingDoor || isHiding) && hasGun)
        {
            if (gunObject.activeInHierarchy)
            {
                gunObject.SetActive(false);
            }
        }
        else if((!isUsingDoor && !isHiding) && hasGun)
        {
            if(!gunObject.activeInHierarchy)
            {
                gunObject.SetActive(true);
            }
        }
    }

    //Get the current speed of the player, used for 'hearing' detection
    public float getCurrentSpeed()
    {
        Vector3 velocity;       
        velocity = firstPersonController.GetVelocity();
        float speed = velocity.magnitude;
        //Debug.Log("speed=" + speed);
        return speed;
    }

    public void StartHideInObject(HideInObject hideIn)
    {
        isHiding = true;
        objectHiddenIn = hideIn.gameObject;

        OldPosition = this.transform.position;
        OldCameraPositon = playerCamera.transform.position;
        OldCameraRotation = playerCamera.transform.rotation;

        gameObject.GetComponent<Rigidbody>().useGravity = false;
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        gameObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        firstPersonController.enabled = false;

        transform.position = new Vector3(hideIn.transform.position.x, hideIn.transform.position.y - 10f, hideIn.transform.position.z);
        playerCamera.transform.position = new Vector3(hideIn.transform.position.x, hideIn.transform.position.y, hideIn.transform.position.z);
        playerCamera.transform.rotation = hideIn.transform.rotation;

        if(EnterHideInBroadcast != null)
            EnterHideInBroadcast();
    }

    public void LeaveHideInObject()
    {
        isHiding = false;
        objectHiddenIn.GetComponent<HideInObject>().playerIsIn = false;

        gameObject.GetComponent<Rigidbody>().useGravity = true;

        firstPersonController.enabled = true;

        this.transform.rotation = objectHiddenIn.transform.rotation;
        playerCamera.transform.rotation = objectHiddenIn.transform.rotation;
        firstPersonController.resetRotation = true;

        this.transform.position = OldPosition;
        playerCamera.transform.position = OldCameraPositon;

        objectHiddenIn = null;

    }

    public void StartUsingDoor(Door doorUsing)
    {
        isUsingDoor = true;
        doorBeingUsed = doorUsing.gameObject;

        OldPosition = this.transform.position;
        OldCameraPositon = playerCamera.transform.position;
        OldCameraRotation = playerCamera.transform.rotation;

        firstPersonController.enabled = false;

        if (doorBeingUsed.GetComponent<Door>().snapPointFront.GetComponent<DoorSnapPoint>().isHit)
        {
            this.transform.position = new Vector3(doorBeingUsed.GetComponent<Door>().snapPointFront.transform.position.x, doorBeingUsed.GetComponent<Door>().snapPointFront.transform.position.y + 1.5f, doorBeingUsed.GetComponent<Door>().snapPointFront.transform.position.z); 
            this.transform.rotation = doorBeingUsed.GetComponent<Door>().snapPointFront.transform.rotation;
            playerCamera.transform.position = this.transform.position;
            playerCamera.transform.rotation = this.transform.rotation;
            firstPersonController.resetRotation = true;
        }
        else if (doorBeingUsed.GetComponent<Door>().snapPointBack.GetComponent<DoorSnapPoint>().isHit)
        {
            this.transform.position = new Vector3(doorBeingUsed.GetComponent<Door>().snapPointBack.transform.position.x, doorBeingUsed.GetComponent<Door>().snapPointBack.transform.position.y + 1.5f, doorBeingUsed.GetComponent<Door>().snapPointBack.transform.position.z);
            this.transform.rotation = doorBeingUsed.GetComponent<Door>().snapPointBack.transform.rotation;
            playerCamera.transform.position = this.transform.position;
            playerCamera.transform.rotation = this.transform.rotation;
            firstPersonController.resetRotation = true;
        }

    }

    public void StopUsingDoor()
    {
        isUsingDoor = false;
        doorBeingUsed.GetComponent<Door>().playerIsUsing = false;

        firstPersonController.enabled = true;

        if (doorBeingUsed.GetComponent<Door>().snapPointFront.GetComponent<DoorSnapPoint>().isHitPlayer)
        {
            this.transform.rotation = doorBeingUsed.GetComponent<Door>().snapPointFront.transform.rotation;
            playerCamera.transform.rotation = this.transform.rotation;
            this.transform.position = new Vector3(doorBeingUsed.GetComponent<Door>().snapPointFront.transform.position.x, doorBeingUsed.GetComponent<Door>().snapPointFront.transform.position.y + 1.5f, doorBeingUsed.GetComponent<Door>().snapPointFront.transform.position.z);
            playerCamera.transform.position = this.transform.position;
        }
        else if (doorBeingUsed.GetComponent<Door>().snapPointBack.GetComponent<DoorSnapPoint>().isHitPlayer)
        {
            this.transform.rotation = doorBeingUsed.GetComponent<Door>().snapPointBack.transform.rotation;
            playerCamera.transform.rotation = this.transform.rotation;
            this.transform.position = new Vector3(doorBeingUsed.GetComponent<Door>().snapPointBack.transform.position.x, doorBeingUsed.GetComponent<Door>().snapPointBack.transform.position.y + 1.5f, doorBeingUsed.GetComponent<Door>().snapPointBack.transform.position.z);
            playerCamera.transform.position = this.transform.position;
        }

        firstPersonController.resetRotation = true;

        //this.transform.position = OldPosition;
        //playerCamera.transform.position = OldCameraPositon;

        doorBeingUsed = null;
    }

    void ToggleTorch()
    {
        torchOnStatus = !torchOnStatus;
        torchObject.SetActive(torchOnStatus);
        toggleTorchHUDTip.SetActive(false);

        if (torchClickOnOff != null)
        {
            playerAudioSource.PlayOneShot(torchClickOnOff, 1);
        }
    }

    public void TurnOffTorch()
    {
        if (torchObject != null)
        {
            torchObject.SetActive(false);
        }
        torchOnStatus = false;
    }

    public void FlickerTorch()
    {
        torchTimeBetweenFlickerTimer += Time.deltaTime;
        if(torchTimeBetweenFlickerTimer >= (torchTimeBetweenFlickerDuration))
        {
            torchFlickerTimer += Time.deltaTime;
            if(torchFlickerTimer <= (torchFlickerDuration + torchFlickerModifier))
            {
                torchObject.SetActive(false);
            }
            else
            {

                if(torchRemainingPower <= 15)
                {
                    torchFlickerTimer = Random.Range(0.0f, 0.2f);
                    torchTimeBetweenFlickerTimer = Random.Range(0.4f, 1.0f);
                    torchFlickerModifier = Random.Range(0.5f, 1.5f);
                }
                else
                {
                    torchFlickerTimer = Random.Range(0.2f, 0.4f);
                    torchTimeBetweenFlickerTimer = Random.Range(0.0f, 0.3f);
                    torchFlickerModifier = 0.0f;
                }
            }    
        }
        else
        {
            torchObject.SetActive(true);
        }
    }

    public void TurnOnTorch()
    {
        torchObject.SetActive(true);
        torchOnStatus = true;
    }

    public void GainTorchPower(float powerAmount)
    {
        if (!hasDisplayedToggleTorchTip)
        {
            toggleTorchHUDTip.SetActive(true);
            hasDisplayedToggleTorchTip = true;
        }

        if (pickupTorchSound != null)
        {
            playerAudioSource.PlayOneShot(pickupTorchSound, 1.0f);
        }

        torchRemainingPower += powerAmount;
        if(torchRemainingPower > 100)
        {
            torchRemainingPower = 100;
        }
    }

    public void PickUpGun()
    {
        if (!hasDisplayedReloadToolTip)
        {
            reloadToolTip.SetActive(true);
        }

        gunObject.SetActive(true);
        hasGun = true;
    }

    public void GainAmmo(int gainAmount)
    {
        if(pickupGunSound != null)
        {
            playerAudioSource.PlayOneShot(pickupGunSound, 1.0f);
        }

        ammoCountTotal += gainAmount;
    }

    public void ReloadMag()
    {
        reloadToolTip.SetActive(false);
        if (ammoCountTotal >= ammoMagSize)
        {
            ammoCountTotal -= ammoMagSize;
            ammoCountMag = ammoMagSize;
        }
        else
        {
            ammoCountMag = ammoCountTotal;
            ammoCountTotal = 0;
        }
    }

    public bool CheckAmmoInMag()
    {
        if(ammoCountMag > 0)
        {
            return true;
        }
        return false;
    }

    public bool CheckAmmoTotal()
    {
        if(ammoCountTotal > 0)
        {
            return true;
        }
        return false;
    }

    public void FireBullet()
    {
        ammoCountMag -= 1;

        Vector3 rayOrigin = playerCamera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0.0f));
        RaycastHit hit;
        LayerMask ignorePlayer = ~(1 << 8);
        if (Physics.Raycast(rayOrigin, playerCamera.transform.forward, out hit, gunMaxRange, ignorePlayer))
        {
            //Debug.Log("hit " + hit.collider.gameObject.name.ToString());
            if(hit.collider.tag == "AdvancedAI" || hit.collider.tag == "BasicAI" || hit.collider.tag == "ScoutAI")
            {
                hit.collider.gameObject.GetComponent<AIactions>().TakeDamage(gunDamage);
            }

            if(hit.collider.tag == "StaticAI")
            {
                hit.collider.gameObject.GetComponent<StaticAI>().TakeDamage(gunDamage);
            }
        }

    }

    public void LoseHealth(float healthLoss)
    {
        healthCurrent -= healthLoss;

        if (takeDamageSound != null)
        {
            playerAudioSource.PlayOneShot(takeDamageSound, 0.2f);
        }

        if (healthCurrent <= 0)
        {
            if (playerCanDie)
            {
                gameOverHUD.SetActive(true);
                Time.timeScale = 0;
                firstPersonController.enabled = false;
            }
            healthCurrent = 0;
        }
    }

    public void GainHealth(float healthGain)
    {
        if (pickupHealthSound != null)
        {
            playerAudioSource.PlayOneShot(pickupHealthSound, 1.0f);
        }

        healthCurrent += healthGain;
        if(healthCurrent > healthMax)
        {
            healthCurrent = healthMax;
        }
    }
}

