using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link.Cooking.Spageti
{
    public enum Fx
    {
        Click,
        Take,
        Fry,
        Pour,
        Boil,
        StoveTurn,
        OpenLid,
        KnifeCut,
        Slash,
        Pepper,
        LeafCut,
        Blink,
        Ping,
        SpoonFry,
    }

    public class LevelStep_1 : LevelStepBase
    {
        [SerializeField] private Stove stove;
        [SerializeField] private Noodle Noodle;

        public override bool IsDone()
        {
            return Noodle.IsDone && stove.IsDone;
        }
        public override void OnFinish(Action action)
        {
            base.OnFinish(action);
        }
    }
}