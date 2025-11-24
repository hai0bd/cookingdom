using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link.Cooking.Spageti_Bear
{
    public class LevelStep_Scallop_4 : LevelStepBase
    {
        [SerializeField] Sprite hint_1;
        [SerializeField] PotMix potMix;

        public override bool IsDone()
        {
            return potMix.IsDone;
        }

    }
}
