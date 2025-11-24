using DG.Tweening;
using Link;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link.Turkey
{
    public class LevelStep_3 : LevelStepBase
    {
        //public enum FX { Hold, Put, Toggle, Clock, Ting }

        [SerializeField] Chicken chicken;
        [SerializeField] Transform ovenTF, plateTF;
        public override void OnStart()
        {
            transform.position = Vector3.right * 7;
            gameObject.SetActive(true);
            transform.DOMove(Vector3.zero, 1f).OnComplete(() =>
            {
                chicken.ChangeState(Chicken.State.Grill);
            });

            chicken.TF.DOMove(new Vector3(0, -2.25f, 0), 0.5f);
        }

        public override void OnFinish(Action action)
        {
            DOVirtual.DelayedCall(1, () =>
            {
                ovenTF.DOMoveY(10, 1f).OnComplete(()=> gameObject.SetActive(false));
                plateTF.DOMoveX(-10, 1);
                base.OnFinish(action);
            });
        }

        public override bool IsDone()
        {
            return chicken.IsState(Chicken.State.Ripe);
        }

    }
}
