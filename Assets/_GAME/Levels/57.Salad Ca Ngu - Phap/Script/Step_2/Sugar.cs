using DG.Tweening;
using Link;
using UnityEngine;

namespace HuyThanh.Cooking.TunaSaladFrench
{
    public class Sugar : ItemIdleBase
    {
        public enum State
        {
            Normal,
            Opening,
            Opened,
            Closed
        }

        [SerializeField] private State state;

        [SerializeField] private SugarSpoon spoon;
        [SerializeField] private Animation anim;
        [SerializeField] private string animOpening, animClosing;

        [SerializeField] SpriteRenderer napSugar;

        public override bool IsState<T>(T t)
        {
            return state == (State)(object)t;
        }

        public override void ChangeState<T>(T t)
        {
            state = (State)(object)t;
            switch (state)
            {
                case State.Normal:
                    break;
                case State.Opening:
                    napSugar.sortingOrder = 2;
                    anim.Play(animOpening);
                    spoon.PlayStart();
                    DOVirtual.DelayedCall(1f, () => spoon.ChangeState(SugarSpoon.State.Normal));
                    break;
                case State.Closed:
                    napSugar.sortingOrder = 0;
                    DOVirtual.DelayedCall(.3f, () => anim.Play(animClosing));
                    break;
            }
        }

        public override void OnClickDown()
        {
            if (IsState(Sugar.State.Normal))
            {
                ChangeState(Sugar.State.Opening);
                SoundControl.Ins.PlayFX(Fx.Click);
                return;
            }
            base.OnClickDown();
        }
    }
}