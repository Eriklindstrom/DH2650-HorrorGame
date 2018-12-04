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
            base.StartUsing(usingObject);
            open = !open;
        }

        
        /*public void Start()
        {
            GameObject BedRoomController = GameObject.Find("Master Bedroom");
            MasterBedroomController controller = BedRoomController.GetComponent<MasterBedroomController>();
            MasterBedroomController.paintingFlipped = true;
        }*/


    protected override void Update()
        {
            base.Update();
            if (open && !isOpened)
            {
                if (gameObject.tag == "DoorLeft")
                {
                    float angle = 75 * Time.deltaTime;
                    if(totAngle <= 90)
                    {
                        totAngle += angle;
                        transform.RotateAround(RotateAround.transform.position, Vector3.up, angle);
                    }
                    
                    else if (totAngle >= 90)
                    {
                        //open = !open;
                        totAngle = 0;
                        isOpened = true;
                    }
                }

                if (gameObject.tag == "DoorRight")
                {
                    float angle = 75 * Time.deltaTime;
                    if (totAngle <= 90)
                    {
                        totAngle += angle;
                        transform.RotateAround(RotateAround.transform.position, -Vector3.up, angle);
                    }

                    else if (totAngle >= 90)
                    {
                        //open = !open;
                        totAngle = 0;
                        isOpened = true;
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
                        //open = !open;
                        totAngle = 0;
                        isOpened = true;
                    }
                }

            }
            else if(!open && isOpened)
            {
                if (gameObject.tag == "DoorLeft")
                {
                    float angle = 75 * Time.deltaTime;
                    if (totAngle <= 90)
                    {
                        totAngle += angle;
                        transform.RotateAround(RotateAround.transform.position, -Vector3.up, angle);
                    }
                    else if (totAngle >= 90)
                    {
                        //open = !open;
                        totAngle = 0;
                        isOpened = false;
                    }
                }

                if (gameObject.tag == "DoorRight")
                {
                    float angle = 75 * Time.deltaTime;
                    if (totAngle <= 90)
                    {
                        totAngle += angle;
                        transform.RotateAround(RotateAround.transform.position, Vector3.up, angle);
                    }
                    else if (totAngle >= 90)
                    {
                        //open = !open;
                        totAngle = 0;
                        isOpened = false;
                    }
                }
                if (gameObject.tag == "RotateUp")
                {
                    float angle = 45 * Time.deltaTime;
                    if (totAngle <= 90)
                    {
                        totAngle += angle;
                        transform.RotateAround(RotateAround.transform.position, Vector3.forward, angle);
                    }
                    else if (totAngle >= 90)
                    {
                        //open = !open;
                        totAngle = 0;
                        isOpened = false;
                    }
                }
            }
        }
    }
}
    
