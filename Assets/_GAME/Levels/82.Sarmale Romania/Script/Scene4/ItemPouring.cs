using Link;
using System.Collections;
using UnityEngine;

namespace HuyThanh.Cooking.SarmaleRomania
{
    public class ItemPouring : ItemMovingBase
    {
        public enum State
        {
            Normal,
            Pouring,
            Done
        }

        public enum PouringType
        {
            Cabbage,
            Rosemary,
            Dill,
            TomatoSauce,
            WaterBowl,
            Oil,
            Onion,
            Rice,
            Chili,
            Beef,
            Pork,
            Pepper,
            Salt,
            DillMix,
        }

        [SerializeField] State state;
        [SerializeField] PouringType pouringType;
        [SerializeField] Animation anim;
        [SerializeField] float animTime;
        [SerializeField] string animPouringName;

        public override bool IsCanMove => IsState(State.Normal);

        public override bool IsState<T>(T t)
        {
            return state == (State)(object)t;
        }

        public bool IsPouringType(PouringType type)
        {
            return pouringType == type;
        }

        public override void ChangeState<T>(T t)
        {
            state = (State)(object)t;

            switch (state)
            {
                case State.Normal:
                    break;
                case State.Pouring:
                    anim.Play(animPouringName);
                    StartCoroutine(WaitForPouring());
                    break;
                case State.Done:
                    OnBack();
                    break;
            }
        }

        IEnumerator WaitForPouring()
        {
            yield return WaitForSecondCache.Get(animTime);
            ChangeState(State.Done);
        }

        public override void OnClickTake()
        {
            base.OnClickTake();
            OrderLayer = 51;
        }
    }
}