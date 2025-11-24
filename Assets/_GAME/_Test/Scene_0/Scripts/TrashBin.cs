using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Link;
using DG.Tweening;
using HuyThanh.Cooking.Burrito;

namespace HoangLinh.Cooking.Test
{
    public class TrashBin : ItemIdleBase
    {
        [SerializeField] Vector3 needTrashBinPos;
        [SerializeField] Vector3 defaultTrashBinPos;

        [SerializeField] Animation anim;
        [SerializeField] string animClose, animOpen;

        public override bool OnTake(IItemMoving item)
        {
            if (item is TrashItem trashItem)
            {
                item.OnMove(TF.position + Vector3.up * 0.6f, Quaternion.identity, 0.2f);
                item.TF.DOScale(Vector3.zero, 0.2f);
                DOVirtual.DelayedCall(0.2f, () =>
                {
                    trashItem.OnThrow();
                    OnNoNeedTrashBin();
                });

                return true;
            }

            return base.OnTake(item);
        }

        public void OnNeedTrashBin()
        {
            this.TF.DOMove(needTrashBinPos, 0.2f).OnComplete(() => anim.Play(animOpen));
        }

        public void OnNoNeedTrashBin()
        {
            anim.Play(animClose);
            //DOVirtual.DelayedCall(0.5f, () => {
            //this.TF.DOMove(defaultTrashBinPos, 0.2f)); };
            DOVirtual.DelayedCall(0.25f, () =>
            {
                this.TF.DOMove(defaultTrashBinPos, 0.2f);
            });


        }
    }
}