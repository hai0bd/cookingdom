using DG.Tweening;
using Link;
using UnityEngine;

namespace HuyThanh.Cooking.TunaSaladFrench
{
    public class XaLach : ItemMovingBase
    {
        public enum State
        {
            Dirty,
            Washing,
            Spice,
            Done
        }

        [SerializeField] private State state;

        [SerializeField] private GameObject dirtyGameObject;
        [SerializeField] private ParticleSystem vfxBlink;

        [SerializeField] private Animation anim;
        [SerializeField] private string animDirty;

        public override bool IsCanMove => IsState(XaLach.State.Dirty, XaLach.State.Spice);

        public override bool IsState<T>(T t)
        {
            return state == (State)(object)t;
        }

        public override void ChangeState<T>(T t)
        {
            state = (State)(object)t;

            switch (state)
            {
                case State.Dirty:
                    break;
                case State.Washing:
                    anim.Play(animDirty);
                    DOVirtual.DelayedCall(1f, () => ChangeState(State.Spice));
                    break;
                case State.Spice:
                    dirtyGameObject.SetActive(false);
                    vfxBlink.Play();
                    break;
                case State.Done:
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