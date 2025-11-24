using DG.Tweening;
using Link;
using UnityEngine;

namespace HuyThanh.Cooking.TunaSaladFrench
{

    public class MixItemTarget : ItemIdleBase
    {
        public enum ItemType
        {
            XaLach,
            Tomato,
            Egg,
        }
        [SerializeField] ItemType itemType;
        [SerializeField]
        ItemMovingBase item;
        [SerializeField] DoneMix doneMix;


        public override bool OnTake(IItemMoving item)
        {
            if (item is MixItemMove mixItemMove && this.item == item as ItemMovingBase)
            {
                mixItemMove.TF.SetParent(this.TF);
                mixItemMove.OnMove(TF.position, TF.rotation, 0.2f);
                DOVirtual.DelayedCall(0.2f, () => mixItemMove.OrderLayer = -49);
                this.GetComponent<Collider2D>().enabled = false;
                mixItemMove.OnDone();
                if (itemType == ItemType.XaLach)
                    doneMix.OnDoneXaLach(this.gameObject);
                if (itemType == ItemType.Tomato)
                    doneMix.OnDoneTomato(this.gameObject);
                if (itemType == ItemType.Egg)
                    doneMix.OnDoneEgg(this.gameObject);
                return true;
            }
            return base.OnTake(item);
        }
    }

}