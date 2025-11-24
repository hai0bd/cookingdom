using Link;
using UnityEngine;

namespace HoangLinh.Cooking.Test
{   
    public class SinkCover : ItemMovingBase
    {
        [SerializeField] WaterInSink waterInSink;

        public override bool IsCanMove => waterInSink.haveItem == false;

        public override void OnClickDown()
        {
            base.OnClickDown();
        }

        public override void OnClickTake()
        {
            base.OnClickTake();
            waterInSink.OnTake(this);
        }

        public override void OnDrop()
        {
            OnClickTake();
        }
    }
}