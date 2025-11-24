using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Link.Cooking.Spageti
{
    public class Bowl : ItemMovingBase
    {
        public enum State { Normal, Pouring, Done }
        [SerializeField] State state;
        [SerializeField] Animation pouring;
        
        public override bool IsCanMove => IsState(State.Normal);

        public override void ChangeState<T>(T t)
        {
           state = (State)(object)t;

           switch (state)
           {
               case State.Normal:
                   //active anim
                   break;
               case State.Pouring:
                   //active anim
                    pouring.Play();
                    SoundControl.Ins.PlayFX(Fx.Pour);
                    DOVirtual.DelayedCall(1.5f, ()=> ChangeState(State.Done));
                    OnDone();
                   break;
               case State.Done:
                   OnBack();
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
            OnBack();
        }
    }
}