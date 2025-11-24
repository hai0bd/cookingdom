using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Link.Cooking.Spageti
{
    public class MushroomPoint : ItemIdleBase
    {
        [field:SerializeField] public Mushroom.MushroomType Type { get; private set; }
        [SerializeField] SortingGroup sortingGroup;
        [SerializeField] Collider2D col;

        public override bool OnTake(IItemMoving item)
        {
            if (item is Mushroom mushroom && mushroom.Type == Type)
            {
                item.SetOrder(sortingGroup.sortingOrder);
                item.OnMove(TF.position, TF.rotation, 0.2f);
                item.OnDone();
                OnDone();
                return true;
            }
            return base.OnTake(item);
        }

        public override void OnDone()
        {
            base.OnDone();
            col.enabled = false;
        }

        protected override void Editor()
        {
            base.Editor();
            if (col == null)
            {
                col = GetComponent<Collider2D>();
            }
            if (sortingGroup == null)
            {
                sortingGroup = GetComponent<SortingGroup>();
            }
        }
    }
}