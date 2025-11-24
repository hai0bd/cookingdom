using Link;
using System.Collections.Generic;
using UnityEngine;

namespace HuyThanh.Cooking.SarmaleRomania
{

    public class PlateDoneCabbage : ItemIdleBase
    {
        [SerializeField] List<Transform> targetTF;
        public override bool OnTake(IItemMoving item)
        {
            if (item is DoneCabbage cabbage)
            {
                cabbage.CabbageSetType(targetTF.Count <= 2 ? 1 : 2);
                cabbage.OnMove(targetTF[0].position, targetTF[0].rotation, 0.2f);
                cabbage.PlayAnim();
                cabbage.OnDone();
                targetTF.RemoveAt(0);

                if (targetTF.Count == 0)
                {
                    LevelControl.Ins.CheckStep(1f);
                }
                return true;
            }
            return base.OnTake(item);
        }
    }

}