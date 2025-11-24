using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Link.Cooking.Spageti
{
    public class LeafFail : ItemMovingBase
    {
        public Action<LeafFail> OnDoneAction;
        [SerializeField] Rigidbody2D rb;
        public override bool IsCanMove => true;

        public void OnActive()
        {
            collider.enabled = true;
        }

        public override void OnDrop()
        {
            base.OnDrop();
            OnDone();
        }

        public override void OnDone()
        {
            base.OnDone();
            TF.SetParent(null);
            rb.bodyType = RigidbodyType2D.Dynamic;
            OnDoneAction?.Invoke(this);
        }

        protected override void Editor()
        {
            base.Editor();
            rb = GetComponent<Rigidbody2D>();
        }
    }
}
