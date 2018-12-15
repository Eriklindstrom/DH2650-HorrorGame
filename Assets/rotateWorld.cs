using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VRTK.Examples
{

    public class rotateWorld : VRTK_InteractableObject
    {

        [SerializeField] private GameObject Cellar;
        [SerializeField] private GameObject BackWall;
        [SerializeField] private GameObject FinalRoom;
        [SerializeField] private GameObject Door;
        [SerializeField] private GameObject Player;
        [SerializeField] private GameObject[] Mannequins;

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

                foreach(GameObject man in Mannequins)
                {
                    man.SetActive(false);
                    //man.GetComponent<Rigidbody>().isKinematic = false;
                }

                StartCoroutine(LoadScene());
            }

        }

        IEnumerator LoadScene()
        {
            yield return new WaitForSeconds(5.0f);
            gameObject.GetComponent<AudioSource>().Play(0);
            yield return new WaitForSeconds(5.0f);
            SceneManager.LoadScene(0);
        }
    }
}
