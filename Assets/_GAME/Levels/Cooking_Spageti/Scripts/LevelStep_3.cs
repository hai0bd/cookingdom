using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Link.Cooking.Spageti
{
    public class LevelStep_3 : LevelStepBase
    {
        [SerializeField] Stove stove;
        [SerializeField] PotFry potFry;

        [SerializeField] GameObject lose;
        [SerializeField] SpriteRenderer loseSpr;
            
        [SerializeField] ActionBase[] itemMoveOuts;

        public override bool IsDone()
        {
            return potFry.IsDone && stove.IsDone;
        }

        public override void OnStart()
        {
            base.OnStart();
            LevelControl.Ins.OnLoseEvent.AddListener(NextHint);
        }
        public override void OnFinish(Action action)
        {
            base.OnFinish(action);
        }

        public override void OnLose()
        {
            LevelControl.Ins.OnLoseEvent.RemoveListener(NextHint);

            base.OnLose();
            loseSpr.color = Color.clear;
            loseSpr.gameObject.SetActive(true);
            loseSpr.DOFade(0.75f, 0.5f).OnComplete(() =>
            {
                lose.SetActive(true);
            });
        }

        public override void NextHint()
        {
            base.NextHint();
            foreach (var item in itemMoveOuts)
            {
                item.Active();
            }
        }
    }
}