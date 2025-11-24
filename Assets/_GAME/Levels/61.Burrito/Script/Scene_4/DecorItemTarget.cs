using DG.Tweening;
using Link;
using UnityEngine;

namespace HuyThanh.Cooking.Burrito
{
    public class DecorItemTarget : ItemIdleBase
    {
        [SerializeField] DecorItemType type;

        [SerializeField] int itemTargetOrder;

        private bool isHaveItem = false;

        public override bool OnTake(IItemMoving item)
        {
            if (item is DecorItem decorItem && isHaveItem == false)
            {
                if (decorItem.IsType(type))
                {
                    isHaveItem = true;
                    decorItem.SetOrder(itemTargetOrder);
                    decorItem.OnDone();
                    decorItem.RemoveFromTarget();
                    //decorItem.OnMove(TF.position, TF.rotation, 0.2f);
                    decorItem.TF.DORotate(TF.rotation.eulerAngles, 0.2f);
                    decorItem.TF.DOJump(TF.position, 0.2f, 1, 0.2f);
                    return true;
                }
            }

            if (item is BurritoDoneCut burritoDoneCut && isHaveItem == false && burritoDoneCut.IsType(type))
            {
                isHaveItem = true;
                burritoDoneCut.SetOrder(itemTargetOrder);
                burritoDoneCut.OnDone();
                burritoDoneCut.RemoveFromTarget();
                //burritoDoneCut.OnMove(TF.position, TF.rotation, 0.2f);
                burritoDoneCut.TF.DORotate(TF.rotation.eulerAngles, 0.2f);
                burritoDoneCut.TF.DOJump(TF.position, 0.2f, 1, 0.2f);
                return true;
            }
            return base.OnTake(item);
        }
    }
}