using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Link.Cooking.Spageti
{
    public class ItemDecors : ItemMovingBase
    {
        public override bool IsCanMove => base.IsCanMove;
        public override bool IsDone => !collider.enabled;

        public UnityEvent<ItemDecors> unityEvent;

        public ItemType ItemType;

        public override void OnDone()
        {
            base.OnDone();
            LevelControl.Ins.CheckStep(0.5f);
            unityEvent?.Invoke(this);
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