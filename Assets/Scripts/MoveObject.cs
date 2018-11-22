using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRTK.Examples
{
    using UnityEngine;

    public class MoveObject : VRTK_InteractableObject
    {
        private bool open = false;
        float m_distanceTraveled = 0f;

        public override void StartUsing(VRTK_InteractUse usingObject)
        {
            base.StartUsing(usingObject);
            open = !open;
        }

        protected void Start()
        {
            AudioSource Sound = GetComponent<AudioSource>();
            Sound.Pause();
        }
        protected override void Update()
        {
            base.Update();
            if (open)
            {
                if (gameObject.tag == "Chair")
                {
                    if (m_distanceTraveled < 1f)
                    {
                        Vector3 oldPosition = transform.position;
                        transform.Translate(0, 0, -1 * Time.deltaTime);
                        m_distanceTraveled += Vector3.Distance(oldPosition, transform.position);
                    }
                }

                if(gameObject.tag == "Desk")
                {
                    if (m_distanceTraveled < 0.3f)
                    {
                        Vector3 oldPosition = transform.position;
                        transform.Translate(0, 0, 1 * Time.deltaTime);
                        m_distanceTraveled += Vector3.Distance(oldPosition, transform.position);
                    }
                }
            }
            
            /*else
            {
                if (m_distanceTraveled < 1f)
                {
                    Vector3 oldPosition = transform.position;
                    transform.Translate(0, 0, -1 * Time.deltaTime);
                    m_distanceTraveled += Vector3.Distance(oldPosition, transform.position);
                }
            }*/
        }
    }
}
