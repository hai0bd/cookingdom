using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link.Turkey
{
    public class WaterCover : ItemMovingBase
    {
        [SerializeField] WaterSink waterSink;
        public override bool IsCanMove => true;

        public override void OnClickDown()
        {
            OrderLayer = 55; //order nam tren hoa qua
            SoundControl.Ins.PlayFX(LevelStep_1.FX.Hold);
        }

        public override bool OnTake(IItemMoving item)
        {
            return false;
        }

        public override void OnClickTake()
        {
            base.OnClickTake();
            waterSink.OnTake(this);
            SoundControl.Ins.PlayFX(LevelStep_1.FX.Put);
        }

        public override void OnDrop()
        {
            OnClickTake();
        }
    }
}