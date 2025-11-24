using DG.Tweening;
using Satisgame;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link.Cooking.Lobster
{
    public class LevelStep_5 : LevelStepBase
    {
        [SerializeField] WoodBowl woodBowl;
        [SerializeField] LobsterPiece piece_1, piece_2;
        [SerializeField] AnimaBase2D[] anima2Ds;

        [SerializeField] EmojiControl emoji;
        [SerializeField] List<DecorePoint> decorePoints;
        [SerializeField] ParticleSystem finishVFX;

        [SerializeField] Transform complete;

        public override void OnStart()
        {
            base.OnStart();
            woodBowl.ChangeState(WoodBowl.State.Moving);

            for (int i = 0; i < anima2Ds.Length; i++)
            {
                anima2Ds[i].transform.localScale = Vector3.zero;
            }

            DOVirtual.DelayedCall(1.5f + delayStartAction, () =>
            {
                for (int i = 0; i < anima2Ds.Length; i++)
                {
                    anima2Ds[i].transform.localScale = Vector3.one;
                    anima2Ds[i].OnActive();
                }
            });

            foreach (var item in decorePoints)
            {
                item.OnDoneAction = OnDone;
            }
        }

        public override void OnFinish(Action action)
        {
            complete.DOMoveY(0.75f, 0.5f).SetDelay(1f);
            complete.DOScale(1.25f, 0.5f).SetDelay(1f).OnComplete(() =>
            {
                finishVFX.Play();
                SoundControl.Ins.PlayFX(LevelStep_1.Fx.Blink);
            });

            base.OnFinish(action);
        }

        public override bool IsDone()
        {
            return decorePoints.Count <= 0;
        }

        private void OnDone(DecorePoint point)
        {
            decorePoints.Remove(point);
            if (decorePoints.Count <= 0)
            {
                emoji.ShowPositive();
                LevelControl.Ins.CheckStep(1.5f);
            }
            else
            if (decorePoints.Count <= 2 || (decorePoints.Count <= 3 && !woodBowl.IsDone))
            {
                piece_1.ChangeState(LobsterPiece.State.Decore);
                piece_2.ChangeState(LobsterPiece.State.Decore);
            }

        }
    }
}