using Link;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hai.Cooking.Burrito
{
    public class LevelStep : LevelStepBase
    {
        [SerializeField] private Pan pan;
        [SerializeField] private Pot pot;

        [SerializeField] private BowlMeat bowlMeat;
        [SerializeField] private BowlPotItem bowlPotItem1,bowlPotItem2;
        [SerializeField] private Sprite hintBoil, hintFry1, hintFry2, hintFryDone;

        public override void NextHint()
        {
            if (bowlPotItem1.IsState(BowlPotItem.State.Done) == false || bowlPotItem2.IsState(BowlPotItem.State.Done) == false || pot.CheckDone() == false)
            {
                LevelControl.Ins.SetHint(hintBoil);
                return;
            }

            if (pan.IsState(Pan.State.Normal, Pan.State.HeatOn, Pan.State.HaveOil, Pan.State.HaveGarlic, Pan.State.Mixing1))
            {
                LevelControl.Ins.SetHint(hintFry1);
            }
            else if (pan.IsState(Pan.State.Meat, Pan.State.Chili, Pan.State.Chili, Pan.State.Peper, Pan.State.Tomato, Pan.State.Mixing2))
            {
                LevelControl.Ins.SetHint(hintFry2);
            }
            else if (pan.IsState(Pan.State.DoneMix2, Pan.State.WaitForTurnOff))
            {
                LevelControl.Ins.SetHint(hintFryDone);
            }


        }
        public override bool IsDone()
        {
            if(!bowlPotItem1.IsState(BowlPotItem.State.Done) && !bowlPotItem2.IsState(BowlPotItem.State.Done))
            {
                return false;
            }
            return pot.CheckDone() && pan.IsState(Pan.State.Done) && bowlMeat.CheckDone();
        }
    }
}
