using Link;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hai.Cooking.NewTest
{
    public class StepCook : LevelStepBase
    {
        [SerializeField] private Pan pan;
        [SerializeField] private Sprite hint_prepare, hint_fry, hint_done;

        public static event Action OnLevelEnd;


        public override void NextHint()
        {
            if (pan.IsState(Pan.State.HeatOn))
            {
                LevelControl.Ins.SetHint(hint_prepare);
            }
            else if (pan.IsState(Pan.State.Mixing))
            {
                LevelControl.Ins.SetHint(hint_fry);
            }
            else if (pan.IsState(Pan.State.Done))
            {
                LevelControl.Ins.SetHint(hint_done);
            }
        }
        public override bool IsDone()
        {
            if(pan.IsState(Pan.State.Done))
            {
                OnLevelEnd.Invoke();
                return true;
            }
            return false;
        }
    }
}