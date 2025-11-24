using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link.Turkey
{
    public class TrashBin : ItemIdleBase
    {
        public override bool OnTake(IItemMoving item)
        {
            if (item is Tissue)
            {
                if (!item.IsState(Tissue.State.Dirty)) item.ChangeState(Tissue.State.Dirty);
                item.ChangeState(Tissue.State.Remove);
                item.OnMove(TF.position, Quaternion.identity, 0.3f);
                item.TF.DOScale(Vector3.zero, 0.3f);
                Destroy((item as Tissue).gameObject, 0.35f);
                LevelControl.Ins.CheckStep();
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