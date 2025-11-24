using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link.Turkey
{
    public class LevelStep_2 : LevelStepBase
    {
        //public enum FX { Hold, Put, Toggle, Fry, Spoon }

        [SerializeField] Chicken chicken;
        [SerializeField] Pan pan;
        [SerializeField] Transform plateTF;
        [SerializeField] Spoon spoon;
        [SerializeField] Fruit[] fruits;
        [SerializeField] ItemMovingBase[] itemMovingBases;

        public override void OnStart()
        {
            transform.position = Vector3.right * 7;
            gameObject.SetActive(true);
            transform.DOMove(Vector3.zero, 1f).OnComplete(() =>
            {
                chicken.ChangeState(Chicken.State.Prepare);
                for (int i = 0; i < fruits.Length; i++)
                {
                    fruits[i].ChangeState(Fruit.State.Mix);
                }
                for (int i = 0; i < itemMovingBases.Length; i++)
                {
                    itemMovingBases[i].OnSavePoint();
                }
            });

            for (int i = 0; i < fruits.Length; i++)
            {
                fruits[i].OnSavePoint();
            }
        }

        public override void OnFinish(Action action)
        {
            spoon.ChangeState(Spoon.State.Done);
            DOVirtual.DelayedCall(0.75f, () =>
            {
                //delay xong chuyen man
                base.OnFinish(action);
                plateTF.SetParent(transform);
                LevelControl.Ins.BlockControl(0.5f);
                transform.DOMove(Vector3.left * 7, 1f).OnComplete(() => gameObject.SetActive(false));

            });
        }

        public override bool IsDone()
        {
            return chicken.IsState(Chicken.State.Marinated) && pan.IsDone;
        }
    }
}