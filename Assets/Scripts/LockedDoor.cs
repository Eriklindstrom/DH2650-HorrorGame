namespace VRTK.Examples
{
    using UnityEngine;

    public class LockedDoor : VRTK_InteractableObject
    {
        public override void StartUsing(VRTK_InteractUse usingObject)
        {
            base.StartUsing(usingObject);
            GetComponent<AudioSource>().Play(0);
        }

        protected void Start()
        {
            GetComponent<AudioSource>().Pause();
        }
    }
}