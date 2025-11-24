using System;
using System.Collections;
using System.Collections.Generic;
using Satisgame;
using UnityEngine;

namespace Link.Cooking.Spageti
{
    public class LevelStep_0 : LevelStepBase
    {
        [SerializeField] List<ItemDecor> itemDecors;
        [SerializeField] EmojiControl emoji;

        [SerializeField] HintText hintText;

        public override bool IsDone()
        {
            return itemDecors.Count == 0;
        }

        public override void OnFinish(Action action)
        {
            base.OnFinish(action);
            hintText.OnActiveHint();
        }

        public void OnDone(ItemDecor item)
        {
            itemDecors.Remove(item);
            if (IsDone())
            {
                emoji.ShowPositive();
            }
        }
    }
}