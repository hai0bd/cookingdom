using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Link.Cooking.Spageti
{
    public class Salt : ItemMovingBase
    {
        [SerializeField] GameObject raw;
        [SerializeField] AnimaBase2D animaBase;

        public override void OnDone()
        {
            base.OnDone();
            raw.SetActive(false);
            animaBase.gameObject.SetActive(true);
            animaBase.OnActive();
        }

        public override void OnDrop()
        {
            base.OnDrop();
            OnBack();
        }

        public override void OnClickDown()
        {
            base.OnClickDown();
            SoundControl.Ins.PlayFX(Fx.Click);
        }

        public override void OnClickTake()
        {
            base.OnClickTake();
            SoundControl.Ins.PlayFX(Fx.Pepper);
        }
    }
}
