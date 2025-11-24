using DG.Tweening;
using Link;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LawNguyen.CookingGame.LRisottoY
{
    public class Parsley : ItemMovingBase
    {
        public enum State { idle, inbroad,Peel, minece, done }
        public State state=State.idle;
        [SerializeField] GameObject pasleydirty, pasleydirty1;
        [SerializeField ] Animation animation;
        [SerializeField ] GameObject pasleyfisst,pasleydone;
        [SerializeField] Sprite HintParsley;
        public override bool IsCanMove => !IsState(State.inbroad) && !IsState(State.Peel);
        int clickCount = 0;
        int clickThreshold = 5;

        public override bool IsState<T>(T t)
        {
            return this.state == (State)(object)t;
        }

        public override bool OnTake(IItemMoving item)
        {
            if( item is Knift && IsState(State.Peel))
            {   item.SetOrder(51);
                
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                item.ChangeState(Knift.State.CutPasley);

                SoundControl.Ins.PlayFX(SoundFXEnum.Soundname.beardCuting1);
                DOVirtual.DelayedCall(2.42f, () => { 
                    pasleyfisst.SetActive(false);
                    pasleydone.SetActive(true);
                    item.OnBack();
                    ChangeState(State.minece);

                });
                return true;

            }


            return false;
        }


        public override void ChangeState<T>(T t)
        {
            this.state = (State)(object)t;

            switch (state)
            {
                case State.idle:
                    break;
                case State.inbroad:
                    break;
                case State.minece:
                    break;
                    case State.Peel:
                    OrderLayer = 0;
                        break;  
              
                case State.done:
                    break;

                default:
                    break;
            }

        }
        public override void OnClickDown()
        {
            base.OnClickDown();
            if (HintParsley != null) LevelControl.Ins.SetHint(HintParsley);
           

            if (state != State.inbroad)
            {
                
                return;
            }

            clickCount++;
            transform.DOShakeScale(0.2f, 0.3f, 10, 90f, false);
            SoundControl.Ins.PlayFX(SoundFXEnum.Soundname.graliccracking);
            if (clickCount == 3)
            {
                pasleydirty.SetActive(false);
                animation.Play();
            }

            if (clickCount >= clickThreshold)
            {
                animation.Play();
                pasleydirty1.SetActive(false);
                transform.DOScale(Vector3.one, 0.2f);
                SetOrder(0);
                ChangeState(State.Peel);
            }

        }





    }
}
