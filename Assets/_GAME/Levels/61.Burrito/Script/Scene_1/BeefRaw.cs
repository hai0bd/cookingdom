using DG.Tweening;
using Link;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HuyThanh.Cooking.Burrito
{
    public class BeefRaw : ItemMovingBase
    {
        public enum State
        {
            Normal,
            Cutting,
            KatanaCutting,
            DoneCut,
            Grindering,
            Done
        }
        public override bool IsCanMove => IsState(State.Normal, State.DoneCut);

        [SerializeField] State state;
        [SerializeField] Knife knife;

        [SerializeField] Animation anim;
        [SerializeField] string animBeefGet, animBeefOnClick;
        [SerializeField] ParticleSystem splashVFX;
        [BoxGroup("Cutting")][SerializeField] Transform maskSlide, maskSpice, startPoint, finishPoint;

        private float step = 0;

        private const int CHOP_STEP = 4;

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
                    knife.OnMove(startPoint.position, Quaternion.Euler(0, 0, 90f), 0.2f);
                    break;
                case State.KatanaCutting:
                    SoundControl.Ins.PlayFX(Fx.KnifeSplash);
                    DOVirtual.DelayedCall(0.5f, () => anim.Play(animBeefGet));
                    DOVirtual.DelayedCall(1.5f, () => ChangeState(State.DoneCut));
                    break;
                case State.DoneCut:
                    SoundControl.Ins.PlayFX(Fx.DoneSomething);
                    break;
                case State.Grindering:
                    break;
                case State.Done:
                    break;
            }
        }

        public override void OnClickDown()
        {
            if (IsState(State.Cutting))
            {
                OrderLayer = 1;

                knife.ChangeAnim("KnifeBeefCutting");
                SoundControl.Ins.PlayFX(Fx.KnifeCut);

                step += 1f / CHOP_STEP;
                Vector2 point = Vector3.Lerp(startPoint.position, finishPoint.position, step);
                splashVFX.transform.position = point;
                splashVFX.Play();
                maskSlide.position = point;
                maskSpice.position = point;
                knife.TF.position = point;

                if (step >= 1f)
                {
                    step = 0; // reset counter step
                    ChangeState(State.KatanaCutting);
                    knife.OnMove(TF.position, Quaternion.identity, 0.2f);
                    DOVirtual.DelayedCall(0.2f, () => knife.ChangeState(Knife.State.KatanaCutting));
                }
                return;
            }
            SoundControl.Ins.PlayFX(Fx.Click);
            if (IsState(State.DoneCut))
            {
                anim.Play(animBeefOnClick);
            }
            base.OnClickDown();
        }

        public override void OnDrop()
        {
            if (IsState(State.DoneCut))
            {
                anim.Play(animBeefOnClick);
            }

            SoundControl.Ins.PlayFX(Fx.Click);
            base.OnDrop();
        }
    }
}