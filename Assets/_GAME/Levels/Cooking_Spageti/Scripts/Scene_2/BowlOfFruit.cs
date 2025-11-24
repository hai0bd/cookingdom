using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link.Cooking.Spageti
{
    public class BowlOfFruit : ItemIdleBase
    {
        [SerializeField] ItemMovingBase fruit;

        public override bool OnTake(IItemMoving item)
        {
            if (item.Equals(fruit))
            {
                if(item is Tomato && item.IsState(Tomato.State.Pieced)) item.OnDone();
                if(item is Onion && item.IsState(Onion.State.Pieced)) item.OnDone();
                if(item is Leaf && item.IsState(Leaf.State.Cut)) item.OnDone();
                if(item is Oliu && item.IsState(Oliu.State.Pieced)) item.OnDone();
                if(item is Bacon && item.IsState(Bacon.State.Pieced)) item.OnDone();
                item.OnMove(TF.position + Vector3.up * 0.2f, Quaternion.identity, 0.2f);
                return true;
            }
            return false;
        }
    }
}