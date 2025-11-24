using DG.Tweening;
using Link;
using UnityEngine;
namespace HuyThanh.Cooking.SarmaleRomania
{
    public class TrashBin : ItemIdleBase
    {
        [SerializeField] Vector3 needTrashBinPos;
        [SerializeField] Vector3 oldTrashBinPos;

        [SerializeField] Animation anim;
        [SerializeField] string animClose, animOpen;
        public override bool OnTake(IItemMoving item)
        {
            if (item is DillMoveTrash dillMoveTrash)
            {
                item.TF.DOJump(TF.position + Vector3.up * 0.6f, 1, 1, 0.15f);
                item.TF.DOScale(Vector3.zero, 0.2f);
                DOVirtual.DelayedCall(0.2f, () =>
                {
                    dillMoveTrash.OnThrow();
                    OnNoNeedTrashBin();
                });

                return true;
            }

            if (item is CabbageMoveTrash cabbageMoveTrash)
            {
                item.TF.DOJump(TF.position + Vector3.up * 0.6f, 1, 1, 0.15f);
                item.TF.DOScale(Vector3.zero, 0.2f);
                DOVirtual.DelayedCall(0.2f, () =>
                {
                    cabbageMoveTrash.OnThrow();
                    OnNoNeedTrashBin();
                });

                return true;
            }

            if (item is Core core)
            {
                item.TF.DOJump(TF.position + Vector3.up * 0.6f, 1, 1, 0.15f);
                item.TF.DOScale(Vector3.zero, 0.2f);
                DOVirtual.DelayedCall(0.2f, () =>
                {
                    core.OnThrow();
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
            this.TF.DOMove(oldTrashBinPos, 0.2f);
        }

    }

}
