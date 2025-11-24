using UnityEngine;
using Link;
using DG.Tweening;

namespace LawNguyen.CookingGame.LRisottoY
{
    public class Rice2 : ItemMovingBase
    {
        public enum State { Raw, Cooking, Cooked, Done }
        private State state = State.Raw;

        [SerializeField] public GameObject mark;

        public override bool IsCanMove => state == State.Raw || state == State.Cooked;

        public override void OnBack()
        {
            base.OnBack();
        }

        public override void OnDrop()
        {
            base.OnDrop();
            OnBack();
        }

        public override bool IsState<T>(T t)
        {
            return t is State s && state == s;
        }

        public override void ChangeState<T>(T t)
        {
            if (t is State newState)
            {
                state = newState;
                switch (state)
                {
                    case State.Raw:
                        break;
                    case State.Cooking:
                        if (mark != null)
                            mark.transform.localPosition = new Vector3(0f, 0.08f, 0f);
                        break;
                    case State.Cooked:
                        break;
                    case State.Done:
                        break;
                }
            }
            else
            {
                Debug.LogWarning("Rice2: Invalid state change attempted.");
            }
        }
    }
}
