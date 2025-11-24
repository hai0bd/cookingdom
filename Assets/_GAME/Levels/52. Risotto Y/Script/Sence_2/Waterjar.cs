using DG.Tweening;
using Link;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LawNguyen.CookingGame.LRisottoY
{
    public class Waterjar : ItemMovingBase
    {

        public enum State
        {
            idle,
            during,
            end,


        }
        public State state = State.idle;
        public Animation animationduring;

        public override bool IsCanMove => state!=State.during;


      

        public override void OnBack()
        {
            base.OnBack();
        }
        public override void OnDrop()
        {
            base.OnDrop();
            OnBack();

        }


        public override void ChangeState<T>(T t)
        {
            this.state = (State)(object)t;

            switch (state) 
            { 
                case State.idle:

                    break;
                case State.during:
                    animationduring.Play();
                    SoundControl.Ins.PlayFX(SoundFXEnum.Soundname.waterpour);
                    break;
                 case State.end:
                    SoundControl.Ins.StopFX(SoundFXEnum.Soundname.waterpour);
                    OnBack();
                    OnDone();
                    break;
                 default:
                    break;
            
            }
        }





    }
}

