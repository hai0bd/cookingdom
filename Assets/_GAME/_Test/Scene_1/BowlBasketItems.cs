using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Link;

namespace HoangLinh.Cooking.Test
{
    public class BowlBasketItems : ItemIdleBase
    {
        public enum ItemSize { Big, Small};

        [SerializeField] ItemMovingBase item;
        [SerializeField] ItemSize itemSize;

        public override bool OnTake(IItemMoving item)
        {
            if(itemSize is ItemSize.Small && this.item == null)
            {
                if (item.TF.name == "Onion" && item.IsState(BasketCutItems.State.Spice))
                {
                    this.item = item as ItemMovingBase;
                    item.OnMove(TF.position, Quaternion.identity, 0.2f);
                    item.ChangeState(BasketCutItems.State.Done);
                    return true;
                }
            }

            return base.OnTake(item);
        }

    }
}