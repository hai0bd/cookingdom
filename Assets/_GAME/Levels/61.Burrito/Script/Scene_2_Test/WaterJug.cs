using DG.Tweening;
using HuyThanh.Cooking.Burrito;
using Link;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hai.Cooking.Burrito
{
    public class WaterJug : ItemMovingBase
    {
        public enum State
        {
            HaveWater,
            Pouring,
            Done
        }

        public override bool IsCanMove => IsState(State.HaveWater);
        [SerializeField] private State state;

        [SerializeField] private Animation anim;
        [SerializeField] private string animPouring, animIdle;
        [SerializeField] private ItemAlpha waterLayerAlpha;

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
                    SoundControl.Ins.PlayFX(Fx.WaterPouring);
                    OrderLayer = 49;
                    DOVirtual.DelayedCall(1.2f, () =>
                    {
                        ChangeState(State.Done);
                        OnBack();
                    });
                    break;
                case State.Done:
                    break;
            }
        }

        public void Refill()
        {
            TF.DOMove(TF.position + Vector3.right * 0.5f, 0.5f).OnComplete(() =>
            {
                waterLayerAlpha.DoAlpha(1, 0.1f);
                TF.DOMove(TF.position + Vector3.left * 5f, 0.5f);
                ChangeState(State.HaveWater);
                anim.Play(animIdle);
            });
        }

        public override void OnClickDown()
        {
            base.OnClickDown();
            SoundControl.Ins.PlayFX(Fx.Click);
        }

        public override void OnClickTake()
        {
            base.OnClickTake();
            OrderLayer = 51;
        }

        public override void OnDrop()
        {
            base.OnDrop();
            SoundControl.Ins.PlayFX(Fx.PutDown);
        }
    }
}
