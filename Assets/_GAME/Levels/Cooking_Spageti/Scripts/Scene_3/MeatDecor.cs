using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link.Cooking.Spageti
{
    public class MeatDecor : ItemMovingBase
    {
        [SerializeField] Animation animScale;
        [SerializeField] ItemAlpha itemAlpha;
        public override bool IsCanMove => false;
        public override bool IsDone => !collider.enabled;

        public void SetAlpha(float alpha)
        {
            itemAlpha.SetAlpha(alpha);
        }

        public void SetActive(bool active)
        {
            collider.enabled = active;
        }

        public void SetScale()
        {
            SoundControl.Ins.PlayFX(Fx.Ping);
            animScale.Play("ItemScale");
        }

        public void SetScaleLight()
        {
            animScale.Play("ItemScaleLight");
        }

        public override void OnDone()
        {
            base.OnDone();
            gameObject.SetActive(false);
        }
    }
}