using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRTK.Examples
{
    using UnityEngine;

    public class RotatePainting : VRTK_InteractableObject
    {

        private bool open = false;
        private bool isOpened = false;
        float m_distanceTraveled = 0f;
        private float time;
        [SerializeField] private GameObject RotateAround;
        private bool paintingFlipped = false;

        public override void StartUsing(VRTK_InteractUse usingObject)
        {
            base.StartUsing(usingObject);
            open = !open;
            //GetComponent<AudioSource>().Play(0);
        }

        protected override void Update()
        {
            base.Update();
            if (open && !isOpened)
            {
                if (gameObject.tag == "Painting" && !paintingFlipped)
                {
                    Rotation();
                }
            }
            
            else if (!open && isOpened)
            {
            }
        }
        void Rotation()
        {
            time += Time.deltaTime;
            gameObject.transform.RotateAround(RotateAround.transform.position, -Vector3.right, (5.0f * Time.deltaTime));
            if (time > 1.5f)
            {
                paintingFlipped = true;
                time = 0.0f;
            }
        }
    }
}
