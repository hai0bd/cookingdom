using Link;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace HoangLinh.Cooking.Test
{
    public class WaterJug : ItemMovingBase
    {
        public enum State
        {
            HaveWater,
            Pouring,
            Done
        }

        [SerializeField] private State state;

        [SerializeField] Animation anim;
        [SerializeField] string animPouring, animIdle;
        [SerializeField] ItemAlpha waterLayerAlpha;

        public override bool IsState<T>(T t)
        {
            return state == (State)(object)t;
        }

        public override void ChangeState<T>(T t)
        {
            state = (State)(object)t;

            switch (state)
            {
                case State.HaveWater:
                    break;
                case State.Pouring:
                    anim.Play(animPouring);
                    OrderLayer = 49;
                    DOVirtual.DelayedCall(1.2f, () =>
                    {
                        ChangeState(State.Done);
                        this.OnBack();
                    });
                    break;
                case State.Done:
                    break;
            }
        }
    }
}