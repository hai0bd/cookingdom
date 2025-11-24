using DG.Tweening;
using Link;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace LawNguyen.CookingGame.LRisottoY
{
    public class LevelStep_3_Riso : LevelStepBase
    {
        [Header("Plate")]
        [SerializeField] Plateend[] plateendAR;
        [Header("StoveTapon")]
        [SerializeField] StoveTapOn stoveTapOn;
        [Header("BG")]
        [SerializeField] GameObject BG123;
        [SerializeField] GameObject BG4;
        public override bool IsDone()
        {
            foreach (var plateend in plateendAR)
            {
                if (plateend.IsDone == false) return false;
            }
           if(  stoveTapOn.Ison==true) return false; 

            return true;
        }
        public override void OnFinish(Action action)
        {

            DOVirtual.DelayedCall(0.75f, () =>
            {
                //delay xong chuyen man
                base.OnFinish(action);
                LevelControl.Ins.BlockControl(0.5f);
                BG123.SetActive(false);
                BG4.SetActive(true);
                transform.DOMove(Vector3.left * 12, 1f).OnComplete(() => {
                    gameObject.SetActive(false);
                   
                    
                    });

            });
        }


    }



}
