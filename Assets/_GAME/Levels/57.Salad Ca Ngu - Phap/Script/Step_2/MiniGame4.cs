using DG.Tweening;
using Link;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace HuyThanh.Cooking.TunaSaladFrench
{
    public class MiniGame4 : MiniGame3
    {
        [BoxGroup("Deactive Object On Start")][SerializeField] private List<GameObject> deactiveObject;
        public override void OnStart()
        {
            base.OnStart();
            LevelControl.Ins.BlockControl(delayStartAction + 1f);
            DOVirtual.DelayedCall(delayStartAction + 1f, () => DeactiveOnStart());
        }
        private void DeactiveOnStart() /// tat nhung vat pham cua cua scene2
        {
            foreach (GameObject gO in deactiveObject)
            {
                gO.SetActive(false);
            }
        }
    }

}