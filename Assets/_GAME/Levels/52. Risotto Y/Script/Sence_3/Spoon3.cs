using UnityEngine;
using DG.Tweening;
using Link;

namespace LawNguyen.CookingGame.LRisottoY
{
    public class Spoon3 : ItemMovingBase
    {
        public enum State { Idle, Moving }

        [SerializeField] private Animation animation;
        [SerializeField] private AnimationClip idleClip;
        [SerializeField] private AnimationClip movingClip;
        [SerializeField] private Transform takeSeafoodPoint;

        private State state;
        private Transform currentSeafood;

        public bool HaveSeafood => currentSeafood != null;

        public void TakeSeafood(Transform seafood)
        {
            if (HaveSeafood || takeSeafoodPoint == null) return;

            isBackWhenDrop = false;
            currentSeafood = seafood;

            seafood.SetParent(takeSeafoodPoint);
            seafood.DOLocalMove(Vector3.zero, 0.1f).OnComplete(() =>
            {
                seafood.localRotation = Quaternion.identity;
                seafood.localScale = Vector3.one;
            });
        }

        public void DropSeafood()
        {
            if (!HaveSeafood) return;

            isBackWhenDrop = true;

            currentSeafood.SetParent(null);
            currentSeafood = null;
        }

        public override void OnClickDown()
        {
            SetOrder(50);
            base.OnClickDown();
           
            if (HaveSeafood) return;
            isBackWhenDrop = false;
            ChangeState(State.Moving);
        }

        public override void OnDrop()
        {
          
            if (HaveSeafood) return;
            isBackWhenDrop = true;
            base.OnDrop();

        }

        public override void OnBack()
        {
            base.OnBack();
            ChangeState(State.Idle);
        }

        public override void ChangeState<T>(T newState)
        {
            state = (State)(object)newState;

            switch (state)
            {
                case State.Idle:
                    if (idleClip != null) ChangeAnim(idleClip.name);
                    break;
                case State.Moving:
                    if (movingClip != null) ChangeAnim(movingClip.name);
                    break;
            }
        }

        private void ChangeAnim(string clipName)
        {
            if (animation != null && !string.IsNullOrEmpty(clipName))
            {
                animation.Play(clipName);
            }
        }
    }
}
