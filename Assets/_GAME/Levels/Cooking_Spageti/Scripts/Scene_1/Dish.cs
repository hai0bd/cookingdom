using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link.Cooking.Spageti
{
    public class Dish : ItemIdleBase
    {
        public override bool OnTake(IItemMoving item)
        {
            if(item is Scoop && item.IsState(Scoop.State.Noodle))
            {
                item.OnDone();
                Noodle noodle = (item as Scoop).noodle;
                noodle.TF.SetParent(TF);
                noodle.OnMove(TF.position, Quaternion.identity, 0.2f);
                noodle.ChangeState(Noodle.State.InDish);
                return true;
            }
            return false;
        }
    }
}
