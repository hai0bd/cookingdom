using Link;
using UnityEngine;

namespace HuyThanh.Cooking.TunaSaladFrench
{
    public class WaterTap : ItemIdleBase
    {
        [SerializeField] GameObject onButton, offButton;
        [SerializeField] WaterSink waterSink;

        bool isOn = false;

        public override void OnClickDown()
        {
            base.OnClickDown();
            SoundControl.Ins.PlayFX(Fx.Button);

            isOn = !isOn;
            onButton.SetActive(isOn);
            offButton.SetActive(!isOn);

            waterSink.OnOpenWater(isOn);
        }
    }
}


