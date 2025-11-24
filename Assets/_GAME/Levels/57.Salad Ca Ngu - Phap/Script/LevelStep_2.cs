using Link;
using UnityEngine;

namespace HuyThanh.Cooking.TunaSaladFrench
{
    public class LevelStep_2 : LevelStepBase
    {
        [SerializeField] SaladBowl saladBowl;

        public override bool IsDone()
        {
            return false;
        }
    }
}