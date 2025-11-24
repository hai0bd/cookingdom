using DG.Tweening;
using Link;
using UnityEngine;

namespace HuyThanh.Cooking.Burrito
{
    public class Colander : ItemMovingBase
    {
        public enum State
        {
            NotHaveBean,
            HaveBean,
            Washing,
            DoneWash,
            Pouring,
            Done
        }

        [SerializeField] private State state;
        [SerializeField] ParticleSystem vfxCleanWater, vfxBlink;
        [SerializeField] int numberTapToClean;
        [SerializeField] private ItemAlpha beanAlpha, waterAlpha, dirtyAlpha;

        [SerializeField] Animation anim, animColander;
        [SerializeField] string animClean, animPouring;
        [SerializeField] Vector3 oldPosition;

        public override bool IsCanMove => IsState(State.HaveBean, State.DoneWash);

        private float currentDirtyAlpha = 0.15f;

        private Tween delayedCallWaterLayer;

        public override bool IsState<T>(T t)
        {
            return state == (State)(object)t;
        }

        public override void ChangeState<T>(T t)
        {
            state = (State)(object)t;

            switch (state)
            {
                case State.NotHaveBean:
                    break;
                case State.HaveBean:
                    break;
                case State.Washing:
                    waterAlpha.DoAlpha(1f, 0.1f);
                    break;
                case State.DoneWash:
                    break;
                case State.Pouring:
                    animColander.Play(animPouring);
                    DOVirtual.DelayedCall(1.3f, () => ChangeState(State.Done));
                    break;
                case State.Done:
                    LevelControl.Ins.NextHint();
                    OnMove(oldPosition, Quaternion.identity, 0.2f);
                    OnSave(0.2f);
                    LevelControl.Ins.CheckStep(1f);
                    break;
            }
        }

        public override bool OnTake(IItemMoving item)
        {
            if (item is PlateRedBean && item.IsState(PlateRedBean.State.HaveBean))
            {
                DOVirtual.DelayedCall(1f, () => ChangeState(State.HaveBean));

                item.ChangeState(PlateRedBean.State.Pouring);
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                beanAlpha.DoAlpha(1f, 0.1f, 1f); /// bat bean trong noi len
                dirtyAlpha.DoAlpha(currentDirtyAlpha, 0.1f, 1f); /// bat do ban dau cua noi
                return true;
            }
            return base.OnTake(item);
        }

        public override void OnClickDown()
        {

            if (IsState(State.Washing))
            {
                SoundControl.Ins.PlayFX(Fx.WaterSplash);
                vfxCleanWater.transform.position = TF.position;
                vfxCleanWater.Play();

                anim.Play(animClean);

                currentDirtyAlpha -= 0.15f / numberTapToClean;
                dirtyAlpha.DoAlpha(currentDirtyAlpha, 0.1f);

                if (currentDirtyAlpha < 0f)
                {
                    vfxCleanWater.Stop();
                    vfxBlink.transform.position = TF.position;
                    vfxBlink.Play();
                    SoundControl.Ins.PlayFX(Fx.DoneSomething);
                    ChangeState(State.DoneWash);
                }
                return;
            }

            SoundControl.Ins.PlayFX(Fx.Click);

            if (IsState(State.DoneWash) && LevelControl.Ins.IsHaveObject<WaterSink>(TF.position))
            {
                if (delayedCallWaterLayer != null && delayedCallWaterLayer.IsActive())
                {
                    delayedCallWaterLayer.Kill();
                }
                waterAlpha.DoAlpha(0, 0.1f);
            }

            base.OnClickDown();
        }

        public override void OnDrop()
        {
            SoundControl.Ins.PlayFX(Fx.PutDown);
            delayedCallWaterLayer = DOVirtual.DelayedCall(0.3f, () =>
            {
                if (IsState(State.DoneWash) && LevelControl.Ins.IsHaveObject<WaterSink>(TF.position))
                {
                    waterAlpha.DoAlpha(1, 0.1f);
                }
            });
            base.OnDrop();
        }
    }
}
