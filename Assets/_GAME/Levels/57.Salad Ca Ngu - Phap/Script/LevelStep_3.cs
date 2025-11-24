using Link;
using UnityEngine;

namespace HuyThanh.Cooking.TunaSaladFrench
{
    public class LevelStep_3 : LevelStepBase
    {
        [SerializeField] Oven oven;
        [SerializeField] PanOvenMoving panOvenMoving;
        public override bool IsDone()
        {
            return oven.IsState(Oven.State.Done) && panOvenMoving.IsState(PanOvenMoving.State.Done);
        }
    }

}