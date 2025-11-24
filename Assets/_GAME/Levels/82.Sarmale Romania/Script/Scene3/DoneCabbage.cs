using Link;
using UnityEngine;

namespace HuyThanh.Cooking.SarmaleRomania
{
    public class DoneCabbage : ItemMovingBase
    {
        [SerializeField] GameObject sprite1, sprite2;
        [SerializeField] Animation anim;
        [SerializeField] string animGetItem;

        private bool isDone = false;
        public bool IsOnDonePlate => isDone;

        public void PlayAnim()
        {
            anim.Play(animGetItem);
        }

        public void CabbageSetType(int type)
        {
            if (type == 1)
            {
                return;
            }
            if (type == 2)
            {
                sprite1.SetActive(false);
                sprite2.SetActive(true);
            }
        }

        public override void OnDone()
        {
            base.OnDone();
            isDone = true;
        }
    }
}