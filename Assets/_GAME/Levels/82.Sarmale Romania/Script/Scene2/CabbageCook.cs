using Link;
using System.Collections;
using UnityEngine;

namespace HuyThanh.Cooking.SarmaleRomania
{

    public class CabbageCook : ItemMovingBase
    {
        public enum State
        {
            Normal,
            Cooking,
            WaitingFork,
            TappingToRemoveLeaf,
            Done,
        }
        [SerializeField] State state;
        [SerializeField] Animation anim;
        [SerializeField] ItemAlpha squareCabbageAlpha, squareCabbageCookedAlpha;
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
                case State.Cooking:
                    StartCoroutine(WaitForCooking());
                    break;
            }
        }

        public override void OnClickTake()
        {
            base.OnClickTake();
            OrderLayer = -2;
        }

        public void DoneRemoveLeaf()
        {
            squareCabbageCookedAlpha.DoAlpha(0, 0.2f);
        }

        IEnumerator WaitForCooking()
        {
            yield return WaitForSecondCache.Get(0.4f);
            squareCabbageCookedAlpha.DoAlpha(1, 4f);
            squareCabbageAlpha.DoAlpha(0, 4f);

            yield return WaitForSecondCache.Get(4f);
            ChangeState(State.WaitingFork);
        }

        public void PlayAnim()
        {
            anim.Play("Bounce");
        }
    }
}