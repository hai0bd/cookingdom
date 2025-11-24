using DG.Tweening;
using Link;
using UnityEngine;

namespace HuyThanh.Cooking.TunaSaladFrench
{
    public class NapNoi : ItemMovingBase
    {
        public enum State
        {
            Cooking,
            Normal,
            Done,
        }

        [SerializeField] private State state;
        [SerializeField] private Pot pot;
        [SerializeField] Stove stove;

        private Tween stoveTween;

        public override bool IsCanMove => IsState(NapNoi.State.Normal);

        public override bool IsState<T>(T t)
        {
            return state == (State)(object)t;
        }

        public override void ChangeState<T>(T t)
        {
            state = (State)(object)t;

            switch (state)
            {
                case NapNoi.State.Cooking:
                    DOVirtual.DelayedCall(4f, () => ChangeState(NapNoi.State.Normal));
                    break;
            }
        }
        public override void OnClickDown()
        {
            base.OnClickDown();
            if (stoveTween != null && stoveTween.IsActive() && stoveTween.IsPlaying())
            {
                stoveTween.Kill();
            }
            SoundControl.Ins.PlayFX(Fx.Click);
            DOVirtual.DelayedCall(0.2f, () =>
            {
                pot.ChangeSteam(true);
            });
        }

        public override void OnDrop()
        {
            base.OnDrop();

            DOVirtual.DelayedCall(0.19f, () =>
            {
                pot.ChangeSteam();
            });
        }

        public override void OnBack()
        {
            base.OnBack();
            stoveTween = DOVirtual.DelayedCall(0.3f, () =>
            {
                stove.OnTake(this);
            });
        }
        public override void OnMove(Vector3 pos, Quaternion rot, float time)
        {
            base.OnMove(pos, rot, time);

            DOVirtual.DelayedCall(time, () =>
            {
                pot.ChangeSteam();
            });
        }

        public override void OnClickTake()
        {
            base.OnClickTake();
            SoundControl.Ins.PlayFX(Fx.Take);
        }
    }
}