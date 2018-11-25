namespace VRTK.Examples
{
    using UnityEngine;

    public class ShowerHandler : VRTK_InteractableObject
    {
        private bool open = false;
        private bool horrorOverride = false;
        private Color originalColor;

        private ParticleSystem pSys;
        private AudioSource Sound;

        public override void StartUsing(VRTK_InteractUse usingObject)
        {

            base.StartUsing(usingObject);
            open = !open;
        }

        protected void Start()
        {
            //transform.GetChild(0).gameObject.SetActive(false);
            AudioSource Sound = GetComponent<AudioSource>();
            Sound.Pause();
            ParticleSystem pSys = gameObject.GetComponentInChildren<ParticleSystem>();
            pSys.Pause();

            originalColor = pSys.startColor;
        }

        protected override void Update()
        {
            if (horrorOverride) return;

            base.Update();
            if (open)
            {
                //transform.GetChild(0).gameObject.SetActive(true);
                GetComponent<AudioSource>().Play(0);
                gameObject.GetComponentInChildren<ParticleSystem>().Play();
            }
            else
            {
                //transform.GetChild(0).gameObject.SetActive(false);
                GetComponent<AudioSource>().Stop();
                gameObject.GetComponentInChildren<ParticleSystem>().Stop();
            }
        }

        public void RemoteStart()
        {
            horrorOverride = true;

            var particles = gameObject.GetComponentInChildren<ParticleSystem>();
            if (particles.isPlaying)
                return;

            GetComponent<AudioSource>().Play(0);
            particles.Play();            
        }

        public void SetColor(Color color)
        {
            gameObject.GetComponentInChildren<ParticleSystem>().startColor = color;
        }

        public void ResetColor()
        {
            gameObject.GetComponentInChildren<ParticleSystem>().startColor = originalColor;
        }
    }
}