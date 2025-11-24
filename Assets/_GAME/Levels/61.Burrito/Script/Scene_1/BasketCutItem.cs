using DG.Tweening;
using Link;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HuyThanh.Cooking.Burrito
{
    public class BasketCutItem : ItemMovingBase
    {
        public enum State
        {
            Normal,
            Cutting1,
            Cutting2,
            Spice,
            Done
        }

        [SerializeField] State state;

        public override bool IsCanMove => IsState(State.Normal, State.Spice);

        [SerializeField] private int CHOP_STEP = 4;
        [SerializeField] private int CUT_STEP = 10;

        [SerializeField] Knife knife;
        [SerializeField] ParticleSystem splashVFX;
        [SerializeField] ParticleSystem chopVFX;

        [SerializeField] GameObject origin, cut1, cut2, spice;
        [BoxGroup("Cutting")][SerializeField] Transform maskSlide, maskPiece1, maskPiece2, maskSpice, startPoint, finishPoint;
        [SerializeField] HintText hintText;

        private float step = 0;


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
                case State.Spice:
                    break;
                case State.Cutting1:
                    this.OnSavePoint(); ///luu lai vi tri tren thot
                    knife.OnMove(startPoint.position, Quaternion.identity, 0.2f);
                    break;
                case State.Cutting2:
                    break;
                case State.Done:
                    SoundControl.Ins.PlayFX(Fx.DoneSomething);
                    hintText.OnActiveHint();
                    LevelControl.Ins.NextHint();
                    break;
            }
        }

        override public void OnClickDown()
        {

            if (IsState(State.Cutting1))
            {
                OrderLayer = 1;

                knife.ChangeAnim("KnifeChop");
                SoundControl.Ins.PlayFX(Fx.KnifeCut);

                step += 1f / CHOP_STEP;
                Vector2 point = Vector3.Lerp(startPoint.position, finishPoint.position, step);
                splashVFX.transform.position = point;
                splashVFX.Play();
                maskSlide.position = point;
                maskPiece1.position = point;
                knife.TF.position = point;
                knife.OrderLayer = 49;

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

            if (IsState(State.Cutting2))
            {
                OrderLayer = 1;

                knife.ChangeAnim("KnifeChop");
                SoundControl.Ins.PlayFX(Fx.KnifeCut);

                step += 1f / CUT_STEP;
                Vector2 point = Vector3.Lerp(startPoint.position, finishPoint.position, step);
                splashVFX.transform.position = point;
                splashVFX.Play();
                maskPiece2.position = point;
                maskSpice.position = point;
                knife.TF.position = point;
                knife.OrderLayer = 49;

                chopVFX.transform.position = point;
                chopVFX.Play();

                if (step >= 1f)
                {
                    step = 0; // reset counter step
                    ChangeState(State.Spice);
                    knife.ChangeState(Knife.State.Done);
                }

                return;
            }

            SoundControl.Ins.PlayFX(Fx.PutDown);
            base.OnClickDown();
            TF.DORotate(Vector3.zero, 0.1f);
        }

        public override void OnDrop()
        {
            base.OnDrop();
            //SoundControl.Ins.PlayFX(Fx.Take);
        }
    }

}