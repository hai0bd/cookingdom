using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Link.Cooking.Spageti
{
    public class Noodle : ItemMovingBase
    {
        public enum State { None, InPot, Cooking, Stream, Cooked, InScoop, InDish }
        public State state = State.None;
        public override bool IsCanMove => IsState(State.None);

        [SerializeField] ItemAlpha noodleRaw, noodleStream, noodleScoop;
        [SerializeField] Animation anim, animScale;
        [SerializeField] GameObject smoke;

        [SerializeField] ActionBase plate, dish;
        [SerializeField] HintText hintText_1, hintText_2;

        public override bool IsDone => IsState(State.InDish);
        
        public override void ChangeState<T>(T t)
        {
           state = (State)(object)t;
           switch (state)
           {
               case State.InPot:
                    //active anim
                    DOVirtual.DelayedCall(0.4f, ()=>
                    {
                        anim.Play("Noodle Fall");
                    });
                    hintText_1.OnActiveHint();
                    break;
                case State.Cooking:
                    //chuyen trang thai
                    anim.Play("Noodle Stream");
                    plate.Active();
                    dish.Active();
                    break;
                case State.Stream:
                    //chuyen trang thai
                    StartCoroutine(IEStream());
                    break;     
                    case State.InScoop:
                    //chuyen trang thai
                    noodleStream.SetAlpha(0);
                    noodleScoop.SetAlpha(1);
                    smoke.SetActive(true);
                    animScale.Play();
                    TF.DOScale(0.8f, 0.2f);
                    break;
                case State.Cooked:
                    //chuyen trang thai
                    SoundControl.Ins.PlayFX(Fx.Ping);
                    break;

                case State.InDish:
                    animScale.Play();
                    OnDone();
                    //DOVirtual.DelayedCall(0.2f, OnDone);
                    //chuyen trang thai
                    hintText_2.OnActiveHint();

                    break;
            }
        }

        public override bool IsState<T>(T t)
        {
           return state == (State)(object)t;
        }

        public override void OnDone()
        {
            base.OnDone();
            LevelControl.Ins.CheckStep(1f);
        }

        public override void OnDrop()
        {
            base.OnDrop();
            OnBack();
        }

        private IEnumerator IEStream()
        {
            float time = 4f;

            yield return new WaitForSeconds(1.5f);
            while (time > 0)
            {
                noodleRaw.SetAlpha(time/4f);
                noodleStream.SetAlpha(1 - time/4);
                time -= Time.deltaTime;
                yield return null;
            }

            //ChangeState(State.Cooked);
        }

        public override void OnClickDown()
        {
            base.OnClickDown();
            SoundControl.Ins.PlayFX(Fx.Click);
        }

        public override void OnClickTake()
        {
            base.OnClickTake();
            SoundControl.Ins.PlayFX(Fx.Take);
        }
    }
}
