using Link;
using UnityEngine;

namespace HuyThanh.Cooking.TunaSaladFrench
{
    public class WaterCover : ItemMovingBase
    {
        [SerializeField] WaterSink waterSink;
        public override bool IsCanMove => true;

        public override void OnClickDown()
        {
            OrderLayer = 55; //order nam tren hoa qua
            SoundControl.Ins.PlayFX(Fx.Click);
        }

        public override void OnClickTake()
        {
            base.OnClickTake();
            waterSink.OnTake(this);
            SoundControl.Ins.PlayFX(Fx.Take);
        }

        public override void OnDrop()
        {
            OnClickTake();
        }
    }

}
