using DG.Tweening;
using Link;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace HuyThanh.Cooking.TunaSaladFrench
{
    [System.Serializable]
    public class StepData
    {
        [HorizontalGroup()]
        public ActionBase item;
        [HorizontalGroup()]
        public float delay;

        public void Active()
        {
            item.Active();
        }
    }
    public class MiniGame1 : Singleton<MiniGame1>
    {
        [SerializeField] private StepData[] stepFinish;

        [Header("Checking done game")]
        [SerializeField] SaladBowl saladBowl;
        [SerializeField] List<BanhMi> banhMis;
        [SerializeField] List<Butter> butters;
        [SerializeField] List<Pearl> pearls;

        [SerializeField] Pearl pearl;

        [Header("Next Mini Game")]
        [SerializeField] MiniGame3 miniGame3;
        [SerializeField] MiniGame4 miniGame4;

        [SerializeField] Sprite hint_next_step;
        private bool haveWin = false;

        private void Start()
        {
            InitInstance();
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

        public void CheckDone()
        {
            if (CheckBanhMi() && CheckButter() && CheckPearl() && saladBowl.CheckDone() && haveWin == false)
            {
                OnFinish(null);
                miniGame3.OnStart();
                miniGame4.OnStart();
                LevelControl.Ins.SetHint(hint_next_step);
                haveWin = true;
            }
        }

        public void CheckDone(float time)
        {
            DOVirtual.DelayedCall(time, () => CheckDone());
        }

        private bool CheckBanhMi()
        {
            foreach (ItemMovingBase item in banhMis)
            {
                if (item.IsState(BanhMi.State.Done) == false)
                {
                    return false;
                }
            }
            return true;
        }
        private bool CheckButter()
        {
            foreach (ItemMovingBase item in butters)
            {
                if (item.IsState(Butter.State.Done) == false)
                {
                    return false;
                }
            }
            return true;
        }

        private bool CheckPearl()
        {
            foreach (ItemMovingBase item in pearls)
            {
                if (item.IsState(Pearl.State.Done) == false)
                {
                    return false;
                }
            }
            return true;
        }



    }
}