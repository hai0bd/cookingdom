using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link.Cooking.Lobster
{
    public class PotLid : ItemMovingBase
    {
        [SerializeField] Animation anim;
        [SerializeField] ParticleSystem puff;

        public enum State { Open, Freeze, Stream, StreamOpen , Done }
        State state = State.Open;
        public override bool IsCanMove => IsState(State.Open, State.Stream, State.StreamOpen);

        public override bool IsDone => IsState(State.Done);

        public override void OnClickDown()
        {
            base.OnClickDown();
            if (IsState(State.Stream))
            {
                puff.Play();
                anim.Stop();
                SoundControl.Ins.StopFX(LevelStep_1.Fx.StreamLid);
            }
            SoundControl.Ins.PlayFX(LevelStep_1.Fx.Click);
        }

        public override void OnClickTake()
        {
            base.OnClickTake();
            OrderLayer = 25;
            SoundControl.Ins.PlayFX(LevelStep_1.Fx.Take);
        }

        public override void ChangeState<T>(T t)
        {
            state = (State)(object)t;
            switch (state)
            {
                case State.Open:
                    break;
                case State.Freeze:
                    break;
                case State.Stream:
                    DOVirtual.DelayedCall(0.15f, () => {
                        if (LevelControl.Ins.IsHaveObject<Lobster>(TF.position))
                        {
                            anim.Play();
                            SoundControl.Ins.PlayFX(LevelStep_1.Fx.StreamLid, true);
                        }
                        else
                        {
                            ChangeState(State.Done);
                        }
                    });
                    break;
                case State.Done:
                    LevelControl.Ins.CheckStep();
                    break;
                case State.StreamOpen:
                    break;
                default:
                    break;
            }
        }

        public override bool IsState<T>(T t)
        {
            return state == (State)(object)t;
        }

        public override void OnDrop()
        {
            base.OnDrop();
            IfChangeState(State.Stream, State.StreamOpen);
            if(IsState(State.Open)) LevelControl.Ins.SetStep(LevelName.Lobster, Step.OpenLid_1);
            if(IsState(State.StreamOpen)) LevelControl.Ins.SetStep(LevelName.Lobster, Step.OpenLid_2);
        }

    }
}