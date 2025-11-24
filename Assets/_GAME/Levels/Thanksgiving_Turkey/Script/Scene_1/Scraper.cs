using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link.Turkey
{
    public class Scraper : ItemMovingBase
    {
        [SerializeField] private AnimationClip idling, cutting;
        [SerializeField] private Animation animation;

        public enum State { Idling, Cutting, }
        State state = State.Idling;
        public override bool IsCanMove => state == State.Idling;

        public override void ChangeState<T>(T t)
        {
            this.state = (State)(object)(t);
            switch (state)
            {
                case State.Idling:
                    ChangeAnim(idling.name);
                    break;
                case State.Cutting:
                    ChangeAnim(cutting.name);
                    SoundControl.Ins.PlayFX(LevelStep_1.FX.Scrap, true);
                    DOVirtual.DelayedCall(2f, () =>
                    {
                        SoundControl.Ins.StopFX(LevelStep_1.FX.Scrap);
                        ChangeState(Scraper.State.Idling);
                        OnBack();
                    });
                    break;
            }
        }


        public override void OnBack()
        {
            base.OnBack();
            OnDrop();
        }

        public override void OnDrop()
        {
            base.OnDrop();
            SoundControl.Ins.PlayFX(LevelStep_1.FX.Put);
            OrderLayer = 13;
        }
        private void ChangeAnim(string anim)
        {
            animation.Play(anim);
        }

        public override void OnClickDown()
        {
            base.OnClickDown();
            SoundControl.Ins.PlayFX(LevelStep_1.FX.Hold);
        }

        public override void OnClickTake()
        {
            base.OnClickTake();
            SoundControl.Ins.PlayFX(LevelStep_1.FX.Put);
            OrderLayer = 13;
        }
    }
}