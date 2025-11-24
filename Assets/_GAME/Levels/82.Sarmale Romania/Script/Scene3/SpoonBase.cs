using DG.Tweening;
using Link;
using System.Collections;
using UnityEngine;

namespace HuyThanh.Cooking.SarmaleRomania
{
    public class SpoonBase : ItemMovingBase
    {
        public enum State
        {
            Normal,
            Pouring,
        }
        [SerializeField] State state;
        [SerializeField] GameObject itemTF;
        [SerializeField] Animation anim;
        [SerializeField] string animGetItem, animPouringName;

        private bool isHaveBase = false;

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
                    isHaveBase = false;
                    anim.Play(animPouringName);
                    StartCoroutine(WaitForDonePouring());
                    break;
                case State.Normal:
                    break;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out BowlBase bowlBase) && isHaveBase == false && bowlBase.IsCanTake())
            {
                isHaveBase = true;
                itemTF.SetActive(true);
                bowlBase.TakeSpice();
                bowlBase.PlayTakeAnim();
                return;
            }
        }

        public override void OnClickDown()
        {
            base.OnClickDown();
            TF.DORotate(Vector3.forward * 120, 0.1f);
        }

        public bool CanTake()
        {
            return isHaveBase;
        }

        IEnumerator WaitForDonePouring()
        {
            yield return WaitForSecondCache.Get(0.5f);

            OnBack();
            ChangeState(State.Normal);
        }
    }
}
