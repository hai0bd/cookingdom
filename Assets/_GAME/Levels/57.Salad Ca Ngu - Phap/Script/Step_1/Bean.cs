using DG.Tweening;
using Link;
using UnityEngine;

namespace HuyThanh.Cooking.TunaSaladFrench
{
    public class Bean : ItemMovingBase
    {
        public enum State
        {
            Normal,
            Cutting,
            Raw,
            Cooking,
            Spice,
            Done
        }

        [SerializeField] private State state;
        [SerializeField] private GameObject origin, cutting, doneCut, doneCook;
        [SerializeField] private ItemAlpha originAlpha, cookedAlpha;
        [SerializeField] private ParticleSystem vfxBlink;
        [SerializeField] private ParticleSystem vfxSmoke;
        [SerializeField] private Animation anim;

        [SerializeField] Sprite hint;
        public override bool IsCanMove => IsState(Bean.State.Normal, Bean.State.Raw);

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
                    DOVirtual.DelayedCall(.4f, () =>
                    {
                        origin.SetActive(false);
                        cutting.SetActive(true);
                    });
                    DOVirtual.DelayedCall(.8f, () =>
                    {
                        cutting.SetActive(false);
                        doneCut.SetActive(true);
                    });
                    DOVirtual.DelayedCall(1.2f, () =>
                    {
                        ChangeState(Bean.State.Raw);
                    });
                    break;
                case State.Raw:
                    LevelControl.Ins.SetHint(hint);
                    break;

                case State.Cooking:
                    OrderLayer = 3;
                    originAlpha.DoAlpha(0f, 4f);
                    cookedAlpha.DoAlpha(1f, 4f);
                    DOVirtual.DelayedCall(4f, () =>
                    {
                        ChangeState(Bean.State.Spice);
                    });
                    break;
                case State.Spice:
                    vfxSmoke.Play();
                    vfxBlink.Play();
                    break;
                case State.Done:
                    DOVirtual.DelayedCall(0.2f, () =>
                    {
                        OrderLayer = -1;
                    });
                    ChangeAnim("CookingItemScale");
                    break;
            }
        }

        public override void OnDrop()
        {
            OnBack();
            base.OnDrop();
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

        public void ChangeAnim(string animName)
        {
            if (anim == null) return;
            anim.Play(animName);
        }
    }
}