using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link.Cooking.Lobster
{
    public class LevelStep_2 : LevelStepBase
    {
        [SerializeField] ElectricStove electricStove;
        [SerializeField] Pot pot;
        [SerializeField] Lobster lobsters;
        //[SerializeField] ActionBase actionBaseStove, actionBasePot;

        public override void OnStart()
        {
            base.OnStart();
            lobsters.ChangeState(Lobster.State.MoveToPot);
        }

        public override void OnFinish(Action action)
        {
            base.OnFinish(action);
            pot.OnStream();
        }

        public override bool IsDone()
        {
            //if (electricStove.IsOn && pot.IsFullItem && pot.IsLidClose && isDelay)
            //{
            //    electricStove.ChangeState(ElectricStove.State.NoContact);
            //    isDelay = false;
            //    actionBasePot.OnActive();
            //    actionBaseStove.OnActive();

            //    LevelControl.Ins.CheckStep(1.25f);
            //    return false;
            //}
            return electricStove.IsOn && pot.IsFullItem && pot.IsLidClose;
        }
    }
}