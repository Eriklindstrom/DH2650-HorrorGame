using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
* Handles rotation of doors. Can't close
**/
namespace VRTK.Examples
{
    public class rotateDoor : VRTK_InteractableObject
    {

        [SerializeField] private GameObject RotateAround;   //Empty object on door to rotate around

        private bool open = false;
        private bool isOpened = false;
        private float totAngle;             //Float to check when to stop rotating


        public override void StartUsing(VRTK_InteractUse usingObject)
        {
            open = !open;
        }


        protected override void Update()
        {
            base.Update();
            if (open && !isOpened)
            {
                //Debug.Log("1" + isOpened);
                //Debug.Log(isOpened);
                if (gameObject.tag != "RotateUp")
                {
                    float angle = 45 * Time.deltaTime;
                    if(totAngle <= 90)
                    {
                        totAngle += angle;
                        transform.RotateAround(RotateAround.transform.position, Vector3.up, angle);
                    }
                    
                    else if (totAngle >= 90)
                    {
                        open = !open;
                        isOpened = !isOpened;
                    }
                }

                if (gameObject.tag == "RotateUp")
                {
                    float angle = 45 * Time.deltaTime;
                    if (totAngle <= 90)
                    {
                        totAngle += angle;
                        transform.RotateAround(RotateAround.transform.position, -Vector3.forward, angle);
                    }
                    else if (totAngle >= 90)
                    {
                        open = !open;
                        isOpened = !isOpened;
                    }
                }

            }
            else if(open && isOpened)
            {
                //Debug.Log("2" + isOpened);
                if (gameObject.tag != "RotateUp")
                {
                    float angle = 45 * Time.deltaTime;
                    if (totAngle <= 90)
                    {
                        totAngle += angle;
                        transform.RotateAround(RotateAround.transform.position, -Vector3.up, angle);
                    }
                    else if (totAngle >= 90)
                    {
                        open = !open;
                        isOpened = !isOpened;
                    }
                }
                if (gameObject.tag == "RotateUp")
                {
                    float angle = 45 * Time.deltaTime;
                    if (totAngle <= 90)
                    {
                        totAngle += angle;
                        transform.RotateAround(RotateAround.transform.position, -Vector3.forward, angle);
                    }
                    else if (totAngle >= 90)
                    {
                        open = !open;
                        isOpened = !isOpened;
                    }
                }
            }
        }
    }
}
    
