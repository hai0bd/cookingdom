using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Link.Cooking.Spageti
{
    public class OliuFail : ItemMovingBase
    {
        public UnityEvent<OliuFail> OnDoneEvent;
        public override bool IsCanMove => true;
        
        [SerializeField] Transform target;
        [SerializeField] float jumpForce = 1;

        [SerializeField] AudioClip clip;

        public void OnInit()
        {
            collider.enabled = true;
        }

        public override void OnClickDown()
        {
            base.OnClickDown();
            SoundControl.Ins.PlayFX(clip);
            // SoundControl.Ins.PlayFX(Fx.LeafCut);
        }

        public override void OnDrop()
        {
            base.OnDrop();
            TF.DOJump(target.position, jumpForce, 1, 0.5f).OnComplete(OnDone);
            TF.DORotate(target.eulerAngles, 0.5f);
            TF.DOScale(target.localScale, 0.5f);
            base.OnDone();
        }

        public override void OnDone()
        {
            base.OnDone();
            target.gameObject.SetActive(true);
            gameObject.SetActive(false);
            OnDoneEvent?.Invoke(this);
        }

        protected override void Editor()
        {
            base.Editor();
            collider.enabled = false;
        }
    }
}