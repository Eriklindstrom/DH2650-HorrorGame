using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour {

    [SerializeField] private AudioSource safeSoudns;
    [SerializeField] private AudioSource scarySounds;
    [SerializeField] private Image blinds;

    private bool changeTriggered = false;
    private float alpha = 0.0f;


    void OnCollisionExit(Collision col)
    {
       
        if (col.transform.tag == "Toilet" && !changeTriggered)
        {
            StartCoroutine(AudioFade.FadeOut(safeSoudns, 1f));

            scarySounds.enabled = true;
            scarySounds.clip = MakeSubclip(scarySounds.clip, 9, 30);
            StartCoroutine(AudioFade.FadeIn(scarySounds, 10f));

            StartCoroutine("DoSwitch", 1);
            changeTriggered = true;
        }    
    }

    public IEnumerator DoSwitch(int scene)
    {
        Debug.Log("coroutine");
        float timer = 0.0f;
        while(timer < 8.0f)
        {
            timer += Time.deltaTime;
            alpha = (timer/6.0f);
            blinds.color = new Color(blinds.color.r, blinds.color.g, blinds.color.b, alpha);
            yield return null;
        }
        scarySounds.Stop();
        SceneManager.LoadScene(scene);
        yield break;
    }

    public static class AudioFade
    {
        public static IEnumerator FadeIn(AudioSource audioSource, float FadeTime)
        {
            audioSource.volume = 0;
            audioSource.Play();
            while (audioSource.volume < 0.7)
            {
                audioSource.volume += Time.deltaTime / FadeTime;
                yield return null;
            }
        }

        public static IEnumerator FadeOut(AudioSource audioSource, float FadeTime)
        {
            float startVolume = audioSource.volume;
            while (audioSource.volume > 0)
            {
                audioSource.volume -= startVolume * Time.deltaTime / FadeTime;
                yield return null;
            }

            audioSource.Stop();
            audioSource.volume = startVolume;
        }

    }

    private AudioClip MakeSubclip(AudioClip clip, float start, float stop)
    {
        /* Create a new audio clip */
        int frequency = clip.frequency;
        float timeLength = stop - start;
        int samplesLength = (int)(frequency * timeLength);
        AudioClip newClip = AudioClip.Create(clip.name + "-sub", samplesLength, 1, frequency, false);

        /* Create a temporary buffer for the samples */
        float[] data = new float[samplesLength];

        /* Get the data from the original clip */
        clip.GetData(data, (int)(frequency * start));

        /* Transfer the data to the new clip */
        newClip.SetData(data, 0);

        /* Return the sub clip */
        return newClip;
    }
}
