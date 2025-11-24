using Link;
using Satisgame;
using System.Collections.Generic;
using UnityEngine;

namespace HuyThanh.Cooking.Burrito
{
    public class Basket : ItemIdleBase
    {
        [SerializeField] List<ItemMovingBase> basketItem;

        [SerializeField] Animation anim;
        [SerializeField] string animGetItem;

        [SerializeField] EmojiControl _emoji;

        public override bool OnTake(IItemMoving item)
        {
            if (item is TrashItem || item is GarlicRotten)
            {
                LevelControl.Ins.LoseHalfHeart(TF.position);
                return false;
            }

            if (IsGetItem(item) && item is DirtyItem && item.IsState(DirtyItem.State.Dirty))
            {
                LevelControl.Ins.LoseHalfHeart(TF.position);
                return false;
            }

            if (IsGetItem(item) && item is DirtyItem && item.IsState(DirtyItem.State.Normal))
            {
                item.ChangeState(DirtyItem.State.Done);
                RemoveGetItem(item);
                anim.Play(animGetItem);
                return true;
            }

            if (IsGetItem(item) && item is Garlic && item.IsState(Garlic.State.Normal))
            {
                item.ChangeState(Garlic.State.Done);
                RemoveGetItem(item);
                anim.Play(animGetItem);
                return true;
            }

            if (IsGetItem(item) && item is Corn && item.IsState(Corn.State.Normal))
            {
                item.ChangeState(Corn.State.Done);
                RemoveGetItem(item);
                anim.Play(animGetItem);
                return true;
            }

            return base.OnTake(item);
        }

        private bool IsGetItem(IItemMoving item)
        {
            return basketItem.Contains(item as ItemMovingBase);
        }

        private void RemoveGetItem(IItemMoving item)
        {
            basketItem.Remove(item as ItemMovingBase);

            if (basketItem.Count == 0)
            {
                _emoji.ShowPositive();
                LevelControl.Ins.CheckStep(1f);
            }
        }

        public bool CheckDone()
        {
            return basketItem.Count == 0;
        }
    }
}