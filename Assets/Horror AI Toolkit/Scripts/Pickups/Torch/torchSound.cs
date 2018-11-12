using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class torchSound : MonoBehaviour
{
    AudioSource source;
    public AudioClip onSound;
    public AudioClip flickerSound;
    Player player;

	// Use this for initialization
	void Start ()
    {
        source = GetComponent<AudioSource>();
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(player.torchIsFlickering)
        {
            source.clip = flickerSound;
            if(!source.isPlaying)
            {
                source.Play();
                source.loop = false;
            }
        }
		else if(player.torchOnStatus)
        {
            source.clip = onSound;
            if(!source.isPlaying)
            {
                source.Play();
                source.loop = true;
            }
        }
        else
        {
            source.clip = onSound;
            if (source.isPlaying)
            {
                source.Stop();
                source.loop = false;
            }
        }
	}
}
