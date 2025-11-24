using Link;
using UnityEngine;

namespace HuyThanh.Cooking.Burrito
{
    public class CornCob : ItemMovingBase
    {
        public override bool IsCanMove => true;

        [SerializeField] TrashBin trashBin;
        [SerializeField] CornCutItem cornCutItem;

        public override void OnClickDown()
        {
            base.OnClickDown();
            trashBin.OnNeedTrashBin();
        }

        public override void OnDrop()
        {
            SoundControl.Ins.PlayFX(Fx.PutDown);
            base.OnDrop();
            trashBin.OnNoNeedTrashBin();
        }

        public void ActiveCollider()
        {
            collider.enabled = true;
            OnSavePoint();
        }

        public void OnThrow()
        {
            SoundControl.Ins.PlayFX(Fx.PutDown);
            cornCutItem.OnDoneCornThrow();
        }

    }

}