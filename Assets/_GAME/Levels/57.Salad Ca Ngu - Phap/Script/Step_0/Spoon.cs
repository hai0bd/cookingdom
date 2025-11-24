using DG.Tweening;
using Link;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HuyThanh.Cooking.TunaSaladFrench
{
    public enum SpoonSpiceType
    {
        Mustard, Sugar, Salt, Chill,
    }

    public class Spoon : ItemMovingBase
    {
        public enum State { Normal, HavingSpice, Pouring, Done }
        [SerializeField] State state;
        [SerializeField] Vector3 rotationOnClick;
        [SerializeField] bool IsRandomOrder = false;
        [SerializeField] GameObject itemActive;
        [SerializeField] GameObject itemMustard;

        [BoxGroup("Animation")][SerializeField] Animation anim;
        [BoxGroup("Animation")][SerializeField] string animPouring;
        public override bool IsCanMove => IsState(State.Normal, State.HavingSpice);

        public override void ChangeState<T>(T t)
        {
            state = (State)(object)t;

            switch (state)
            {
                case State.Normal:
                    OnBack();
                    break;
                case State.HavingSpice:
                    itemMustard.SetActive(true);
                    break;

                case State.Pouring:
                    ChangePouringState();
                    break;
                case State.Done:
                    OnBack();
                    break;
            }
        }
        public override bool IsState<T>(T t)
        {
            return state == (State)(object)t;
        }

        public override void OnDrop()
        {
            base.OnDrop();
            OnBack();
        }

        public override void OnDone()
        {
            base.OnDone();
            OnBack();
        }

        public override void OnClickDown()
        {
            base.OnClickDown();
            TF.DORotate(rotationOnClick, 0.1f);
            SoundControl.Ins.PlayFX(Fx.Click);
        }

        public override void OnClickTake()
        {
            base.OnClickTake();

            SoundControl.Ins.PlayFX(Fx.Take);
        }


        private void ChangePouringState()
        {

            anim.Play(animPouring);
            DOVirtual.DelayedCall(1f, () =>
            {
                itemActive.SetActive(true);
            });
            DOVirtual.DelayedCall(1.2f, () =>
            {
                ChangeState(Spoon.State.Normal);
            });
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out MustardIdle mustardIdle))
            {
                if (mustardIdle.IsState(MustardIdle.State.OpenCap) && IsState(SpoonSaladSauce.State.Normal))
                {
                    ChangeState(SpoonSaladSauce.State.HaveSauce);
                    mustardIdle.ChangeState(MustardIdle.State.TookMustard);
                }
            }
        }
    }

}
