using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link.Cooking.Spageti
{
    public class LevelStep_Scallop_2 : LevelStepBase
    {
        [SerializeField] Sprite hint_1;
        [SerializeField] Scallop[] scallops;

        public override bool IsDone()
        {
            foreach (var scallop in scallops)
            {
                if (scallop.IsState(Scallop.State.Dirty)) return false;
            }
            LevelControl.Ins.SetHint(hint_1);
            foreach (var scallop in scallops)
            {
                if (!scallop.IsState(Scallop.State.Done)) return false;
            }
            return true;
        }

    }
}
