using DG.Tweening;
using Link;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HuyThanh.Cooking.TunaSaladFrench
{
    public class TunaCap : ItemMovingBase
    {
        public enum State { Normal, DropCap, Trash, Done }
        [SerializeField] State state;

        [BoxGroup("Anim Name")][SerializeField] Animation anim;
        [BoxGroup("Anim Name")][SerializeField] string animDropCap;

        public override bool IsCanMove => IsState(TunaCap.State.Trash);

        ///ban dau collider = false de cho khong tuong tac duoc
        public override void ChangeState<T>(T t)
        {
            state = (State)(object)t;

            switch (state)
            {
                case State.DropCap:

                    anim.Play(animDropCap);
                    DOVirtual.DelayedCall(1, () =>
                    {
                        ChangeState(TunaCap.State.Trash);
                    });

                    break;
                case State.Trash:

                    collider.enabled = true;

                    break;
                case State.Done:
                    collider.enabled = false; ///vut di roi thi tat box
                    LevelControl.Ins.CheckStep(1f);
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
            SoundControl.Ins.PlayFX(Fx.Click);
        }
        public override void OnClickTake()
        {
            base.OnClickTake();
            SoundControl.Ins.PlayFX(Fx.Take);
        }
    }

}

