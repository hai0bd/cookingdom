using Link;
using UnityEngine;

namespace HuyThanh.Cooking.TunaSaladFrench
{
    public class LevelStep_4 : LevelStepBase
    {
        [SerializeField] DoneMix doneMix;
        [SerializeField] Tuna tuna;
        public override bool IsDone()
        {
            return doneMix.IsState(DoneMix.State.Done);
        }
    }
}