using DG.Tweening;
using Link;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link.Turkey
{
    public class LevelStep_4 : LevelStepBase
    {
        [SerializeField] Chicken chicken;
        [SerializeField] DecorPoint point_1, point_2;
        [SerializeField] ItemMovingBase[] itemMovingBases;
        [SerializeField] Transform plate, plateBG;
        [SerializeField] ParticleSystem doneVFX;

        public override void OnStart()
        {
            gameObject.SetActive(true);
            plate.position = Vector3.up * -10;
            plate.DOMove(Vector3.up * -0.75f, 1f);
            doneVFX.Stop();

            foreach (var item in itemMovingBases)
            {
                item.TF.position += Vector3.up * 10f;
                item.OnMove(item.TF.position - Vector3.up * 10f, Quaternion.Euler(0, 0, UnityEngine.Random.Range(0, 360)), 1.5f);
            }
            chicken.TF.DOMoveX(0, 1f);
        }

        public override void OnFinish(Action action)
        {
            doneVFX.Play();
            plate.DOMove(Vector3.up * 0.5f, 1f).SetDelay(0.3f);
            plate.DOScale(Vector3.one * 1.4f, 1f).SetDelay(0.3f);
            plateBG.DOMoveX(-10, 0.5f);
            base.OnFinish(action);
        }

        public override bool IsDone()
        {
            if (point_1.IsDone && point_2.IsDone && chicken.IsState(Chicken.State.Ripe))
            {
                chicken.ChangeState(Chicken.State.Decore);
            }

            return chicken.IsState(Chicken.State.Done) && point_1.IsDone && point_2.IsDone;
        }

    }
}
