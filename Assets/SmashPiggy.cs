﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRTK.Examples
{
    using UnityEngine;

    

    public class SmashPiggy : VRTK_InteractableObject
    {

        private bool isUsing = false;

        public override void StartUsing(VRTK_InteractUse usingObject)
        {
            base.StartUsing(usingObject);
            isUsing = true;
        }

        public override void StopUsing(VRTK_InteractUse usingObject)
        {
            base.StopUsing(usingObject);
            isUsing = false;
        }

        public Component[] rb;

        /* public override void StartUsing(VRTK_InteractUse usingObject)
         {

         }

        */
        // Use this for initialization
        /* void Start()
         {

         }
         */

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
            if (col.gameObject.tag != "Untagged")
            {
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
