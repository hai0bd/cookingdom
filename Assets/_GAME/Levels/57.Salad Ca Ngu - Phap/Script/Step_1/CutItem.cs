using DG.Tweening;
using Link;
using UnityEngine;

namespace HuyThanh.Cooking.TunaSaladFrench
{
    public class CutItem : ItemMovingBase
    {
        public enum State
        {
            Normal,
            Cutting,
            Spice,
            Done
        }
        [SerializeField] State state;

        [SerializeField] private GameObject rawItem, cuttingItem, doneCutItem;

        [SerializeField] Sprite hint;
        public override bool IsCanMove => IsState(CutItem.State.Normal, CutItem.State.Spice);

        public override void ChangeState<T>(T t)
        {
            state = (State)(object)t;

            switch (state)
            {
                case State.Normal:
                    break;
                case State.Cutting:

                    DOVirtual.DelayedCall(.8f, () =>
                    {
                        rawItem.SetActive(false);
                    });

                    DOVirtual.DelayedCall(1f, () =>
                    {
                        cuttingItem.SetActive(true);
                    });
                    DOVirtual.DelayedCall(1.2f, () =>
                    {
                        cuttingItem.SetActive(false);
                        doneCutItem.SetActive(true);
                    });
                    DOVirtual.DelayedCall(1.4f, () =>
                    {
                        ChangeState(CutItem.State.Spice);
                    });
                    break;
                case State.Spice:
                    break;
                case State.Done:
                    break;
            }
        }

        public override bool IsState<T>(T t)
        {
            return state == (State)(object)t;
        }

        public override bool OnTake(IItemMoving item)
        {
            if (item is Knife && IsState(CutItem.State.Normal) && LevelControl.Ins.IsHaveObject<CuttingBoard>(TF.position) != null)
            {
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                item.ChangeState(Knife.State.Cutting);
                ChangeState(CutItem.State.Cutting);
                return true;
            }
            return base.OnTake(item);
        }

        public override void OnClickDown()
        {
            base.OnClickDown();
            LevelControl.Ins.SetHint(hint);
            SoundControl.Ins.PlayFX(Fx.Click);
        }

        public override void OnClickTake()
        {
            base.OnClickTake();
            SoundControl.Ins.PlayFX(Fx.Take);
        }
    }
}