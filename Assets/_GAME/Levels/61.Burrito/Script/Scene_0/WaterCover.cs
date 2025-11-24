using Link;
using UnityEngine;

namespace HuyThanh.Cooking.Burrito
{
    public class WaterCover : ItemMovingBase
    {
        public override bool IsCanMove => waterSink.haveItem == false;

        [SerializeField] WaterSink waterSink;

        public override void OnClickDown()
        {
            base.OnClickDown();
            OrderLayer = 55; //order nam tren hoa qua khi keo di
            SoundControl.Ins.PlayFX(Fx.Click);
        }

        public override void OnClickTake()
        {
            base.OnClickTake();
            waterSink.OnTake(this);
            SoundControl.Ins.PlayFX(Fx.PutDown);
        }

        public override void OnDrop()
        {
            OnClickTake();
        }
    }

}