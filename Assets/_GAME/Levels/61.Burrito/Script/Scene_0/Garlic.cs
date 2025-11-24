using DG.Tweening;
using Link;
using UnityEngine;

namespace HuyThanh.Cooking.Burrito

{

    public class Garlic : ItemMovingBase
    {
        public enum State { Normal, Done }

        [SerializeField] private State state;

        [SerializeField] Animation anim;
        [SerializeField] string animClean;

        [SerializeField] Transform targetDoneTF; //target TF khi ma rua xong
        [SerializeField] int targetOrderLayer;

        [SerializeField] HintText hintText;
        public override bool IsCanMove => IsState(State.Normal);

        private int numberOfGarlicRotten = 0;

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
                case State.Done:
                    hintText.OnActiveHint();
                    OnMove(targetDoneTF.position, targetDoneTF.rotation, 0.2f);
                    DOVirtual.DelayedCall(0.2f, () =>
                    {
                        anim.Play(animClean);
                        OrderLayer = targetOrderLayer;
                        collider.enabled = false;
                    });
                    break;
            }
        }

        public void OnDoneThrowRotten()
        {
            numberOfGarlicRotten++;
            if (numberOfGarlicRotten >= 2)
            {
                this.enabled = true;
                collider.enabled = true;
            }
        }

        public override void OnClickDown()
        {
            SoundControl.Ins.PlayFX(Fx.Click);
            base.OnClickDown();
        }

        public override void OnClickTake()
        {
            SoundControl.Ins.PlayFX(Fx.Take);
            base.OnClickTake();
        }
    }

}