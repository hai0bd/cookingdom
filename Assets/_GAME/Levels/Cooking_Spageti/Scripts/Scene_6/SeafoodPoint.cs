using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link.Cooking.Spageti
{
    public class SeafoodPoint : ItemIdleBase
    {
        [field:SerializeField] public Seafood.SeafoodType Type { get; private set; }
        [SerializeField] Collider2D col;
        public override bool IsDone => !col.enabled;

        [SerializeField] Seafood.State isState, changeState;

        [SerializeField] Seafood seafoodRoot;

        public override bool OnTake(IItemMoving item)
        {
            if (item is Seafood seafood && seafood.Type == Type && seafood.IsState(isState))
            {
                if (seafoodRoot != null && seafoodRoot != seafood)
                {
                    return false;
                }
                seafood.ChangeState(changeState);
                seafood.TF.SetParent(TF);
                seafood.OnMove(TF.position, TF.rotation, 0.2f);
                seafood.OnDone();
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
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}
