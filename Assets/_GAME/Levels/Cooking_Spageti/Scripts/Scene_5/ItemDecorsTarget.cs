using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link.Cooking.Spageti
{
    public class ItemDecorsTarget : ItemIdleBase
    {
        [SerializeField] Collider2D col;
        [SerializeField] ItemType itemType;

        public override bool OnTake(IItemMoving item)
        {
            if (item is ItemDecors decor && decor.ItemType == itemType)
            {
                item.TF.SetParent(transform);
                item.OnMove(TF.position, TF.rotation, 0.2f);
                item.OnDone();
                OnDone();
                return true;
            }
            return false;
        }

        public override void OnDone()
        {
            base.OnDone();
            col.enabled = false;
        }

        protected override void Editor()
        {
            base.Editor();
            col = GetComponent<Collider2D>();
        }
    }
}