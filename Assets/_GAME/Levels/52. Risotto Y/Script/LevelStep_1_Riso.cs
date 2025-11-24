using DG.Tweening;
using Link;
using Satisgame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace LawNguyen.CookingGame.LRisottoY.Level
{
    public class LevelStep_1_Riso : LevelStepBase
    {
        [SerializeField] WaterSink waterSink;


        public override bool IsDone()
        {
           
            return waterSink.IsdoneSence1;
        }


        public override void OnFinish(Action action)
        {
            
            DOVirtual.DelayedCall(0.75f, () =>
            {
                //delay xong chuyen man
                base.OnFinish(action);
                LevelControl.Ins.BlockControl(0.5f);
                transform.DOMove(Vector3.left * 7, 1f).OnComplete(() =>
                {
                   
                    gameObject.SetActive(false);
                      
                });
             
            });
        }



    }
}

