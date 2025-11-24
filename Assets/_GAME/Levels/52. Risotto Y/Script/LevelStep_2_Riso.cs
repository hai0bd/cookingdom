using DG.Tweening;
using Link;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace LawNguyen.CookingGame.LRisottoY.Level
{
    public class LevelStep_2_Riso : LevelStepBase
    {

        [SerializeField] Bowlrice bowlrice;
        [SerializeField] StoveTap stoveTap;

        public override bool IsDone()
        {
        
            if (!bowlrice.IsDone) return false;
            if (!stoveTap.IsDone) return false;

            return true;
        }

        public override void OnFinish(Action action)
        {

            DOVirtual.DelayedCall(0.75f, () =>
            {
                //delay xong chuyen man
                base.OnFinish(action);
                LevelControl.Ins.BlockControl(0.5f);
                transform.DOMove(Vector3.down * 12, 1f).OnComplete(() => gameObject.SetActive(false));
            });
        }



    }
}