using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Link;
namespace LawNguyen.CookingGame.LRisottoY
{
    public class Safron : ItemMovingBase
    {
        public enum State
        {
            idle,
            dropsaforn
        }
        public State state;
        [SerializeField] Animation animation;
        [SerializeField] Sprite HintSafron;
        public override bool IsCanMove => state==State.idle;

        public override void ChangeState<T>(T t)
        {
            this.state=(State)(object)t;
            switch (state)
            {
                case State.idle:
                    break;
                case State.dropsaforn:
                    animation.Play();
                    break;
                default:
                    break;
            }

        }
        public override void OnClickDown()
        {
            base.OnClickDown();
            if (HintSafron != null) LevelControl.Ins.SetHint(HintSafron);

        }



    }
}

