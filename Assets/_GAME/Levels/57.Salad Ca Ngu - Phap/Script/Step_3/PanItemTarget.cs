using DG.Tweening;
using Link;
using UnityEngine;
namespace HuyThanh.Cooking.TunaSaladFrench
{
    public class PanItemTarget : ItemIdleBase
    {
        [SerializeField] PanItemMoving itemPan;
        [SerializeField] Collider2D col;

        [SerializeField] private PanOvenIdle panController;

        [SerializeField] Transform secondTarget;

        [SerializeField] PanItemMoving.ItemType type;
        public override bool OnTake(IItemMoving item)
        {
            if (item is PanItemMoving itemMovingPan && type == itemMovingPan.GetType())
            {
                item.TF.SetParent(transform);
                item.OnMove(TF.position, TF.rotation, 0.2f);
                item.OnDone();
                panController.OnDonePanItemTarget(this, itemMovingPan.GetType());
                OnDone();

                if (itemMovingPan.GetType() == PanItemMoving.ItemType.Pearl)
                {
                    DOVirtual.DelayedCall(0.2f, () =>
                    {
                        itemMovingPan.OrderLayer = -1;
                    });

                    itemMovingPan.PearlMove(secondTarget);
                }
                else
                {
                    DOVirtual.DelayedCall(0.2f, () =>
                    {
                        itemMovingPan.OrderLayer = -2;
                    });

                }
                return true;
            }
            return base.OnTake(item);
        }

        public void EnableCollider()
        {
            this.col.enabled = true;
        }
        public override void OnDone()
        {
            base.OnDone();

            col.enabled = false;
        }
    }
}