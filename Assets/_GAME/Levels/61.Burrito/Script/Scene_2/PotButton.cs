using Link;
using UnityEngine;

namespace HuyThanh.Cooking.Burrito
{


    public class PotButton : ItemIdleBase
    {
        [SerializeField] Pot pot;
        [SerializeField] ItemAlpha heatAlpha;
        [SerializeField] GameObject buttonOn, buttonOff;
        private bool isOn = false;

        public override void OnClickDown()
        {
            base.OnClickDown();

            if (CanUseButton())
            {
                ButtonClick();
            }
        }

        private void ButtonClick()
        {
            SoundControl.Ins.PlayFX(Fx.Button);
            isOn = !isOn;
            if (isOn)
            {
                buttonOn.SetActive(true);
                buttonOff.SetActive(false);
                heatAlpha.DoAlpha(1f, 1f);
            }
            else
            {
                buttonOn.SetActive(false);
                buttonOff.SetActive(true);
                heatAlpha.DoAlpha(0, 1f);
            }
            pot.OnClickButton(isOn);
        }

        private bool CanUseButton()
        {
            if (pot.IsState(Pot.State.HaveItem) && isOn == false)
            {
                return true;
            }

            if (pot.IsState(Pot.State.DoneCook) && isOn == true && pot.HaveBowlPotItem() == false)
            {
                return true;
            }

            return false;
        }

    }
}