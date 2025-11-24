using DG.Tweening;
using Satisgame;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link.Turkey
{
    public class Oven : ItemIdleBase
    {
        public enum State { Start, Grill, Grilled, Done }
        State state;
        [SerializeField] Collider2D collider;
        [SerializeField] GameObject doorOpen, doorClose, doorCloseEmty;
        [SerializeField] Transform chickenTF, buttonTimeTF;
        [SerializeField] EmojiControl emoji;
        [SerializeField] EmojiControl[] emojis;
        [SerializeField] ParticleSystem vfx;
        Chicken chicken;

        bool isDoorClose = true;
        int index = 0;

        public void OpenDoor()
        {
            if (IsState(State.Grill) || IsState(State.Done)) return;
            isDoorClose = !isDoorClose;
            doorCloseEmty.SetActive(false);
            doorClose.SetActive(false);
            doorOpen.SetActive(!isDoorClose);
            //doorClose.SetActive(isDoorClose);
            if (chicken != null)
            {
                chicken.Collider.enabled = !isDoorClose;
                doorClose.SetActive(isDoorClose);
            }
            else
            {
                doorCloseEmty.SetActive(isDoorClose);
            }

            if (IsState(State.Start))
            {
                CheckGrill();
            }

            if (IsState(State.Grilled))
            {
                ChangeState(State.Done);
                isDoorClose = false;
                doorOpen.SetActive(!isDoorClose);
                doorClose.SetActive(isDoorClose);

                LevelControl.Ins.CheckStep();
                vfx.Play();
            }
            collider.enabled = !isDoorClose;

            SoundControl.Ins.PlayFX(LevelStep_1.FX.Toggle);
        }

        public void ClickButton()
        {
            index++;
            CheckGrill();
            SoundControl.Ins.PlayFX(LevelStep_1.FX.Toggle);
        }

        private void CheckGrill()
        {
            if (isDoorClose && index == 3 && chicken != null)
            {
                foreach (var item in emojis)
                {
                    item.ShowPositive();
                }
                ChangeState(State.Grill);

            }
        }

        public override void ChangeState<T>(T t)
        {
            this.state = (State)(object)(t);

            switch (state)
            {
                case State.Start:
                    break;
                case State.Grill:
                    TF.DOShakePosition(100, 0.01f, 10).SetEase(Ease.Linear).SetDelay(0.5f);
                    DOVirtual.DelayedCall(0.7f, () =>
                    {
                        SoundControl.Ins.PlayFX(LevelStep_1.FX.Clock, true);
                        SoundControl.Ins.PlayFX(LevelStep_1.FX.Oven, true);
                        buttonTimeTF.DORotate(Vector3.zero, 7f).OnComplete(() =>
                        {
                            SoundControl.Ins.StopFX(LevelStep_1.FX.Oven);
                            SoundControl.Ins.StopFX(LevelStep_1.FX.Clock);
                            SoundControl.Ins.PlayFX(LevelStep_1.FX.Ting);
                            ChangeState(State.Grilled);
                        });
                    });
   
                    break;
                case State.Grilled:
                    TF.DOKill();
                    chicken.ChangeState(Chicken.State.Ripe);
                    emoji.ShowPositive();
                    break;
                case State.Done:
                    break;
                default:
                    break;
            }
        }

        public override bool IsState<T>(T t)
        {
            return this.state == (State)(object)(t);
        }

        public override bool OnTake(IItemMoving item)
        {
            if (item is Chicken && !isDoorClose)
            {
                chicken = (item as Chicken);
                chicken.TF.DOKill();
                chicken.TF.DOMove(chickenTF.position, 0.2f);
                chicken.TF.DOScale(Vector3.one * 0.85f, 0.2f);
                chicken.ChangeState(Chicken.State.Grilling);
                return true;
            }
            return false;
        }
    }
}