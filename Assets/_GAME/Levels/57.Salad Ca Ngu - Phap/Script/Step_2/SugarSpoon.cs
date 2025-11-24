using DG.Tweening;
using Link;
using UnityEngine;

namespace HuyThanh.Cooking.TunaSaladFrench
{
    public class SugarSpoon : ItemMovingBase
    {
        public enum State
        {
            Waiting,
            Normal,
            Pouring,
            GetBack,
            Done
        }

        [SerializeField] private State state;
        [SerializeField] private Animation anim;
        [SerializeField] private string animOpening, animPouring, animBack;
        [SerializeField] private Sugar sugar;
        public override bool IsCanMove => IsState(SugarSpoon.State.Normal);

        public override bool IsState<T>(T t)
        {
            return state == (State)(object)t;
        }

        public void PlayStart()
        {
            anim.Play(animOpening);
        }
        public override void ChangeState<T>(T t)
        {
            state = (State)(object)t;
            switch (state)
            {
                case State.Normal:
                    OnSave(0.1f);
                    break;
                case State.Pouring:
                    anim.Play(animPouring);
                    DOVirtual.DelayedCall(1.5f, () => ChangeState(SugarSpoon.State.GetBack));
                    break;
                case State.GetBack:
                    OnBack();
                    DOVirtual.DelayedCall(.3f, () => anim.Play(animBack));
                    sugar.ChangeState(Sugar.State.Closed);
                    break;
            }
        }

        public override void OnClickDown()
        {
            base.OnClickDown();
            SoundControl.Ins.PlayFX(Fx.Click);
        }

        public override void OnClickTake()
        {
            base.OnClickTake();
            SoundControl.Ins.PlayFX(Fx.Take);
        }
    }
}