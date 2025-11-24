using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Link.Cooking.Spageti
{
    public class ItemDecorTarget : ItemIdleBase
    {
        [SerializeField] ItemDecor itemDecor;
        [SerializeField] Collider2D col;


        public override bool OnTake(IItemMoving item)
        {
            if (item is ItemDecor && itemDecor.Equals(item))
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
            foreach (var item in FindObjectsOfType<ItemDecor>())
            {
                if (item == null) continue;
                if (item.name == name)
                {
                    itemDecor = item;
                    break;
                }
            }
        }
    }
}