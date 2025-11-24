using Link;
using UnityEngine;

namespace HuyThanh.Cooking.SarmaleRomania
{
    public class PanBaseButton : ItemIdleBase
    {
        [SerializeField] PanBase panBase;
        [SerializeField] ItemAlpha heatAlpha;
        [SerializeField] GameObject buttonOn, buttonOff;
        private bool isOn = false;

        public override void OnClickDown()
        {
            base.OnClickDown();

            if (panBase.CanUseButton())
            {
                isOn = !isOn;
                buttonOff.SetActive(!isOn);
                buttonOn.SetActive(isOn);
                heatAlpha.DoAlpha(isOn ? 1 : 0, 1f);
                panBase.OnButtonClick(isOn);
            }
        }
    }
}