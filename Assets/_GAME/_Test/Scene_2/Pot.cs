using Link;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoangLinh.Cooking.Test
{
    public class Pot : ItemIdleBase
    {
        public enum State
        {
            Normal,
            HaveWater,
            HaveItem,
            Cooking,
            DoneCook,
            Done
        }

        [SerializeField] private State state;

        [SerializeField] ItemAlpha waterLayerAlpha;
        [SerializeField] ItemAlpha boilWaterAlpha;

        [SerializeField] ParticleSystem boilVFX;
        [SerializeField] ParticleSystem steamVFX;

        [SerializeField] ClockTimer _clockTimer;

        [SerializeField] WaterJug waterJug;

        [SerializeField] Transform potItem;

        private BowlPotItem bowlPotItem;

        private void OnEnable()
        {
            _clockTimer._OnTimeOut.AddListener(() =>
            {
                if (IsState(State.Cooking))
                {
                    ChangeState(State.DoneCook);
                }
            });
        }

        private void OnDisable()
        {
            _clockTimer._OnTimeOut.RemoveListener(() =>
            {
                if (IsState(State.Cooking))
                {
                    ChangeState(State.DoneCook);
                }
            });
        }

        public override bool IsState<T>(T t)
        {
            return state == (State)(object)t;
        }

        public override void ChangeState<T>(T t)
        {
            state = (State)(object)t;

            switch (state)
            {
                case State.Normal:
                    break;
                case State.HaveWater:
                    waterLayerAlpha.DoAlpha(1f, 2f);
                    break;
                case State.HaveItem:
                    potItem.localScale = Vector3.one;
                    break;
                case State.Cooking:
                    boilWaterAlpha.DoAlpha(1f, 1f);
                    boilVFX.Play();
                    _clockTimer.Show(5f);
                    break;
                case State.DoneCook:
                    boilVFX.Stop();
                    steamVFX.Play();
                    break;
            }
        }

        public override bool OnTake(IItemMoving item)
        {
            if(item is WaterJug && this.IsState(State.Normal) && item.IsState(WaterJug.State.HaveWater))
            {
                item.ChangeState(WaterJug.State.Pouring);
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                
                ChangeState(Pot.State.HaveWater);
                return true;
            }

            if (item is BowlPotItem && item.IsState(BowlPotItem.State.Normal) && this.IsState(State.HaveWater))
            {
                bowlPotItem = item as BowlPotItem;

                item.ChangeState(BowlPotItem.State.Pouring);
                item.OnMove(potItem.position, Quaternion.identity, 0.2f);

                ChangeState(Pot.State.HaveItem);
                return true;
            }

            return base.OnTake(item);
        }

        public void OnClickButton(bool isOn)
        {
            if (isOn && IsState(Pot.State.HaveItem))
            {
                ChangeState(Pot.State.Cooking);
            }
            if (isOn == false && IsState(Pot.State.DoneCook) && bowlPotItem == null)
            {
                ChangeState(Pot.State.Done);
            }
        }





    }
}