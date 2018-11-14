using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
* Handles rotation of doors. It would be cool to be able to open the doors with the vive controllers however. 
**/
namespace VRTK.Examples
{
    public class rotateDoor : VRTK_InteractableObject
    {

        [SerializeField] private GameObject RotateAround;   //Empty object on door to rotate around

        private bool open = false;
        private float totAngle;             //Float to check when to stop rotating


        public override void StartUsing(VRTK_InteractUse usingObject)
        {
            open = !open;
        }


        protected override void Update()
        {
            base.Update();
            if (open)
            {
                float angle = 45 * Time.deltaTime;
                totAngle += angle;
                transform.RotateAround(RotateAround.transform.position, Vector3.up, angle);
                if (totAngle > 90)
                {
                    open = !open;
                }
            }
            /*else
            {
                float angle = -45 * Time.deltaTime;
                totAngle += angle;
                transform.RotateAround(RotateAround.transform.position, Vector3.up, angle);
                if (totAngle < 0)
                {
                    open = !open;
                }
            }*/
        }
    }
}
    
