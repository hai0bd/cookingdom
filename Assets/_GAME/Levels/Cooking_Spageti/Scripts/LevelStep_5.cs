using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link.Cooking.Spageti
{
    public class LevelStep_5 : LevelStepBase
    {
        [SerializeField] ItemMovingBase[] items;

        public override void OnStart()
        {
            base.OnStart();
            LevelControl.Ins.OnLoseEvent?.Invoke();
        }

        public override bool IsDone()
        {
            foreach (var item in items)
            {
                if(!item.IsDone) return false;
            }
            return true;
        }

    }
}