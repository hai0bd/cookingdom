using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Link;
using Unity.VisualScripting;
namespace LawNguyen.CookingGame.LRisottoY
{
    public class Watercover : ItemMovingBase
    {  

        public enum State
        {
            move,
            dontmove
        }
        State state= State.move;    
        [SerializeField] WaterSink waterSink;
        public override bool IsCanMove => state==State.move;
        public override bool OnTake(IItemMoving item)
        {
            return false;
        }
        public override void OnClickTake()
        {
            if (state == State.dontmove)
            {
                // Có thể phát hiệu ứng cảnh báo tại đây nếu muốn
                Debug.Log("🚫 Không thể mở nắp khi gạo chưa vo xong!");
                return;
            }

            base.OnClickTake();
            waterSink.OnTake(this);
        }

        public override void OnDrop()
        {
           
            OnClickTake();
        }


        public override void ChangeState<T>(T t)
        {
            this.state = (State)(object)t;
            switch (state)
            {
                case State.move:
                    break;
                case State.dontmove:
                    break;
                default:
                    break;
            }

        }

    }
}
