using DG.Tweening;
using Link;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HuyThanh.Cooking.TunaSaladFrench
{
    public class Knife : ItemMovingBase
    {
        public enum State
        {
            Ready, Cutting, CuttingBread, CuttingPearl, CuttingButter,
            CuttingPotato, CuttingEgg,
            CuttingGarlic,
            Done
        }

        [SerializeField, ReadOnly] State state;
        [SerializeField] Vector3 rotOnClick;
        [SerializeField] ParticleSystem slashVFX;
        [SerializeField] Animation anim;
        [SerializeField] string cutAnim, cutBreadAnim, cutPearlAnim, cutButterAnim, cutPotatoAnim;

        [SerializeField] private GameObject knifeFront, knifeBack, knifeInBox;

        private Tween showBackTween;
        public override bool IsCanMove => IsState(State.Ready);

        public override void ChangeState<T>(T t)
        {
            state = (State)(object)t;

            switch (state)
            {
                case State.Ready:
                    break;
                case State.Cutting:
                    anim.Play(cutAnim);
                    slashVFX.Play();
                    OrderLayer = 50;

                    SoundControl.Ins.PlayFX(Fx.KnifeCut);
                    DOVirtual.DelayedCall(1.4f, () =>
                    {
                        ChangeState(State.Ready);
                        OnDrop();
                    });
                    break;
                case State.CuttingEgg:
                    SoundControl.Ins.PlayFX(Fx.KnifeCutEgg);
                    anim.Play(cutAnim);
                    OrderLayer = 50;
                    DOVirtual.DelayedCall(1.4f, () =>
                    {
                        ChangeState(State.Ready);
                        OnDrop();
                    });
                    break;
                case State.CuttingBread:
                    anim.Play(cutBreadAnim);
                    SoundControl.Ins.PlayFX(Fx.BeardCutting);
                    DOVirtual.DelayedCall(1.7f, () =>
                    {
                        ChangeState(State.Ready);
                        OnDrop();
                    });

                    break;
                case State.CuttingPearl:
                    anim.Play(cutPearlAnim);
                    SoundControl.Ins.PlayFX(Fx.PearlCutting);
                    DOVirtual.DelayedCall(1.5f, () =>
                    {
                        ChangeState(State.Ready);
                        OnDrop();
                    });
                    break;
                case State.CuttingButter:
                    anim.Play(cutButterAnim);
                    SoundControl.Ins.PlayFX(Fx.ButterCutting);
                    DOVirtual.DelayedCall(1.5f, () =>
                    {
                        ChangeState(State.Ready);
                        OnDrop();
                    });
                    break;
                case State.CuttingPotato:
                    anim.Play(cutPotatoAnim);
                    SoundControl.Ins.PlayFX(Fx.ButterCutting);
                    break;
                case State.Done:
                    break;
                default:
                    break;
            }
        }

        public override bool IsState<T>(T t)
        {
            return state == (State)(object)t;
        }

        public override void OnClickDown()
        {
            base.OnClickDown();
            TF.DORotate(rotOnClick, 0.1f);
            if (showBackTween != null)
            {
                showBackTween.Kill();
            }
            if (IsState(Knife.State.Ready))
            {
                ShowFront();
            }
            SoundControl.Ins.PlayFX(Fx.KnifeTakeOut);
        }

        public override void OnClickTake()
        {
            base.OnClickTake();
            OrderLayer = 50;
            SoundControl.Ins.PlayFX(Fx.Take);
        }

        public override void OnDrop()
        {
            base.OnDrop();
            OnBack();
            showBackTween = DOVirtual.DelayedCall(0.3f, () => ShowInBox());
        }

        private void ShowFront()
        {
            knifeInBox.SetActive(false);
            knifeFront.SetActive(true);
            knifeBack.SetActive(false);
        }

        private void ShowInBox()
        {
            knifeInBox.SetActive(true);
            knifeFront.SetActive(false);
            knifeBack.SetActive(false);
        }

        public void ShowBack()
        {
            knifeInBox.SetActive(false);
            knifeFront.SetActive(false);
            knifeBack.SetActive(true);
        }

        public void ChangeAnim(string name)
        {
            anim.Stop(name);
            anim.Play(name);
        }
    }
}

