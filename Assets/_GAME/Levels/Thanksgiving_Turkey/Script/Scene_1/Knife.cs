using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link.Turkey
{
    public class Knife : ItemMovingBase
    {
        [SerializeField] private AnimationClip blunt, sharping, sharp, moving, cutting;
        [SerializeField] private Animation animation;
        [SerializeField] private ParticleSystem sharpVfx;
        public enum State { Blunt, Sharping, Sharp, Cutting, }
        State state = State.Blunt;

        public override bool IsCanMove => state != State.Sharping && state != State.Cutting;

        public override void ChangeState<T>(T t)
        {
            this.state = (State)(object)(t);
            switch (state)
            {
                case State.Blunt:
                    ChangeAnim(blunt.name);
                    break;

                case State.Sharping:
                    ChangeAnim(sharping.name);
                    break;

                case State.Sharp:
                    ChangeAnim(sharp.name);
                    break;

                case State.Cutting:
                    ChangeAnim(cutting.name);
                    break;
                default:
                    break;
            }
            //Debug.Log("Knife : " + state);
        }
        public override bool IsState<T>(T t)
        {
            return this.state == (State)(object)(t);
        }
        public void OnSharping(float v)
        {
            DOVirtual.DelayedCall(v, ()=>
                    {
                        ChangeState(State.Sharping);
                        SoundControl.Ins.PlayFX(LevelStep_1.FX.Sharper, true);
                        DOVirtual.DelayedCall(1.5f, () =>
                        {
                            SoundControl.Ins.StopFX(LevelStep_1.FX.Sharper);
                            ChangeState(State.Sharp);
                            sharpVfx.Play();
                            OnBack();
                        });
                    });
        }

        public override void OnClickDown()
        {
            base.OnClickDown();
            ChangeAnim(moving.name);
            SoundControl.Ins.PlayFX(LevelStep_1.FX.Hold);
        }

        public override void OnClickTake()
        {
            base.OnClickTake();
            //if (IsState(State.Blunt))
            //{
            //    ChangeAnim(blunt.name);
            //}
            //else
            //{
            //    ChangeAnim(sharp.name);
            //}
            OrderLayer = 12;
            SoundControl.Ins.PlayFX(LevelStep_1.FX.Put);
        }

        public override void OnBack()
        {
            base.OnBack();
            OnDrop();
        }

        public override void OnDrop()
        {
            base.OnDrop();
            if (IsState(State.Blunt))
            {
                ChangeAnim(blunt.name);
            }
            else
            {
                ChangeAnim(sharp.name);
            }
            OrderLayer = 12;
            SoundControl.Ins.PlayFX(LevelStep_1.FX.Put);
        }

        private void ChangeAnim(string anim)
        {
            //Debug.Log(anim);
            animation.Play(anim);
        }
    }
}