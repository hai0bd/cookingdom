using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link.Cooking.Spageti
{
    public class Scoop : ItemMovingBase
    {
        public enum State { Normal, Noodle, Done }
        [SerializeField] State state;
        [SerializeField] Transform noodlePoint;
        [field: SerializeField] public Noodle noodle;

        public override bool IsCanMove => IsState(State.Normal, State.Noodle);

        public override void ChangeState<T>(T t)
        {
           state = (State)(object)t;
        }
        public override bool IsState<T>(T t)
        {
           return state == (State)(object)t;
        }

        public override void OnDrop()
        {
            base.OnDrop();
            OnBack();
        }

        public override void OnDone()
        {
            base.OnDone();
            OnBack();
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.TryGetComponent(out Noodle noodle))
            {
                if (noodle.IsState(Noodle.State.Cooked))
                {
                    this.noodle = noodle;
                    noodle.TF.SetParent(noodlePoint);
                    noodle.TF.localPosition = Vector3.zero;
                    noodle.ChangeState(Noodle.State.InScoop);
                    ChangeState(State.Noodle);
                }
            }
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
