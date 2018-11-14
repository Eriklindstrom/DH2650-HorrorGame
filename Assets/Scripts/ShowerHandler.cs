namespace VRTK.Examples
{
    using UnityEngine;

    public class ShowerHandler : VRTK_InteractableObject
    {
        private bool open = false;

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
        }

        protected override void Update()
        {
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
    }
}