using Link;
using UnityEngine;


namespace HuyThanh.Cooking.Burrito
{

    public class PanButton : ItemIdleBase
    {
        [SerializeField] Pan pan;
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
            pan.OnClickButton(isOn);
        }

        private bool CanUseButton()
        {
            if (pan.IsState(Pan.State.Normal) && isOn == false)
            {
                return true;
            }

            if (pan.IsState(Pan.State.WaitForTurnOff) && isOn == true)
            {
                return true;
            }

            return false;
        }
    }

}