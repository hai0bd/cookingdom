using DG.Tweening;
using Link;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HuyThanh.Cooking.TunaSaladFrench
{
    public class Pestle : ItemMovingBase
    {
        public enum State { Normal, Mixing, Done }
        [SerializeField] State state;

        [BoxGroup("Anim Name")][SerializeField] Animation anim;
        [BoxGroup("Anim Name")][SerializeField] string animPestle;

        [SerializeField] Vector3 rotOnClick;
        [SerializeField] bool IsMixAnim = false;

        public override bool IsCanMove => IsState(Pestle.State.Normal);

        public override void ChangeState<T>(T t)
        {
            state = (State)(object)t;

            switch (state)
            {
                case State.Normal:
                    break;
                case State.Mixing:
                    anim.Play(animPestle);
                    foreach (AnimationState state in anim)
                    {
                        state.speed = 0;
                    }

                    break;
                case State.Done:
                    OnBack();
                    break;
            }
        }

        Tween current_tween = null;
        public void PlayAnimOnClick()
        {

            if (current_tween != null)
                current_tween.Kill();

            foreach (AnimationState state in anim)
            {
                state.speed = 1;
            }

            current_tween =
            DOVirtual.DelayedCall(0.2f, () =>
            {
                foreach (AnimationState state in anim)
                {
                    state.speed = 0;
                }
            });
        }

        public override bool IsState<T>(T t)
        {
            return state == (State)(object)t;
        }

        public override void OnClickDown()
        {
            base.OnClickDown();
            SoundControl.Ins.PlayFX(Fx.Click);
            TF.DORotate(rotOnClick, 0.1f);
        }

        public override void OnDrop()
        {
            base.OnDrop();
            OnBack();
            OrderLayer = 0;
        }
    }

}
