using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link.Turkey
{
    public class DecorPoint : ItemIdleBase
    {
        public override bool IsDone => itemNames.Count == decorPoints.Length;
        List<Decor.DecorType> itemNames = new List<Decor.DecorType>();
        [SerializeField] Transform[] decorPoints;
        [SerializeField] Transform chickenPoint;

        public override bool OnTake(IItemMoving item)
        {
            if (item is Chicken)
            {
                item.TF.SetParent(transform);
                item.ChangeState(Chicken.State.Done);
                item.OnMove(chickenPoint.position, Quaternion.identity, 0.3f);
                LevelControl.Ins.CheckStep();
                return true;
            }
            if (item is Decor && !itemNames.Contains((item as Decor).DecorName))
            {
                Decor.DecorType type = (item as Decor).DecorName;
                item.TF.SetParent(transform);
                itemNames.Add(type);
                item.OnMove(decorPoints[(int)type].position, decorPoints[(int)type].rotation, 0.3f);
                item.ChangeState(Decor.State.Tidy);
                LevelControl.Ins.CheckStep();
                return true;
            }
            return false;
        }


    }
}