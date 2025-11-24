using DG.Tweening;
using Link;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace LawNguyen.CookingGame.LRisottoY
{
    public class Glasssafron : ItemMovingBase
    {

        public enum State
        {
            idle,
            havewater,
            havewatersafron,
            done
        }
        [SerializeField] ItemAlpha water;
        [SerializeField] ItemAlpha watersafron;
        [SerializeField] GameObject safron;
        [SerializeField] Sprite HintGlasssafron;
        public State state=State.idle;

        public event Action<UnityEngine.Object> OnCompleteEvent;
        public override bool IsCanMove => IsState(State.idle);



        public override void OnDrop()
        {
            base.OnDrop();
            OrderLayer = 26;

        }
        public override bool IsState<T>(T t)
        {
            return this.state==(State)(object)t;
        }


        public override bool OnTake(IItemMoving item)
        {
            if(item is Safron && IsState(State.havewater) )
            {
              
                item.OnMove(TF.position + new Vector3(0,1f, 0),Quaternion.identity,1f);

                DOVirtual.DelayedCall(1f, () =>
                {
                   
                    item.ChangeState(Safron.State.dropsaforn);  
                });

                DOVirtual.DelayedCall(2f, () =>
                {
                    ChangeState(State.havewatersafron);
                    
                });

                return true;    
            }


            return false;
        }
        public override void OnClickDown()
        {
            base.OnClickDown();
            if (HintGlasssafron != null) LevelControl.Ins.SetHint(HintGlasssafron);
        }


        public override void ChangeState<T>(T t)
        {
            this.state=(State)(object)t;

            switch (state)
            {
                case State.idle:
                    break;
                case State.done:
                    OnDone();
                    break;
                case State.havewater:
                    water.DoAlpha(1, 1, 0);

                    break;
                case State.havewatersafron:
                    water.DoAlpha(0, 1, 0);
                    watersafron.DoAlpha(1, 2f, 0);
                    safron.SetActive(true);
                    OnCompleteEvent?.Invoke(this);
                  

                        break;
                default:
                    break;
            }
        }





    }
}
