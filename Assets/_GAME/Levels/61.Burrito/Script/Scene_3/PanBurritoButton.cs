using Link;
using UnityEngine;

namespace HuyThanh.Cooking.Burrito
{
    public class PanBurritoButton : ItemIdleBase
    {
        [SerializeField] PanBurrito panBurrito;
        [SerializeField] GameObject buttonOn, buttonOff;

        private bool isOn = false;
        public bool IsOn => isOn;
        public override void OnClickDown()
        {
            base.OnClickDown();

            if (panBurrito.CanUseButton())
            {
                SoundControl.Ins.PlayFX(Fx.Button);
                isOn = !isOn;
                buttonOff.SetActive(!isOn);
                buttonOn.SetActive(isOn);
                panBurrito.OnButtonClick(isOn);
            }
        }
    }
}
