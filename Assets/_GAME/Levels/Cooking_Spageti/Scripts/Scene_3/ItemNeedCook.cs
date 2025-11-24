using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link.Cooking.Spageti
{
    public class ItemNeedCook : ItemMovingBase
    {
        public PotFry.ItemCookType itemCookType;
        public override bool IsCanMove => base.IsCanMove;

        public override void OnDone()
        {
            base.OnDone();
            gameObject.SetActive(false);
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

        override public void OnClickTake()
        {
            base.OnClickTake();
            SoundControl.Ins.PlayFX(Fx.Take);
        }
    }
}
