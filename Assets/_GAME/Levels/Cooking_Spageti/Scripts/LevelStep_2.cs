using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link.Cooking.Spageti
{
    public class LevelStep_2 : LevelStepBase
    {
        [SerializeField] ItemMovingBase[] items;

        public override bool IsDone()
        {
            foreach (var item in items)
            {
                if (!item.IsDone) return false;
            }
            return true;
        }
        public override void OnStart()
        {
            base.OnStart();
        }
        public override void OnFinish(Action action)
        {
            base.OnFinish(action);
        }
    }
}