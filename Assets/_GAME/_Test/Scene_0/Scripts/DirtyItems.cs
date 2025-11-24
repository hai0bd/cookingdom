using DG.Tweening;
using Link;
using UnityEngine;

namespace HoangLinh.Cooking.Test
{
    public class DirtyItems : ItemMovingBase
    {
        public enum State
        {
            Dirty,
            Cleaning,
            Normal,
            Done
        }

        [SerializeField] private State state;
        [SerializeField] Animation anim;
        [SerializeField] string animCleaning;

        [SerializeField] ItemAlpha dirtyAlpha;
        [SerializeField] Transform targetDoneTF;
        [SerializeField] int targetOrderLayer;
        [SerializeField] HintText hintText;

        [SerializeField] ParticleSystem vfxSplashWater;
        [SerializeField] ParticleSystem vfxWaterClean;
        [SerializeField] ParticleSystem vfxBlink;

        [SerializeField] int numberTapToClean;

        public override bool IsCanMove => IsState(State.Dirty, State.Normal);
        private float currentDirtyAlpha = 1f;
        public override bool IsState<T>(T t)
        {
            return state == (State)(object)t;
        }

        protected override void Start()
        {
            base.Start();
            isBackWhenDrop = true;
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
                    OnMove(targetDoneTF.position, targetDoneTF.rotation, 0.2f);
                    DOVirtual.DelayedCall(0.2f, () =>
                    {
                        anim.Play(animCleaning);
                        collider.enabled = false;
                    });
                    break;
            }
        }

        public override void OnDrop()
        {
            base.OnDrop();
            SoundControl.Ins.PlayFX(Fx.PickUp);

            if (IsState(State.Normal))
            {
                DOVirtual.DelayedCall(0.31f, () =>
                {
                    vfxSplashWater.transform.position = TF.position;
                    vfxSplashWater.Play();
                    OrderLayer = -1;
                }); 
            }

        }

        public override void OnClickTake()
        {
            base.OnClickTake();
            SoundControl.Ins.PlayFX(Fx.PickUp);

        }


        public override void OnClickDown()
        {
            if (IsState(State.Cleaning))
            {
                vfxWaterClean.transform.position = TF.position;
                vfxWaterClean.Play();
                SoundControl.Ins.PlayFX(Fx.WaterSplash);

                currentDirtyAlpha -= 1f / numberTapToClean;
                dirtyAlpha.DoAlpha(currentDirtyAlpha, 0.1f);
                anim.Play(animCleaning);
                Debug.Log("Cleaning!!!!!!!");

                if (currentDirtyAlpha < 0f)
                {
                    vfxWaterClean.Stop();
                    vfxBlink.transform.position = TF.position;
                    vfxBlink.Play();
                    SoundControl.Ins.PlayFX(Fx.DoneSth);
                    ChangeState(State.Normal);
                }
                return;
            }
            base.OnClickDown();
        }
        
    }
}