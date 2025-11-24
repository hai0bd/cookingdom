using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link.Cooking.Lobster
{
    public class Wine : ItemMovingBase
    {
        public enum State { Normal, Pouring, Done }
        [SerializeField] State state;
        [SerializeField] Animation anim;
        [SerializeField] SpriteRenderer cap;

        public override bool IsCanMove => IsState(State.Normal);

        public override void ChangeState<T>(T t)
        {
            state = (State)(object)t;
            switch (state)
            {
                case State.Normal:
                    break;
                case State.Pouring:
                    anim.Play("WinePour");
                    SoundControl.Ins.PlayFX(LevelStep_1.Fx.Pour);
                    DOVirtual.DelayedCall(1.3f, () => ChangeState(State.Done));
                    break;
                case State.Done:
                    OnBack();
                    OrderLayer = -10;
                    break;
                default:
                    break;
            }
        }
        public override bool IsState<T>(T t)
        {
            return state == (State)(object)t;
        }

        public override void OnClickDown()
        {
            base.OnClickDown();
            cap.transform.DOLocalMoveY(1.35f, 0.3f);
            cap.DOFade(0, .3f);
            SoundControl.Ins.PlayFX(LevelStep_1.Fx.Click);
        }

        public override void OnClickTake()
        {
            base.OnClickTake();
            SoundControl.Ins.PlayFX(LevelStep_1.Fx.Take);
        }

        public override void OnDrop()
        {
            base.OnDrop();
            OnBack();
        }
    }
}