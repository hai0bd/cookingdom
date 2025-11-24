using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Link;

namespace HoangLinh.Cooking.Test
{
    public class CuttingBoard : ItemIdleBase
    {
        [SerializeField] Animation anim;
        [SerializeField] string animCut;

        private IItemMoving item;
        public bool haveItem => item != null && Vector2.Distance(item.TF.position, TF.position) < 0.2f;

        public override bool OnTake(IItemMoving item)
        {
            if (!haveItem && (item is BasketCutItems || item is Meat) && item.IsState(BasketCutItems.State.Normal))
            {
                this.item = item;
                item.OnMove(TF.position + Vector3.up * 0.1f, Quaternion.identity, 0.2f);
                anim.Play(animCut);
                return true;
            }

            if (haveItem && item is Knife && this.item is BasketCutItems && this.item.IsState(BasketCutItems.State.Normal))
            {
                this.item.ChangeState(BasketCutItems.State.CutBig);
                item.ChangeState(Knife.State.Cutting);
                return true;
            }

            if (haveItem && item is Knife && this.item is Meat && this.item.IsState(Meat.State.Normal))
            {
                this.item.ChangeState(Meat.State.CutBig);
                item.ChangeState(Knife.State.Cutting);
                return true;

            }

            return base.OnTake(item);
        }

        public override void OnClickDown()
        {
            base.OnClickDown();

            if (haveItem && (item is BasketCutItems || item is Meat) && (item.IsState(BasketCutItems.State.CutBig)) || item.IsState(BasketCutItems.State.CutSmall) || item.IsState(Meat.State.CutBig))
            {
                item.OnClickDown();
                return;
            }

        }
    }
}

