using Link;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hai.Cooking.NewTest
{
    public class BowlItem : ItemMovingBase
    {
        public enum State
        {
            Normal,
            Pouring,
            WaitingCookSpice,
            Done
        }

        [SerializeField] private State state;
        [SerializeField] private HintText hintText;

        public override bool IsCanMove => IsState(State.Normal);

        public override bool IsState<T>(T t)
        {
            return state == (State)(object)t;
        }

        public override void ChangeState<T>(T t)
        {
            state = (State)(object)t;
            switch(state)
            {
                case State.Normal:
                    break;
                case State.Pouring:
                    StartPouring();
                    break;
                case State.WaitingCookSpice:
                    OnBack();
                    break;
                case State.Done:
                    NextHint();
                    break;
            }
        }

        private void NextHint()
        {
            //SoundControl.Ins.PlayFX(Fx.DoneSomething);
            hintText.OnActiveHint();
            LevelControl.Ins.NextHint();
        }

        private void StartPouring()
        {
            //SoundControl.Ins.PlayFX(Fx.BeanPouring);
            StartCoroutine(WaitForPouring());
        }

        private IEnumerator WaitForPouring()
        {
            yield return WaitForSecondCache.Get(1.2f);
            ChangeState(State.WaitingCookSpice);
        }
    }
}