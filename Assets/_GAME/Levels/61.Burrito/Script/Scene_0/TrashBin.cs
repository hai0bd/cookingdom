using DG.Tweening;
using Link;
using UnityEngine;

namespace HuyThanh.Cooking.Burrito
{
    public class TrashBin : ItemIdleBase
    {
        [SerializeField] Vector3 needTrashBinPos;
        [SerializeField] Vector3 oldTrashBinPos;

        [SerializeField] Animation anim;
        [SerializeField] string animClose, animOpen;
        public override bool OnTake(IItemMoving item)
        {
            if (item is GarlicRotten garlicRotten)
            {
                item.OnMove(TF.position + Vector3.up * 0.6f, Quaternion.identity, 0.2f);
                item.TF.DOScale(Vector3.zero, 0.2f);
                DOVirtual.DelayedCall(0.2f, () =>
                {
                    SoundControl.Ins.PlayFX(Fx.PutDown);
                    garlicRotten.OnThrow();
                    OnNoNeedTrashBin();
                });

                return true;
            }

            if (item is TrashItem trashItem)
            {
                item.OnMove(TF.position + Vector3.up * 0.6f, Quaternion.identity, 0.2f);
                item.TF.DOScale(Vector3.zero, 0.2f);
                DOVirtual.DelayedCall(0.2f, () =>
                {
                    SoundControl.Ins.PlayFX(Fx.PutDown);
                    trashItem.OnThrow();
                    OnNoNeedTrashBin();
                });

                return true;
            }

            if (item is CornCob cornCob)
            {
                item.OnMove(TF.position + Vector3.up * 0.6f, Quaternion.identity, 0.2f);
                item.TF.DOScale(Vector3.zero, 0.2f);
                DOVirtual.DelayedCall(0.2f, () =>
                {
                    SoundControl.Ins.PlayFX(Fx.PutDown);
                    cornCob.OnThrow();
                    OnNoNeedTrashBin();
                });

                return true;
            }

            if (item is Tissue)
            {
                item.OnMove(TF.position + Vector3.up * 0.6f, Quaternion.identity, 0.2f);
                item.TF.DOScale(Vector3.zero, 0.2f);
                item.ChangeState(Tissue.State.Done);
                DOVirtual.DelayedCall(0.2f, () =>
                {
                    SoundControl.Ins.PlayFX(Fx.PutDown);
                    OnNoNeedTrashBin();
                });

                LevelControl.Ins.CheckStep(1f);

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
            this.TF.DOMove(oldTrashBinPos, 0.2f);
        }
    }

}