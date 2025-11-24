using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Link;

namespace LawNguyen.CookingGame.LRisottoY
{
    public class Oil : ItemMovingBase
    {

        public enum State {idle, during };
       State state;
       [SerializeField]   Animation animation;



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
                    animation.Play();   
                    break;
            default:
                    break;  
            
            
            }

        }


    }
}
