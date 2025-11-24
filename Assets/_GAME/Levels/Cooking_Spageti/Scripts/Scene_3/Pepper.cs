using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Link.Cooking
{

    public class Pepper : ItemMovingBase
    {
        [SerializeField] AudioClip clip;
        [SerializeField] Animation anim;
        public override bool IsCanMove => base.IsCanMove;

        public override void OnDone()
        {
            base.OnDone();
            anim.Play();
            SetOrder(50);
            DOVirtual.DelayedCall(2f, OnBack);
            // SoundControl.Ins.PlayFX(Fx.Pepper, 1f);
            if(clip != null)
            {
                SoundControl.Ins.PlayFX(clip, 1f);
            }
        }

        public override void OnDrop()
        {
            base.OnDrop();
            OnBack();
        }

        public override void OnBack()
        {
            base.OnBack();
            DOVirtual.DelayedCall(0.2f, () =>
            {
                SetOrder(0);
            });
        }
    }
}