using Link;
using UnityEngine;

namespace HuyThanh.Cooking.Burrito
{
    public class LevelStep_3 : LevelStepBase
    {
        [SerializeField] Crust crustTwo;
        [SerializeField] PanBurrito panBurrito;

        public override bool IsDone()
        {
            if (crustTwo.IsState(Crust.State.Done) == false)
                return false;
            if (panBurrito.IsState(PanBurrito.State.Done) == false)
                return false;
            return true;
        }
    }
}