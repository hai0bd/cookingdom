using DG.Tweening;
using Link;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LawNguyen.CookingGame.LRisottoY
{
    public class Chesseshre : ItemMovingBase
    {
        public enum State { idle, inpot,done }
        State state = State.idle;

        public override bool IsCanMove => state == State.idle;
        

        public override void ChangeState<T>(T t)
        {

            this.state = (State)(object)t;
            switch (state)
            {
              case State.idle:  
                    break;  
                    case State.inpot:
                    break;
                    case State.done: 
                    this.gameObject.SetActive(false);   
                    break; 
                    default:
                    break;
            
            }

        }

        public override void OnBack()
        {
            base.OnBack();
        }
        public override void OnDrop()
        {
            base.OnDrop();
            OnBack();

        }


    }
}

