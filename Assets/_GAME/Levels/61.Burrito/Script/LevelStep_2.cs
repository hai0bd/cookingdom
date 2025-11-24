using Link;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HuyThanh.Cooking.Burrito
{
    public class LevelStep_2 : LevelStepBase
    {
        [SerializeField] Pan pan;
        [SerializeField] Pot pot;

        [SerializeField] BowlPotItem potItem1, potItem2;
        [SerializeField] BowlMeat bowlMeat;

        [FoldoutGroup("Hint")][SerializeField] Sprite hint_boil, hint_fry_1, hint_fry_2, hint_fry_done;

        public override void NextHint()
        {
            if (potItem1.IsState(BowlPotItem.State.Done) == false || potItem2.IsState(BowlPotItem.State.Done) == false || pot.CheckDone() == false)
            {
                LevelControl.Ins.SetHint(hint_boil);
                return;
            }

            if (pan.IsState(Pan.State.Normal, Pan.State.HeatOn, Pan.State.HaveOil, Pan.State.HaveGarlic, Pan.State.Mixing1))
            {
                LevelControl.Ins.SetHint(hint_fry_1);
            }
            else if (pan.IsState(Pan.State.Meat, Pan.State.Pepper, Pan.State.Chili, Pan.State.Salt, Pan.State.Turmeric, Pan.State.Tomato, Pan.State.Oregano, Pan.State.Mixing2))
            {
                LevelControl.Ins.SetHint(hint_fry_2);
            }
            else if (pan.IsState(Pan.State.DoneMix2, Pan.State.WaitForTurnOff))
            {
                LevelControl.Ins.SetHint(hint_fry_done);
            }
        }

        public override bool IsDone()
        {
            if (potItem1.IsState(BowlPotItem.State.Done) == false || potItem2.IsState(BowlPotItem.State.Done) == false)
            {
                return false;
            }
            return pot.CheckDone() && pan.IsState(Pan.State.Done) && bowlMeat.CheckDone();
        }
    }

}