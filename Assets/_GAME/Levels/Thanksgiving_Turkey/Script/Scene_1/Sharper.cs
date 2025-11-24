using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link.Turkey
{
    public class Sharper : ItemIdleBase
    {
        [SerializeField] Collider2D collider;

        public override bool OnTake(IItemMoving item)
        {
            if (item is Knife)
            {
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                (item as Knife).OnSharping(0.2f);
                //item.OnBack();
                collider.enabled = false;
                return true;
            }
            else
            {
                item.OnBack();
            }
            return base.OnTake(item);
        }
    }
}