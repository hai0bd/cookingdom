using DG.Tweening;
using Link;
using UnityEngine;

namespace HuyThanh.Cooking.TunaSaladFrench
{
    public class Butter : ItemMovingBase
    {
        public enum State
        {
            Normal,
            Cutting,
            Sliced,
            Done
        }

        [SerializeField] private State state;

        [SerializeField] private Animation anim;
        [SerializeField] private string animCut;
        [SerializeField] private GameObject banhMiSliced1, banhMiSliced2;

        public override bool IsCanMove => IsState(Pearl.State.Normal, Pearl.State.Sliced);

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

                case State.Cutting:
                    anim.Play(animCut);
                    DOVirtual.DelayedCall(1.5f, () =>
                    {
                        ChangeState(State.Done);
                        banhMiSliced1.SetActive(true);
                        banhMiSliced2.SetActive(true);
                        this.OnBack();
                        this.gameObject.SetActive(false);

                    });
                    break;
            }

        }

        public override void OnClickDown()
        {
            base.OnClickDown();
            SoundControl.Ins.PlayFX(Fx.Click);
        }
        public override void OnClickTake()
        {
            base.OnClickTake();
            SoundControl.Ins.PlayFX(Fx.Take);
        }
    }

}
