using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRTK.Examples
{
    using UnityEngine;

    

    public class SmashPiggy : VRTK_InteractableObject
    {

        private bool isUsing = false;
        public Component[] rb;
        private bool smashedPiggy = false;

        public override void StartUsing(VRTK_InteractUse usingObject)
        {
            base.StartUsing(usingObject);
            isUsing = true;
        }

        public override void StopUsing(VRTK_InteractUse usingObject)
        {
            base.StopUsing(usingObject);
            isUsing = true;
        }

        protected override void Update()
        {
            base.Update();
            if(isUsing)
            {
                gameObject.GetComponent<Rigidbody>().isKinematic = false;
                Destroy(gameObject, 10.0f);
            }
        }

        void OnCollisionEnter(Collision col)
        {
            Debug.Log("Collision with piggy");
            if (col.gameObject.tag != "Untagged")
            {
                smashedPiggy = true;
                Debug.Log("smashpiggy");
                //rb = GetComponentsInChildren<Rigidbody>();
                foreach (Rigidbody body in rb)
                {
                    body.isKinematic = false;
                }
            }
        }
    }
}
