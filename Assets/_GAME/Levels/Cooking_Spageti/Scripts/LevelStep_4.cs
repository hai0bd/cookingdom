using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Link.Cooking.Spageti
{
    public class LevelStep_4 : LevelStepBase
    {
        [SerializeField] MeatDecor meatDecor;

        private void Start() 
        {
            meatDecor = FindObjectOfType<MeatDecor>();    
        }

        public override bool IsDone()
        {
            return meatDecor.IsDone;
        }
        public override void OnStart()
        {
            base.OnStart();
            DOVirtual.DelayedCall(1f, ()=> {meatDecor.SetActive(true);});
        }
        public override void OnFinish(Action action)
        {
            base.OnFinish(action);
        }
    }
}