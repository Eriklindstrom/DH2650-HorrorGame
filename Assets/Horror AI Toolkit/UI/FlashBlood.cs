using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashBlood : MonoBehaviour
{

    public float FadeRate;
    private Image image;
    private float targetAlpha;

    Player player;
    float lastPlayerHealth = 0;
    float currentPlayerHealth = 0;

    // Use this for initialization
    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();

        this.image = this.GetComponent<Image>();
        if (this.image == null)
        {
            Debug.LogError("Error: No image on " + this.name);
        }
        this.targetAlpha = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        currentPlayerHealth = player.healthCurrent;
        if(currentPlayerHealth < lastPlayerHealth)
        {
            FadeIn();
        }
        lastPlayerHealth = currentPlayerHealth;

        Color curColor = this.image.color;
        float alphaDiff = Mathf.Abs(curColor.a - this.targetAlpha);
        if (alphaDiff > 0.0001f)
        {
            curColor.a = Mathf.Lerp(curColor.a, targetAlpha, this.FadeRate * Time.deltaTime);
            this.image.color = curColor;

            FadeOut();
        }
    }

    public void FadeOut()
    {
        this.targetAlpha = 0.0f;
    }

    public void FadeIn()
    {
        this.targetAlpha = 0.4f;
    }
}
