using HuyThanh.Cooking.Burrito;
using Link;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hai.Cooking.Burrito
{
    public enum SpiceType
    {
            Onion,
            Garlic,
            Meat,
            Pepper,
            Chili,
            Salt,
            Tumeric,
            Tomato,
            Oregano
    }
    public class SpiceItem : ItemMovingBase
    {
        public enum State { Normal, Pouring, Done};
        [SerializeField] State state;
        [SerializeField] SpiceType spiceType;
        [SerializeField] Fx soundType;

        [SerializeField] Animation anim;
        [SerializeField] string animPouring;

        public override bool IsCanMove => IsState(State.Normal);

        public override bool IsState<T>(T t)
        {
            return state == (State)(object) t;
        }

        public override void ChangeState<T>(T t)
        {
            state = (State)(object) t;

            switch (state)
            {
                case State.Pouring:
                    OrderLayer = -49;
                    anim.Play(animPouring);
                    SoundControl.Ins.PlayFX(soundType);
                    StartCoroutine(WaitingForPouring());
                    break;
                case State.Done:
                    OnBack();
                    break;
            }
        }

        public override void OnDrop()
        {
            SoundControl.Ins.PlayFX(Fx.PutDown);
            base.OnDrop();
        }

        public override void OnClickTake()
        {
            base.OnClickTake();
            if (IsState(State.Pouring))
            {
                OrderLayer = -49;
            }
        }

        public override void OnClickDown()
        {
            base.OnClickDown();
            SoundControl.Ins.PlayFX(Fx.Click);
        }

        public bool IsSpiceType(SpiceType type)
        {
            return this.spiceType == type;
        }

        IEnumerator WaitingForPouring()
        {
            yield return WaitForSecondCache.Get(1.1f);
            ChangeState(State.Done);
        }
    }
}
