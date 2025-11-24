using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Link.Cooking.Spageti
{
    public class Board : ItemIdleBase
    {
        ItemMovingBase item;

        bool isHaveItem => item != null && Vector2.Distance(item.TF.position, TF.position) < 0.2f;

        public override bool OnTake(IItemMoving item)
        {
            if (item is Knife knife && isHaveItem)
            {
                knife.SetOrder(5);
                return this.item.OnTake(knife);
            }
            if (item is Tomato tomato && !isHaveItem && item.IsState(Tomato.State.Idle))
            {
                this.item = tomato;
                tomato.OnMove(TF.position, Quaternion.identity, 0.2f);
                item.ChangeState(Tomato.State.InBoard);
                item.OnSave(0.2f);
                LevelControl.Ins.SetHint(hints[items.IndexOf(tomato)]);
                return true;
            }
            if (item is Onion onion && !isHaveItem && item.IsState(Onion.State.Idle))
            {
                this.item = onion;
                onion.OnMove(TF.position, Quaternion.identity, 0.2f);
                item.ChangeState(Onion.State.InBoard);
                item.OnSave(0.2f);
                LevelControl.Ins.SetHint(hints[items.IndexOf(onion)]);
                return true;
            }   
            if (item is Leaf leaf && !isHaveItem && item.IsState(Leaf.State.Idle))
            {
                this.item = leaf;
                leaf.OnMove(TF.position, Quaternion.identity, 0.2f);
                item.ChangeState(Leaf.State.InBoard);
                item.OnSave(0.2f);
                LevelControl.Ins.SetHint(hints[items.IndexOf(leaf)]);
                return true;
            }
            if (item is Oliu oliu && !isHaveItem && item.IsState(Oliu.State.Idle))
            {
                this.item = oliu;
                oliu.OnMove(TF.position, Quaternion.identity, 0.2f);
                item.ChangeState(Oliu.State.InBoard);
                item.OnSave(0.2f);
                LevelControl.Ins.SetHint(hints[items.IndexOf(oliu)]);
                return true;
            }
            if (item is Bacon bacon && !isHaveItem && item.IsState(Bacon.State.Idle))
            {
                this.item = bacon;
                bacon.OnMove(TF.position, Quaternion.identity, 0.2f);
                item.ChangeState(Bacon.State.InBoard);
                item.OnSave(0.2f);
                LevelControl.Ins.SetHint(hints[items.IndexOf(bacon)]);
                return true;
            }
            return false;
        }

        public override void OnClickDown()
        {
            base.OnClickDown();
            if(isHaveItem && !item.IsState(Tomato.State.Pieced) && !item.IsState(Onion.State.Pieced))
            {
                if (!((item is Leaf && item.IsState(Leaf.State.Cut)) || item is Bacon))
                {
                    item.OnClickDown();
                }
            }
        }

        [SerializeField] ItemMovingBase[] items;
        [SerializeField] Sprite[] hints;

        internal void CheckHint()
        {
            for (int i = 0; i < items.Length; i++)
            {
                if(!items[i].IsDone)
                {
                    LevelControl.Ins.SetHint(hints[i]);
                }
            }
        }
    }
}