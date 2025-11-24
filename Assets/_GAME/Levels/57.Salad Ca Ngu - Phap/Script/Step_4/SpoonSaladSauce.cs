using DG.Tweening;
using Link;
using UnityEngine;

namespace HuyThanh.Cooking.TunaSaladFrench
{
    public class SpoonSaladSauce : ItemMovingBase
    {
        public enum State
        {
            Normal,
            HaveSauce,
            Pouring,
            Done
        }
        [SerializeField] State state;
        [SerializeField] Vector3 rotationOnClick;
        [SerializeField] GameObject sauceGO;
        [SerializeField] Animation anim;
        [SerializeField] string animPouring;
        [SerializeField] GameObject activeItem;
        public override bool IsCanMove => IsState(State.Normal, State.HaveSauce);

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
                case State.HaveSauce:
                    sauceGO.SetActive(true);
                    break;
                case State.Pouring:
                    anim.Play(animPouring);
                    DOVirtual.DelayedCall(.6f, () => activeItem.SetActive(true));
                    DOVirtual.DelayedCall(1.5f, () => ChangeState(State.Done));
                    break;
                case State.Done:
                    OnBack();
                    DOVirtual.DelayedCall(0.3F, () => OrderLayer = -50);
                    break;
            }
        }

        public override void OnClickDown()
        {
            base.OnClickDown();
            TF.DORotate(rotationOnClick, 0.1f);
            SoundControl.Ins.PlayFX(Fx.Click);
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out MortarSpice mortar))
            {
                if (mortar.IsState(MortarSpice.State.Done) && IsState(SpoonSaladSauce.State.Normal))
                {
                    ChangeState(SpoonSaladSauce.State.HaveSauce);
                    mortar.ScaleDownSauce();
                }
            }
        }
    }

}