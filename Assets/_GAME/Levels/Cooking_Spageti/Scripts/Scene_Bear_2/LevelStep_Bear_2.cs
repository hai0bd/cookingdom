using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Link.Cooking.Spageti_Bear
{
    public class LevelStep_Bear_2 : LevelStepBase
    {
        [SerializeField] PotMix potMix;
        [SerializeField] Stove stove;

        public override bool IsDone()
        {
            // Check if the level step is done
            return potMix.IsState(PotMix.State.Done) && stove.IsDone; // Replace with actual condition to check if the level step is done
        }

        public override void OnFinish(Action action)
        {
            base.OnFinish(action);
            DOVirtual.DelayedCall(2f, ()=>{
                potMix.ChangeState(PotMix.State.MoveDish);
            });
        }
    }
}