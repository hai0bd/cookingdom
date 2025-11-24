using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link.Cooking.Lobster
{
    public class Knife : ItemMovingBase
    {
        public enum State { Ready, Cutting, Done }
        [SerializeField, ReadOnly] State state;
        [SerializeField] ParticleSystem slashVFX;
        [SerializeField] Animation animation;

        public override bool IsCanMove => IsState(State.Ready);

        public override void ChangeState<T>(T t)
        {
            state = (State)(object)t;

            switch (state)
            {
                case State.Ready:
                    animation.Play("KnifeFinish");
                    break;
                case State.Cutting:
                    animation.Play("KnifeStart");
                    slashVFX.Play();
                    OrderLayer = 50;
                    //delay
                    //anim cut
                    //cut xong back
                    SoundControl.Ins.PlayFX(LevelStep_1.Fx.KnifeCut);
                    //DOVirtual.DelayedCall(0.5f, () =>
                    //{ 
                    //});

                    DOVirtual.DelayedCall(1.4f, () => {
                        ChangeState(State.Ready);
                        OnDrop();
                    });
                    break;
                case State.Done:
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
            SoundControl.Ins.PlayFX(LevelStep_1.Fx.Click);
        }

        public override void OnClickTake()
        {
            base.OnClickTake();
            OrderLayer = 50;
        }

        public override void OnDrop()
        {
            base.OnDrop();
            OnBack();
        }
    }
}