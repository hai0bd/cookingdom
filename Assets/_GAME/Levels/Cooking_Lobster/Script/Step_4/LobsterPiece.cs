using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link.Cooking.Lobster
{
    public class LobsterPiece : ItemMovingBase
    {
        public enum State { Slice, Normal, InPlate, Decore, Done }
        [SerializeField] State state;
        [SerializeField] Animation anim;
        [SerializeField] SpriteRenderer[] sprites;

        [field:SerializeField] public DecoreItem.NameType Name { get; private set; }

        public override bool IsCanMove => IsState(State.Normal, State.Decore);

        public override void ChangeState<T>(T t)
        {
            state = (State)(object)t;

            switch (state)
            {
                case State.Slice:
                    anim.Play("LobsterScale");
                    break;
                case State.Normal:
                    break;
                case State.InPlate:
                    sprites[0].DOFade(0, 0.2f);
                    sprites[1].DOFade(1, 0.2f);
                    sprites[2].DOFade(1, 0.2f);
                    OrderLayer = -20;
                    break;
                case State.Decore:
                    break;
                case State.Done:
                    //anim.Play("IdleItemAppear");
                    break;
     
                default:
                    break;
            }
        }

        public override void OnDone()
        {
            base.OnDone();
            ChangeState(State.Done);
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
            SoundControl.Ins.PlayFX(LevelStep_1.Fx.Take);
        }
    }
}