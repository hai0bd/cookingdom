using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link.Turkey
{
    public class Spoon : ItemMovingBase
    {
        public enum State { Empty, TakeMixed, Done}
        State state = State.Empty;
        [SerializeField] SpriteRenderer core;
        [SerializeField] int orderLayer;
        Vector3 point;
        Pan pan;

        public override bool IsCanMove => !IsState(State.Done);

        private void LateUpdate()
        {
            if (pan != null && pan.IsState(Pan.State.Cooking))
            {
                if (Vector3.Distance(TF.position, point) > 0.1f)
                {
                    point = TF.position;
                    pan.Cooking();
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (pan == null)
            {
                pan = collision.GetComponent<Pan>();
            }
            if (collision.GetComponent<Bowl>() is Bowl b)
            {
                if (IsState(State.TakeMixed)) return;

                if (b.IsState(Bowl.State.Full))
                {
                    b.ChangeState(Bowl.State.AHalf);
                    ChangeState(State.TakeMixed);
                    core.DOFade(1f, 0.2f);
                    SoundControl.Ins.PlayFX(LevelStep_1.FX.Spoon);
                }
                else if (b.IsState(Bowl.State.AHalf))
                {
                    b.ChangeState(Bowl.State.Clear);
                    ChangeState(State.TakeMixed);
                    core.DOFade(1f, 0.2f);
                    SoundControl.Ins.PlayFX(LevelStep_1.FX.Spoon);
                }
            }
            if (collision.GetComponent<Chicken>() is Chicken c)
            {
                if (IsState(State.Empty)) return;
                if (c.IsState(Chicken.State.Marinate_1))
                {
                    c.ChangeState(Chicken.State.Marinate_2);
                    ChangeState(State.Empty);
                    core.DOFade(0f, 0.2f);
                    SoundControl.Ins.PlayFX(LevelStep_1.FX.Spoon);
                }
                else if (c.IsState(Chicken.State.Marinate_2))
                {
                    c.ChangeState(Chicken.State.Marinated);
                    ChangeState(State.Empty);
                    core.DOFade(0f, 0.2f);
                    SoundControl.Ins.PlayFX(LevelStep_1.FX.Spoon);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (pan != null && collision.GetComponent<Pan>() != null)
            {
                pan = null;
            }
        }

        public override void OnDrop()
        {
            base.OnDrop();
            OnClickTake();
            OnBack();
            LevelControl.Ins.CheckStep();
            SetOrder(-50);
        }
        
        public override void ChangeState<T>(T t)
        {
            this.state = (State)(object)(t);
        }

        public override bool IsState<T>(T t)
        {
            return this.state == (State)(object)(t);
        }

        public override void OnClickDown()
        {
            base.OnClickDown();
            OrderLayer = 100;
            SoundControl.Ins.PlayFX(LevelStep_1.FX.Hold);
        }

        public override void OnClickTake()
        {
            base.OnClickTake();
            OrderLayer = orderLayer;
            SoundControl.Ins.PlayFX(LevelStep_1.FX.Put);
        }
    }
}