using Link;
using UnityEngine;

namespace HuyThanh.Cooking.SarmaleRomania
{
    public class PotRib : ItemIdleBase
    {
        [SerializeField] ItemAlpha onDish1, onTong;
        [SerializeField] Animation anim;
        [SerializeField] string getItemAnimName;

        public void ChangeOnTong()
        {
            onDish1.DoAlpha(0f, 0.2f);
            onTong.DoAlpha(1f, 0.2f);
            anim.Play(getItemAnimName);
        }

        public void ChangeOnDish()
        {
            this.GetComponent<Collider2D>().enabled = false;

            anim.Play(getItemAnimName);
            onDish1.DoAlpha(1f, 0.2f);
            onTong.DoAlpha(0f, 0.2f);
        }
    }
}