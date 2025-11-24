using DG.Tweening;
using Link;
using UnityEngine;

namespace HuyThanh.Cooking.TunaSaladFrench
{
    public class PanOvenMoving : ItemMovingBase
    {
        public enum State
        {
            Normal,
            Baking,
            Baked,
            Done
        }

        [SerializeField] State state;
        [SerializeField] ItemAlpha nutCookedAlpha;
        [SerializeField] ParticleSystem vfxBlink;

        public override bool IsCanMove => IsState(State.Normal, State.Baked);

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
                case State.Baking:
                    nutCookedAlpha.DoAlpha(1f, 4f);
                    DOVirtual.DelayedCall(4f, () => ChangeState(PanOvenMoving.State.Baked));
                    break;
                case State.Done:
                    vfxBlink.Play();
                    LevelControl.Ins.CheckStep(1f);
                    break;
            }
        }

        public void SetCollider(bool isOn)
        {
            collider.enabled = isOn;
        }
    }

}