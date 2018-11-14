namespace VRTK.Examples
{
    using UnityEngine;

    public class TurnOnFaucet : VRTK_InteractableObject
    {
        public bool flipped = false;
        public bool rotated = false;

        private float sideFlip = -1;
        private float side = -1;
        private float smooth = 90.0f;
        private float doorOpenAngle = -180f;
        private bool open = false;

        private Vector3 defaultRotation;
        private Vector3 openRotation;
        private ParticleSystem pSys;

        public override void StartUsing(VRTK_InteractUse usingObject)
        {
            base.StartUsing(usingObject);
            pSys.Play();
            SetFaucetRotation(usingObject.transform.position);
            SetRotation();
            open = !open;
            GetComponent<AudioSource>().Play(0);
        }

        protected void Start()
        {
            ParticleSystem pSys = GetComponent<ParticleSystem>();
            pSys.Pause();
            defaultRotation = transform.eulerAngles;
            SetRotation();
            sideFlip = (flipped ? 1 : -1);
            GetComponent<AudioSource>().Pause();
        }

        protected override void Update()
        {
            base.Update();
            if (open)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(openRotation), Time.deltaTime * smooth);
            }
            else
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(defaultRotation), Time.deltaTime * smooth);
            }
        }

        private void SetRotation()
        {
            openRotation = new Vector3(defaultRotation.x, defaultRotation.y + (doorOpenAngle * (sideFlip * side)), defaultRotation.z);
        }

        private void SetFaucetRotation(Vector3 interacterPosition)
        {
            side = ((rotated == false && interacterPosition.z > transform.position.z) || (rotated == true && interacterPosition.x > transform.position.x) ? -1 : 1);
        }
    }
}