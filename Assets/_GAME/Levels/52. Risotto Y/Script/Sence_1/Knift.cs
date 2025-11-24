using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Link;
namespace LawNguyen.CookingGame.LRisottoY
{
    public class Knift : ItemMovingBase
    {
        public enum State
        {
            idle,
            moving,
            CutOnion,
            MinceOnion,
            CutPasley,
            CutShrimp,
            CutGralic,
            MinceGralic

        }

        public State state = State.idle;
        [SerializeField] Animation animation;
        [SerializeField] AnimationClip idle, moving, CutOnion,MineOnion, CutPasley, CutShrimp, CutGralic ,Minegralic;
        public override bool IsCanMove => IsState(State.idle);


        public override void OnBack()
        {
            base.OnBack();
            ChangeState(State.idle);
        }
        public override void OnDrop()
        {
            base.OnDrop();
    
        }
        public override bool IsState<T>(T t)
        {
            return this.state==(State)(object)t;
        }

        public override void OnClickDown()
        {
            base.OnClickDown();
            ChangeState(State.moving);
        }


        public override void ChangeState<T>(T t)
        {
          this.state =(State)(object)t;

            switch (state)
            {
                case State.idle:
                    ChangeAnim(idle.name);
                    break;
                case State.moving:
                    ChangeAnim(moving.name);
                    break;
                case State.CutPasley:

                    ChangeAnim(CutPasley.name);
                    break;
                case State.CutOnion:
                    ChangeAnim(CutOnion.name);
                    break;
                    case State.MinceOnion:
                    ChangeAnim(MineOnion.name);
                        break;

                case State.CutShrimp:
                    ChangeAnim(CutShrimp.name);
                    break;
                case State.CutGralic:
                    ChangeAnim(CutGralic.name);
                    break;
                    case State.MinceGralic:
                    ChangeAnim(Minegralic.name);
                        break;
                default:
                    break;
            }
        }



        public void ChangeAnim(string name)
        {

            animation.Play(name);
        }




    }
}
