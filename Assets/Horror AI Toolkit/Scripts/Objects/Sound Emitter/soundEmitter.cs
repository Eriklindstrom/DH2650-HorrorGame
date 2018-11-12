using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SoundType
{
    PLAYER,
    PLAYERTHROWNOBJ,
    RADIO,
    TV
}

public class soundEmitter : MonoBehaviour
{
    [Tooltip("The range of the sound emitter. Once an AI enters this trigger they will hear it.")]
    public SphereCollider soundCol;
    [Tooltip("The volume of the emitter. This effects the range of the sound. AI will prioritise sound emitters with the higher volume.")]
    public float volume = 0.0f;
    [Tooltip("The sound type of this emitter. AI will ignore sound types if they search too many of the same.")]
    public SoundType soundType;
    [Tooltip("If there is switch object child the player will be able to turn the emitter on or off. The switch must be the first child of this object.")]
    public soundEmitterSwitch emitterSwitch;
    [Tooltip("Whether or not this sound emitter has a sound effect to play. If enabled an audio source with a clip must be attatched.")]
    public bool playAudio = false;
    AudioSource audioSource;

    // Use this for initialization
    void Start ()
    {
        soundCol = GetComponent<SphereCollider>();

        if(GetComponent<AudioSource>() != null && playAudio)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (gameObject.transform.childCount > 0)
            emitterSwitch = gameObject.transform.GetChild(0).GetComponent<soundEmitterSwitch>();
    }

    void Update()
    {
        if (emitterSwitch != null)
        {
            if (emitterSwitch.isOn)
            {
                if(audioSource != null && playAudio)
                {
                    if (!audioSource.isPlaying)
                    {
                        audioSource.Play();
                    }
                }
                soundCol.radius = volume;
            }
            else
            {
                if (audioSource != null && playAudio)
                {
                    if (audioSource.isPlaying)
                    {
                        audioSource.Stop();
                    }
                }
                soundCol.radius = 0;
            }
        }
        else
        {
            if (audioSource != null && playAudio)
            {
                if (!audioSource.isPlaying)
                {
                    audioSource.Play();
                }
            }
            soundCol.radius = volume;
        }
    }

    void OnTriggerStay(Collider col)
    {
        if((col.tag == "AdvancedAI" || col.tag == "BasicAI" || col.tag == "ScoutAI") && !col.isTrigger)
        {
            AIactions enemy = col.gameObject.GetComponent<AIactions>();
           
            if(enemy.canHearSounds && enemy.CheckIfCloseToPosition(this.transform.position, volume))
            {
                if (!enemy.checkedSoundEmitters.Contains(this) && !enemy.ignoreSoundTypes.Contains(this.soundType))
                {
                    enemy.soundIsHeard = true;

                    if (enemy.lastHeardSoundEmitter == null)
                    {
                        enemy.lastHeardSoundEmitter = this;
                    }
                    else
                    {
                        if ((this.volume >= enemy.lastHeardSoundEmitter.volume) && (enemy.lastHeardSoundEmitter != this))
                        {
                            enemy.lastHeardSoundEmitter = this;
                        }
                    }
                }
                else
                {
                    enemy.soundIsHeard = false;
                }
            }
        }
    }

    void OnTriggerExit(Collider col)
    {
        if ((col.tag == "AdvancedAI" || col.tag == "BasicAI" || col.tag == "ScoutAI") && !col.isTrigger)
        {
            AIactions enemy = col.gameObject.GetComponent<AIactions>();
            enemy.soundIsHeard = false;
        }
    }
}
