using DG.Tweening;
using Link;
using UnityEngine;

namespace HuyThanh.Cooking.SarmaleRomania
{


    public class SpatulaCorn : ItemMovingBase
    {
        public enum State
        {
            Normal,
            Mixing,
            Done,
        }
        [SerializeField] State state;
        [SerializeField] GameObject square, squareMixing;
        [SerializeField] ItemAlpha squareAlpha, squareMixingAlpha;
        [SerializeField] Animation anim;
        [SerializeField] string startMixingAnim;

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
                    anim.Play(startMixingAnim);
                    break;
                case State.Done:
                    squareAlpha.SetAlpha(1f);
                    squareMixingAlpha.SetAlpha(0f);
                    square.SetActive(true);
                    squareMixing.SetActive(false);
                    OnBack();
                    break;

            }
        }

        public override void OnClickDown()
        {
            base.OnClickDown();
            TF.DORotate(new Vector3(0, 0, 130), 0.1f);
        }

        public void SetCollider(bool value)
        {
            collider.enabled = value;
        }
    }
}