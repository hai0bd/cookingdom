using DG.Tweening;
using Link;
using UnityEngine;


namespace HuyThanh.Cooking.Burrito
{
    public class PlateRedBean : ItemMovingBase
    {
        public enum State
        {
            HaveBean,
            Pouring,
            Done
        }

        [SerializeField] State state;
        [SerializeField] private Animation anim;
        [SerializeField] private string animPouring;

        public override bool IsCanMove => IsState(State.HaveBean);

        public override bool IsState<T>(T t)
        {
            return state == (State)(object)t;
        }

        public override void ChangeState<T>(T t)
        {
            state = (State)(object)t;

            switch (state)
            {
                case State.HaveBean:
                    // Logic for when the plate has beans
                    break;
                case State.Pouring:
                    OrderLayer = 49;
                    // Play pouring animation
                    anim.Play(animPouring);
                    DOVirtual.DelayedCall(1.2f, () => ChangeState(State.Done));
                    break;
                case State.Done:
                    // Logic for when the plate is done

                    OnBack();

                    collider.enabled = false;
                    break;
            }
        }

        public override void OnBack()
        {
            TF.DOMove(savePoint, 0.3f).OnComplete(() => OrderLayer = -50);
            TF.DORotateQuaternion(saveRot, 0.3f);
        }

        public override void OnClickDown()
        {
            SoundControl.Ins.PlayFX(Fx.Click);
            base.OnClickDown();
        }

        public override void OnDrop()
        {
            base.OnDrop();
            SoundControl.Ins.PlayFX(Fx.PutDown);
        }

    }

}
