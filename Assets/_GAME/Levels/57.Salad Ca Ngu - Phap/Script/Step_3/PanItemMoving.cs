using DG.Tweening;
using Link;
using UnityEngine;

namespace HuyThanh.Cooking.TunaSaladFrench
{
    public class PanItemMoving : ItemMovingBase
    {
        public enum ItemType
        {
            None,
            Butter,
            Pearl
        }

        [SerializeField] private ItemType type;

        [SerializeField] private Transform secondTarget;
        [SerializeField] Vector3 secondTargetPosition;
        [SerializeField] Vector3 oldTargetPosition;
        public override bool IsDone => !collider.enabled;

        public ItemType GetType()
        {
            return this.type;
        }

        public override void OnDone()
        {
            base.OnDone();
        }

        public override void OnClickDown()
        {
            base.OnClickDown();
            secondTarget.DOLocalMove(secondTargetPosition, 0.2f);
            SoundControl.Ins.PlayFX(Fx.Click);
        }

        public override void OnClickTake()
        {
            base.OnClickTake();
            SoundControl.Ins.PlayFX(Fx.Take);
        }

        public override void OnBack()
        {
            base.OnBack();
            secondTarget.DOLocalMove(oldTargetPosition, 0.2f);
        }

        public void PearlMove(Transform targetTF)
        {
            secondTarget.DOLocalMove(targetTF.localPosition, 0.2f);
            secondTarget.DOLocalRotate(targetTF.localRotation.eulerAngles, 0.2f);
        }
    }

}