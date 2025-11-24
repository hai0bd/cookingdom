using Link;
using UnityEngine;

namespace HuyThanh.Cooking.SarmaleRomania
{
    public enum CabbageRollType { OnDish1, OnDish2 }
    public class PotCabbageRoll : ItemIdleBase
    {

        [SerializeField] ItemAlpha onDish1, onDish2, onTong;
        [SerializeField] Animation anim;
        [SerializeField] string getItemAnimName;

        public void ChangeOnTong()
        {
            onDish1.DoAlpha(0f, 0.2f);
            onDish2.DoAlpha(0f, 0.2f);
            onTong.DoAlpha(1f, 0.2f);
            anim.Play(getItemAnimName);
        }

        public void ChangeOnDish(CabbageRollType type)
        {
            this.GetComponent<Collider2D>().enabled = false;

            anim.Play(getItemAnimName);
            switch (type)
            {
                case CabbageRollType.OnDish1:
                    onDish1.DoAlpha(1f, 0.2f);
                    onDish2.DoAlpha(0f, 0.2f);
                    onTong.DoAlpha(0f, 0.2f);
                    break;
                case CabbageRollType.OnDish2:
                    onDish1.DoAlpha(0f, 0.2f);
                    onDish2.DoAlpha(1f, 0.2f);
                    onTong.DoAlpha(0f, 0.2f);
                    break;
            }
        }
    }
}