using Link;
using UnityEngine;

namespace HuyThanh.Cooking.Burrito
{

    public class BottleItemTarget : ItemIdleBase
    {
        [SerializeField] BottleItemType type;
        [SerializeField] Animation anim;
        [SerializeField] string animSpice;
        public override bool OnTake(IItemMoving item)
        {
            if (item is BottleItem bottleItem && bottleItem.IsType(type))
            {
                anim.Play(animSpice);
                bottleItem.OnMove(TF.position, TF.rotation, 0.2f);
                bottleItem.ChangeState(BottleItem.State.Pouring);

                return true;
            }
            return base.OnTake(item);
        }
    }

}