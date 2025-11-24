using DG.Tweening;
using Link;
using UnityEngine;

namespace HuyThanh.Cooking.Burrito
{
    public class BurritoDoneCut : ItemMovingBase
    {
        [SerializeField] ItemAlpha burritoFront, burritoBack;
        [SerializeField] ItemAlpha shadowFront, shadowBack;

        [SerializeField] DecorItemType type;
        [SerializeField] PlateDecor plateDecor;

        public bool IsType(DecorItemType type)
        {
            return type == this.type;
        }

        public void RemoveFromTarget()
        {
            plateDecor.Remove(type);
        }

        private void ShowFront(bool isFront)
        {
            burritoFront.DOKill();
            burritoBack.DOKill();

            burritoFront.DoAlpha(isFront ? 1 : 0, 0.1f);
            shadowFront.DoAlpha(isFront ? 0.4f : 0, 0.1f);
            burritoBack.DoAlpha(isFront ? 0 : 1, 0.1f);
            shadowBack.DoAlpha(isFront ? 0 : 0.4f, 0.1f);
        }

        public override void OnClickDown()
        {
            base.OnClickDown();
            SoundControl.Ins.PlayFX(Fx.Click);
            ShowFront(isFront: true);
        }

        public override void OnDrop()
        {
            base.OnDrop();
            SoundControl.Ins.PlayFX(Fx.Take);
            ShowFront(isFront: false);
        }
    }

}