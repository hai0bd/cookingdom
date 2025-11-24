using Link;
using UnityEngine;

namespace HuyThanh.Cooking.Burrito
{
    public class GarlicRotten : ItemMovingBase
    {
        public override bool IsCanMove => true;

        [SerializeField] TrashBin trashBin;
        [SerializeField] Garlic garlic;

        public override void OnClickDown()
        {
            SoundControl.Ins.PlayFX(Fx.Click);
            base.OnClickDown();
            trashBin.OnNeedTrashBin();
        }

        public override void OnDrop()
        {
            base.OnDrop();
            trashBin.OnNoNeedTrashBin();
        }

        public void OnThrow()
        {
            garlic.OnDoneThrowRotten();
        }
    }
}