using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Link;
using DG.Tweening;

namespace HoangLinh.Cooking.Test
{
    public class Oil : ItemMovingBase
    {
        public enum State
        {
            Normal,
            Pouring,
            Done,
        }

        [SerializeField] State state;
        [SerializeField] Animation anim;
        [SerializeField] string animPouring;
        [SerializeField] Transform moveTF;

        public override bool IsCanMove => IsState(State.Normal);

        public override bool IsState<T>(T t)
        {
            return state == (State)(object)t;
        }

        public override void ChangeState<T>(T t)
        {
            state = (State)(object)t;
            switch (state)
            {
                case State.Normal:
                    break;
                case State.Pouring:
                    OrderLayer = 49;
                    StartCoroutine(MoveSpices());
                    break;
                case State.Done:
                    OnBack();
                    break;
            }
        }

        public override void OnClickDown()
        {
            base.OnClickDown();
            SoundControl.Ins.PlayFX(Fx.PickUp);
        }

        public override void OnDone()
        {
            base.OnDone();
            SoundControl.Ins.PlayFX(Fx.Drop);
        }

        IEnumerator MoveSpices()
        {
            this.TF.DOMove(moveTF.position, .3f);
            yield return new WaitForSeconds(.35f);
            anim.Play(animPouring);
            yield return new WaitForSeconds(1.15f);
            ChangeState(State.Done);
        }
    }
}