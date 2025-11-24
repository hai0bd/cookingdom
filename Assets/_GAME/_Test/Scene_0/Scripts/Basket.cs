using HuyThanh.Cooking.Burrito;
using Link;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoangLinh.Cooking.Test
{
    public class Basket : ItemIdleBase
    {
        [SerializeField] List<ItemMovingBase> basketItem;
        [SerializeField] Animation anim;

        public override bool OnTake(IItemMoving item)
        {
            if (IsGetItem(item) && item is DirtyItems && item.IsState(DirtyItems.State.Normal))
            {
                item.ChangeState(DirtyItem.State.Done);
                RemoveGetItem(item);
                return true;
            }

            return base.OnTake(item);
        }

        private bool IsGetItem(IItemMoving item)
        {
            return basketItem.Contains(item as ItemMovingBase);
        }

        private void RemoveGetItem(IItemMoving item)
        {
            basketItem.Remove(item as ItemMovingBase);
        }
    }
}
