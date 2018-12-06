namespace VRTK.Examples
{
    using UnityEngine;

    public class ShowerHandler : VRTK_InteractableObject
    {
        [SerializeField] GameObject bathroomControllerObj;

        private bool open = false;
        private bool horrorOverride = false;
        private bool bloodified = false;

        private Color originalColor;
        private ParticleSystem pSys;
        private AudioSource sound;
        private BathroomController bathroomController;

        public override void StartUsing(VRTK_InteractUse usingObject)
        {
            base.StartUsing(usingObject);
            open = !open;

            if (horrorOverride)
            {
                if (bloodified) return;

                bathroomController.TriggerHorror();
                bloodified = true;
                return;
            }
                        
            if (open)
            {
                sound.Play(0);
                pSys.Play();
            }
            else
            {
                sound.Stop();
                pSys.Stop();
            }
        }

        protected void Start()
        {
            sound = GetComponent<AudioSource>();
            pSys = gameObject.GetComponentInChildren<ParticleSystem>();
            bathroomController = bathroomControllerObj.GetComponent<BathroomController>();            
            
            sound.Pause();
            pSys.Pause();
            originalColor = pSys.startColor;
        }

        public void RemoteStart()
        {
            horrorOverride = true;

            if (pSys.isPlaying)
                return;

            GetComponent<AudioSource>().Play(0);
            pSys.Play();            
        }


        public void SetColor(Color color)
        {
            pSys.startColor = color;
        }

        public void ResetColor()
        {
            pSys.startColor = originalColor;
        }
    }
}