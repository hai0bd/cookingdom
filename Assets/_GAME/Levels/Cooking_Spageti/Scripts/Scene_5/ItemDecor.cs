using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Link.Cooking.Spageti
{
    public class ItemDecor : ItemMovingBase
    {
        public override bool IsDone => !collider.enabled;
        public ItemType ItemType;
        public UnityEvent<ItemDecor> onDoneEvent;

        [SerializeField] bool isActiveOnDone = true;

        [SerializeField] HintText hintText;

        public override void OnDone()
        {
            base.OnDone();
            gameObject.SetActive(isActiveOnDone);
            onDoneEvent?.Invoke(this);
            LevelControl.Ins.CheckStep(1f);
            hintText.OnActiveHint();
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