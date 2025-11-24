using DG.Tweening;
using Link;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HuyThanh.Cooking.Burrito

{
    public class GarlicCutItem : ItemMovingBase
    {
        private const int CHOP_STEP = 4;
        private const int CUT_STEP = 10;
        public enum State { Normal, Cutting1, Cutting2, Spice, Done }
        public override bool IsCanMove => IsState(State.Normal, State.Spice);

        [SerializeField] State state;
        [SerializeField] Knife knife;
        [SerializeField] ParticleSystem splashVFX;
        [SerializeField] ParticleSystem chopVFX;

        [SerializeField] GameObject origin, cut1, cut2, spice;
        [SerializeField] ItemAlpha itemAlphaOrigin, itemAlphaCut1, itemAlphaCut2, itemAlphaSpice;
        [BoxGroup("Cutting")][SerializeField] Transform maskSlide, maskPiece, startPoint, finishPoint;

        [SerializeField] HintText hintText;

        private float step = 0;

        public override void ChangeState<T>(T t)
        {
            state = (State)(object)t;

            switch (state)
            {
                case State.Normal:
                    break;
                case State.Cutting1:
                    this.OnSavePoint();
                    knife.OnMove(TF.position, Quaternion.identity, 0.2f);
                    break;

                case State.Cutting2:

                    knife.TF.DORotate(Vector3.zero, 0.1f);
                    //knife.ShowBack();
                    DOVirtual.DelayedCall(0.2f, () => knife.TF.DOMove(startPoint.position, 0.2f));
                    break;
                case State.Spice:
                    break;
                case State.Done:
                    hintText.OnActiveHint();
                    LevelControl.Ins.NextHint();
                    break;
                default:
                    break;
            }
        }

        public override void OnDrop()
        {
            base.OnDrop();
            OnBack();
        }

        public override void OnClickDown()
        {
            if (IsState(State.Cutting1))
            {
                OrderLayer = 1;

                splashVFX.Play();
                knife.ChangeAnim("KnifeChop");
                SoundControl.Ins.PlayFX(Fx.KnifeCut);

                chopVFX.Play();

                step += 1f / CHOP_STEP;
                itemAlphaOrigin.DoAlpha(1 - step, 0.1f);
                itemAlphaCut1.DoAlpha(step, 0.1f);

                if (step >= 1f)
                {
                    step = 0; // reset counter step
                    ChangeState(State.Cutting2);
                    itemAlphaCut1.DoAlpha(0, 0.2f);
                    itemAlphaCut2.DoAlpha(1, 0.2f);
                }
                return;
            }

            if (IsState(State.Cutting2))
            {
                OrderLayer = 1;

                knife.ChangeAnim("KnifeChop");
                SoundControl.Ins.PlayFX(Fx.KnifeCut);

                step += 1f / CUT_STEP;
                Vector2 point = Vector3.Lerp(startPoint.position, finishPoint.position, step);
                splashVFX.transform.position = point;
                splashVFX.Play();
                maskSlide.position = point;
                maskPiece.position = point;
                knife.TF.position = point;

                chopVFX.transform.position = point;
                chopVFX.Play();

                if (step >= 1f)
                {
                    step = 0; // reset counter step
                    ChangeState(State.Spice);
                    knife.ChangeState(Knife.State.Done);
                    knife.OnDrop();
                }

                return;
            }


            base.OnClickDown();
            SoundControl.Ins.PlayFX(Fx.Click);
        }

        public override void OnClickTake()
        {
            base.OnClickTake();
            SoundControl.Ins.PlayFX(Fx.Take);
        }
        public override bool IsState<T>(T t)
        {
            return state == (State)(object)t;
        }
    }
}