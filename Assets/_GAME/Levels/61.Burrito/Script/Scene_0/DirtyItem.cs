using DG.Tweening;
using Link;
using UnityEngine;

namespace HuyThanh.Cooking.Burrito
{
    public class DirtyItem : ItemMovingBase
    {
        public enum State
        {
            Dirty,
            Cleaning,
            Normal,
            Done
        }

        [SerializeField] private State state;

        [SerializeField] ItemAlpha dirtyAlpha;
        [SerializeField] ParticleSystem vfxSplashWater;
        [SerializeField] ParticleSystem vfxCleanWater;
        [SerializeField] ParticleSystem vfxBlink;

        [SerializeField] Animation anim;
        [SerializeField] string animClean;

        [SerializeField] int numberTapToClean;

        [SerializeField] Transform targetDoneTF; //target TF khi ma rua xong
        [SerializeField] int targetOrderLayer;
        [SerializeField] HintText hintText;
        public override bool IsCanMove => IsState(State.Dirty, State.Normal);

        private float currentDirtyAlpha = 1f;

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
                case State.Cleaning:
                    break;
                case State.Normal:
                    break;
                case State.Done:
                    hintText.OnActiveHint();
                    LevelControl.Ins.NextHint();
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

        public override void OnDrop()
        {
            base.OnDrop();
            SoundControl.Ins.PlayFX(Fx.Take);
            if (IsState(State.Normal))
            {
                DOVirtual.DelayedCall(0.31f, () =>
                {
                    vfxSplashWater.transform.position = TF.position;
                    vfxSplashWater.Play();
                    OrderLayer = -1;
                }); ///dang trong nuoc nen se de layer thap hon
            }
        }

        public override void OnClickTake()
        {
            base.OnClickTake();
            SoundControl.Ins.PlayFX(Fx.Take);
        }

        public override void OnClickDown()
        {

            if (IsState(State.Cleaning))
            {
                SoundControl.Ins.PlayFX(Fx.WaterSplash);
                vfxCleanWater.transform.position = TF.position;
                vfxCleanWater.Play();

                anim.Play(animClean);

                currentDirtyAlpha -= 1f / numberTapToClean;
                dirtyAlpha.DoAlpha(currentDirtyAlpha, 0.1f);

                if (currentDirtyAlpha < 0f)
                {
                    vfxCleanWater.Stop();
                    vfxBlink.transform.position = TF.position;
                    vfxBlink.Play();
                    SoundControl.Ins.PlayFX(Fx.DoneSomething);
                    ChangeState(State.Normal);
                }
                return;
            }
            SoundControl.Ins.PlayFX(Fx.Click);
            base.OnClickDown();
        }
    }
}