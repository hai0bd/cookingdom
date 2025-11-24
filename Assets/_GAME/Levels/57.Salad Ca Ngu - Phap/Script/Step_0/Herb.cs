using DG.Tweening;
using Link;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HuyThanh.Cooking.TunaSaladFrench
{
    public class Herb : ItemMovingBase
    {
        private const int CHOP_STEP = 3;
        private const int CUT_STEP = 10;
        public enum State { Origin, Cutting1, Cutting2, Cut2, Spice, Done }
        public override bool IsCanMove => IsState(State.Origin, State.Cut2, State.Spice);

        [SerializeField] State state;
        [SerializeField] GameObject itemActive;
        [SerializeField] Knife knife;
        [SerializeField] ParticleSystem splashVFX;
        [SerializeField] ParticleSystem chopVFX;

        [SerializeField] GameObject origin, cut1, cut2, spice;
        [SerializeField] ItemAlpha itemAlphaOrigin, itemAlphaCut1, itemAlphaCut2, itemAlphaSpice;
        [BoxGroup("Cutting")][SerializeField] Transform maskSlide, maskPiece1, maskPiece2, maskSpice, startPoint, finishPoint;


        private float step = 0;

        public override void ChangeState<T>(T t)
        {
            state = (State)(object)t;

            switch (state)
            {
                case State.Origin:
                    break;
                case State.Cutting1:

                    knife.ShowBack();
                    break;

                case State.Cutting2:

                    knife.TF.DORotate(Vector3.zero, 0.1f);
                    knife.ShowBack();
                    DOVirtual.DelayedCall(0.2f, () => knife.TF.DOMove(startPoint.position, 0.2f));
                    break;
                case State.Cut2:

                    break;
                case State.Spice:
                    break;
                case State.Done:

                    DOVirtual.DelayedCall(0.2f, () =>
                    {
                        collider.enabled = false; /// tat box khi da dung xong
                        spice.SetActive(false);                     ///TODO: sprite.color = 0 nua la duoc
                        itemActive.SetActive(true);
                    });

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
            if (IsState(State.Cutting2))
            {
                OrderLayer = 1;

                knife.ChangeAnim("cut");
                SoundControl.Ins.PlayFX(Fx.KnifeCutEgg);

                step += 1f / CUT_STEP;
                Vector2 point = Vector3.Lerp(startPoint.position, finishPoint.position, step);
                splashVFX.transform.position = point;
                splashVFX.Play();
                maskPiece2.position = point;
                maskSpice.position = point;
                knife.TF.position = point;

                chopVFX.transform.position = point;
                chopVFX.Play();

                if (step >= 1f)
                {
                    step = 0; // reset counter step
                    ChangeState(State.Spice);
                    step = 0;
                    knife.ChangeState(Knife.State.Ready);
                    knife.OnDrop();
                }

                return;
            }

            if (IsState(State.Cutting1))
            {
                OrderLayer = 1;

                knife.ChangeAnim("cut");
                SoundControl.Ins.PlayFX(Fx.KnifeCutEgg);

                step += 1f / CHOP_STEP;
                Vector2 point = Vector3.Lerp(startPoint.position, finishPoint.position, step);
                splashVFX.transform.position = point;
                splashVFX.Play();
                maskSlide.position = point;
                maskPiece1.position = point;
                knife.TF.position = point;

                chopVFX.transform.position = point;
                chopVFX.Play();

                if (step >= 1f)
                {
                    step = 0; // reset counter step
                    ChangeState(State.Cutting2);
                    cut1.SetActive(false);
                    cut2.SetActive(true);
                    step = 0;
                    //knife.ChangeState(Knife.State.Ready);
                    //knife.OnDrop();
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

