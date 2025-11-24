using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link.Cooking.Lobster
{
    public class LevelStep_1 : LevelStepBase
    {
        public enum Fx 
        { 
            Click, 
            Take, 
            MultiTake, 
            ElectricTurn, 
            StreamLid,
            BeerCrack, 
            CleanLobster, 
            Pour, 
            Mix, 
            Move, 
            Fry , 
            Slash, 
            PowerSlash, 
            KnifeCut, 
            BottleCap,
            Blink,
        }

        [SerializeField] Tray tray;
        [SerializeField] Bowl[] bowls;

        public override bool IsDone()
        {
            for (int i = 0; i < bowls.Length; i++)
            {
                if (!bowls[i].IsDone)
                {
                    return false;
                }
            }
            LevelControl.Ins.SetHintTextDone(2, 2);

            return tray.IsDone;
        }
    }
}