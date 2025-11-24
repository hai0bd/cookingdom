using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Satisgame;
using UnityEngine;

namespace Link.Cooking.Spageti
{
    public class LevelStep_Bear_1 : LevelStepBase
    {
        [SerializeField] EmojiControl emoji;
        [SerializeField] ItemMovingBase[] items;

        [SerializeField] HintText hintText;

        public override bool IsDone()
        {
            foreach (var item in items)
            {
                if (!item.IsDone)
                {
                    return false;
                }
            }

            emoji.ShowPositive();
            return true;
        }

        public override void OnFinish(Action action)
        {
            base.OnFinish(action);
            hintText.OnActiveHint();
        }
    }
}