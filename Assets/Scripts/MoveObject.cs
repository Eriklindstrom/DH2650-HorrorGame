using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRTK.Examples
{
    using UnityEngine;

    public class MoveObject : VRTK_InteractableObject
    {
        private bool open = false;
        private bool isOpened = false;
        float m_distanceTraveled = 0f;

        public override void StartUsing(VRTK_InteractUse usingObject)
        {
            base.StartUsing(usingObject);
            open = !open;
            GetComponent<AudioSource>().Play(0);
        }

        protected void Start()
        {
            try {
                AudioSource Sound = GetComponent<AudioSource>();
                Sound.Pause();
            }
            catch (MissingComponentException)
            {
                Debug.Log("No sound on all interactable object. No worries, don't need sound on everything");
            }
                
        }
        protected override void Update()
        {
            base.Update();
            if (open && !isOpened)
            {
                /*Handles Chairs in z axis*/
                if (gameObject.tag == "Chair")
                {
                    if (m_distanceTraveled < 1f)
                    {
                        Vector3 oldPosition = transform.position;
                        transform.Translate(0, 0, -0.9f * Time.deltaTime);
                        m_distanceTraveled += Vector3.Distance(oldPosition, transform.position);
                    }
                    else
                    {
                        isOpened = true;
                        m_distanceTraveled = 0.0f;

                    }
                }
                /*Handles desks in z axis */
                if (gameObject.tag == "Desk")
                {
                    if (m_distanceTraveled < 0.3f)
                    {
                        Vector3 oldPosition = transform.position;
                        transform.Translate(0, 0, 0.7f * Time.deltaTime);
                        m_distanceTraveled += Vector3.Distance(oldPosition, transform.position);
                    }
                    else
                    {
                        isOpened = true;
                        m_distanceTraveled = 0.0f;
                    }
                }
                /*Handles Wardrobe in z axis, weird scale*/
                if (gameObject.tag == "Wardrobe")
                {
                    if (m_distanceTraveled < .3f)
                    {
                        Vector3 oldPosition = transform.position;
                        transform.Translate(0, -0.8f * Time.deltaTime, 0);
                        m_distanceTraveled += Vector3.Distance(oldPosition, transform.position);
                    }
                    else
                    {
                        isOpened = true;
                        m_distanceTraveled = 0.0f;
                    }
                }
                /*Handles Wardrobe in z axis, weird scale*/
                if (gameObject.tag == "DoorLeft")
                {
                    if (m_distanceTraveled < 0.5f)
                    {
                        Vector3 oldPosition = transform.position;
                        transform.Translate(-0.8f * Time.deltaTime, 0, 0);
                        m_distanceTraveled += Vector3.Distance(oldPosition, transform.position);
                    }
                    else
                    {
                        isOpened = true;
                        m_distanceTraveled = 0.0f;
                    }
                }
                /*Handles Wardrobe in z axis, weird scale*/
                if (gameObject.tag == "DoorRight")
                {
                    if (m_distanceTraveled < 0.5f)
                    {
                        Vector3 oldPosition = transform.position;
                        transform.Translate(0.8f * Time.deltaTime, 0, 0);
                        m_distanceTraveled += Vector3.Distance(oldPosition, transform.position);
                    }
                    else
                    {
                        isOpened = true;
                        m_distanceTraveled = 0.0f;
                    }
                }
            }
            /**Close again**/
            else if (!open && isOpened)
            {
                if (gameObject.tag == "Chair")
                {
                    if (m_distanceTraveled < 1f)
                    {
                        Vector3 oldPosition = transform.position;
                        transform.Translate(0, 0, 0.9f * Time.deltaTime);
                        m_distanceTraveled += Vector3.Distance(oldPosition, transform.position);
                    }
                    else
                    {
                        isOpened = false;
                        m_distanceTraveled = 0.0f;
                    }
                }

                if (gameObject.tag == "Desk")
                {
                    if (m_distanceTraveled < 0.3f)
                    {
                        Vector3 oldPosition = transform.position;
                        transform.Translate(0, 0, -0.7f * Time.deltaTime);
                        m_distanceTraveled += Vector3.Distance(oldPosition, transform.position);
                    }
                    else
                    {
                        isOpened = false;
                        m_distanceTraveled = 0.0f;
                    }
                }
                /*Handles Wardrobe in z axis, weird scale*/
                if (gameObject.tag == "Wardrobe")
                {
                    if (m_distanceTraveled < .3f)
                    {
                        Vector3 oldPosition = transform.position;
                        transform.Translate(0, 0.8f * Time.deltaTime, 0);
                        m_distanceTraveled += Vector3.Distance(oldPosition, transform.position);
                    }
                    else
                    {
                        isOpened = false;
                        m_distanceTraveled = 0.0f;
                    }
                }
            }
        }

        public void setOpen(bool val)
        {
            open = val;
        }
    }
}
