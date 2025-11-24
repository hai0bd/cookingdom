using DG.Tweening;
using HoangHH;
using Satisgame;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Link.Cooking.Lobster
{
    public class Lobster : ItemMovingBase
    {
        public enum State { Dirty, NeedClean, Cleaned, InTrayCleaned, MoveToPot, InPot, Stream, InPlate, MoveToBoard, InBoard, Done }
        [SerializeField, ReadOnly] State state;
        public override bool IsCanMove => IsState(State.Dirty, State.Cleaned, State.MoveToPot, State.Stream, State.MoveToBoard);
        [SerializeField] EmojiControl emoji;
        [SerializeField] DrawP3DController drawController;
        [SerializeField] ParticleSystem sparkVFX, smoke, wetVFX;
        [SerializeField] ActionAnim animInPot;
        [SerializeField] GameObject[] lobsters;

        [SerializeField] GameObject headGO;
        [SerializeField] Floating2D floating2D;

        protected override void Start()
        {
            base.Start();
            floating2D.Active();
        }

        public override void ChangeState<T>(T t)
        {
            state = (State)(object)t;

            switch (state)
            {
                case State.Dirty:
                    break;
                case State.NeedClean:
                    drawController.P3dPaintDecal.gameObject.SetActive(true);
                    headGO.SetActive(false);
                    break;
                case State.Cleaned:
                    sparkVFX.Play();
                    SoundControl.Ins.PlayFX(LevelStep_1.Fx.Blink);
                    wetVFX.gameObject.SetActive(false);
                    OnSavePoint();
                    break;
                case State.InTrayCleaned:
                    LevelControl.Ins.SetHintTextDone(2, 1);
                    emoji.ShowPositive();
                    break;
                case State.MoveToPot:
                    break;
                case State.InPot:
                    TF.DOScale(0.875f, 0.2f);
                    animInPot.Active();
                    //TODO: Anim
                    break;
                case State.Stream:
                    smoke.Play();
                    break;
                case State.InPlate:
                    LevelControl.Ins.SetHintTextDone(4, 9);
                    break;
                case State.InBoard:
                    TF.DOScale(1.4f, 0.2f);
                    animInPot.Active();
                    break;
                case State.Done:
                    break;
                default:
                    break;
            }

            lobsters[(int)state - 1].gameObject.SetActive(false);
            lobsters[(int)state].gameObject.SetActive(true);
        }

        public override void OnClickDown()
        {
            if (IsState(State.Dirty))
            {
                wetVFX.gameObject.SetActive(true);
                floating2D.OnStop();
            }
            base.OnClickDown();
            if (IsState(State.Stream))
            {
                TF.DOScale(1, 0.2f);
            }

            SoundControl.Ins.PlayFX(LevelStep_1.Fx.Click);
        }

        public override void OnDrop()
        {
            base.OnDrop();
            if (state >= State.MoveToPot)
            {
                OrderLayer = 50;
                OnBack();
                DOVirtual.DelayedCall(0.3f, () => OrderLayer = 0);
            }
            if (IsState(State.Cleaned))
            {
                OnBack();
            }
        }

        public override void OnClickTake()
        {
            TF.DOScale(1, 0.1f);
            OrderLayer = LevelControl.Ins.GetHighestNoneContactLayer(this, TF.position, -8) + 1;
            SoundControl.Ins.PlayFX(LevelStep_1.Fx.Take);
        }

        public override bool IsState<T>(T t)
        {
            return state == (State)(object)t;
        }

        public void CleanDone()
        {
            IfChangeState(State.NeedClean, State.Cleaned);
            LevelControl.Ins.SetStep(LevelName.Lobster, Step.LobsterCleaned);
        }

        public override void OnDone()
        {
            base.OnDone();
            gameObject.SetActive(false);
            LevelControl.Ins.SetHintTextDone(5, 1);
        }

        public override bool OnTake(IItemMoving item)
        {
            if (IsState(State.NeedClean) && item is Knife) LevelControl.Ins.LoseFullHeart(TF.position);
            return base.OnTake(item);
        }
    }
}