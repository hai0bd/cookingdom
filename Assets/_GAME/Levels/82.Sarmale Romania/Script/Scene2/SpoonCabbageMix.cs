using DG.Tweening;
using Link;
using UnityEngine;

namespace HuyThanh.Cooking.SarmaleRomania
{
    public class SpoonCabbageMix : ItemMovingBase
    {
        public enum State
        {
            Normal,
            Mixing,
            Done,
        }
        [SerializeField] State state;
        [SerializeField] Transform square, squareMixing;

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
                case State.Mixing:
                    SetCollider(false);
                    square.gameObject.SetActive(false);
                    squareMixing.gameObject.SetActive(true);
                    //anim.Play(startMixingAnim);
                    break;
                case State.Done:
                    square.gameObject.SetActive(true);
                    squareMixing.gameObject.SetActive(false);
                    //squareAlpha.SetAlpha(1f);
                    //squareMixingAlpha.SetAlpha(0f);
                    //square.SetActive(true);
                    //squareMixing.SetActive(false);
                    OnBack();
                    break;

            }
        }

        public override void OnClickDown()
        {
            base.OnClickDown();
            TF.DORotate(Vector3.forward * -130f, 0.1f);
        }

        public override void OnClickTake()
        {
            base.OnClickTake();
            OrderLayer = 5;
        }

        public void SetCollider(bool value)
        {
            collider.enabled = value;
        }
    }
}