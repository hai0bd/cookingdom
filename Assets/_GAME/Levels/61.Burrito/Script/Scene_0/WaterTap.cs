using Link;
using UnityEngine;

namespace HuyThanh.Cooking.Burrito
{
    public class WaterTap : ItemIdleBase
    {
        [SerializeField] WaterSink waterSink;

        private bool isOn = false;

        public override void OnClickDown()
        {
            base.OnClickDown();
            SoundControl.Ins.PlayFX(Fx.Button);

            isOn = !isOn;
            waterSink.OnOpenWater(isOn);

            LevelControl.Ins.CheckStep(1f);
        }

        public bool IsOff()
        {
            return isOn == false;
        }
    }

}
