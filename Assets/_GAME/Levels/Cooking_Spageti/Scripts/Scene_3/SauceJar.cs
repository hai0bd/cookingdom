using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Link.Cooking
{
    public class SauceJar : ItemMovingBase
    {
        [SerializeField] AudioClip clip;
        public override bool IsDone => !collider.enabled;
        [SerializeField] Animation anim;
        [SerializeField] HintText hintText;

        public override void OnDone()
        {
            base.OnDone();
            anim.Play();
            DOVirtual.DelayedCall(2f, OnBack);
            // SoundControl.Ins.PlayFX(Fx.Pour, 1f);
            if (clip != null)
            {
                SoundControl.Ins.PlayFX(clip, 1f);
            }
            hintText.OnActiveHint();
        }

        public override void OnDrop()
        {
            base.OnDrop();
            OnBack();
        }

        public override void OnBack()
        {
            base.OnBack();
            LevelControl.Ins.CheckStep(0.5f);
        }
    }
}