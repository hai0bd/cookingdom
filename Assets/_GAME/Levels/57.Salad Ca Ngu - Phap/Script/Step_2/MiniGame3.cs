using DG.Tweening;
using Link;
using Sirenix.OdinInspector;
using System;
using UnityEngine;
namespace HuyThanh.Cooking.TunaSaladFrench
{
    public class MiniGame3 : MonoBehaviour
    {
        [SerializeField] protected float delayStartAction = 0;

        [SerializeField] private StepData[] stepStart;
        [SerializeField] private StepData[] stepFinish;

        [SerializeField] MiniGame5 miniGame5;
        [SerializeField] Pan pan;
        [SerializeField] PanOvenIdle ovenIdle;

        [Button]
        public virtual void OnStart()
        {
            DOVirtual.DelayedCall(delayStartAction, () =>
            {
                gameObject.SetActive(true);
                if (stepStart.Length > 0)
                {
                    foreach (var item in stepStart)
                    {
                        item.Active();
                    }
                }
            });

            LevelControl.Ins.BlockControl(1f + delayStartAction);
        }

        [Button]
        public virtual void OnFinish(Action action)
        {
            if (stepFinish.Length > 0)
            {
                foreach (var item in stepFinish)
                {
                    item.Active();
                }
            }
            action?.Invoke();
        }

        public void CheckNewGame()
        {
            if (pan.IsState(Pan.State.Done) && ovenIdle.IsState(PanOvenIdle.State.DoneSpice))
            {
                //OnFinish(null);
                //miniGame5.OnStart();
            }

        }
    }
}