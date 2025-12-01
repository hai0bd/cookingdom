using Link;
using System.Collections;
using UnityEngine;

namespace Hai.Cooking.NewTest
{
    public class SpiceItem : ItemMovingBase
    {
        public enum State
        {
            Normal,
            Pouring,
            Done
        }
        [SerializeField] private State state;
        [SerializeField] private SpiceType spiceType;

        [SerializeField] private PouringAnim pouringAnim;

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
                case State.Pouring:
                    StartPouring();
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

        public override void OnClickDown()
        {
            SoundControl.Ins.PlayFX(Fx.Click);
            base.OnClickDown();
        }

        public override void OnClickTake()
        {
            base.OnClickTake();
            if(IsState(State.Pouring))
            {
                OrderLayer = 49;
            }
        }

        public bool IsSpiceType(SpiceType type)
        {
            return spiceType == type;
        }

        private void StartPouring()
        {
            if (spiceType == SpiceType.Salt || spiceType == SpiceType.Pepper)
            {
                pouringAnim.PutUp(Fx.SaltPouring);
            }
            else if (spiceType == SpiceType.Oil)
            {
                pouringAnim.PutUp(Fx.OilPour);
            }
            StartCoroutine(WaitForPouring());
        }

        private IEnumerator WaitForPouring()
        {
            yield return new WaitForSeconds(1.5f);
            pouringAnim.PutDown();
            ChangeState(State.Done);
        }
    }
}