using Link;
using System.Collections;
using UnityEngine;



namespace HuyThanh.Cooking.Burrito
{
    public class BowlPur : ItemMovingBase
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
                    anim.Play(animPouring);
                    SoundControl.Ins.PlayFX(Fx.OilPouring);
                    StartCoroutine(WaitForPouring());
                    break;
                case State.Done:
                    OnBack();
                    break;
            }
        }

        IEnumerator WaitForPouring()
        {
            yield return WaitForSecondCache.Get(1.1f);
            ChangeState(State.Done);
        }

        public override void OnClickDown()
        {
            base.OnClickDown();
            SoundControl.Ins.PlayFX(Fx.Click);
        }

        public override void OnDone()
        {
            base.OnDone();
            SoundControl.Ins.PlayFX(Fx.PutDown);
        }
    }
}

