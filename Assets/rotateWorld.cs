﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRTK.Examples
{

    public class rotateWorld : VRTK_InteractableObject
    {

        [SerializeField] private GameObject Cellar;
        [SerializeField] private GameObject BackWall;
        [SerializeField] private GameObject FinalRoom;
        [SerializeField] private GameObject Door;
        [SerializeField] private GameObject Player;

        public bool isOpened;
        private float totAngle;
        private bool rotate = true;

        // Use this for initialization
        void Start() {
            bool isOpened = gameObject.GetComponent<rotateDoor>().isOpened;
        }

        // Update is called once per frame
        protected override void Update() {
            if(Door.GetComponent<rotateDoor>().isOpened && rotate)
            {
                BackWall.GetComponent<BoxCollider>().enabled = false;
                //BackWall.SetActive(false);
                FinalRoom.SetActive(true);

                float angle = 75 * Time.deltaTime;
                if (totAngle <= 90)
                {
                    totAngle += angle;
                    transform.RotateAround(Player.transform.position, Vector3.left, angle);
                }

                else if (totAngle >= 90)
                {
                    totAngle = 0;
                    rotate = false;
                }
            }

        }
    }
}
