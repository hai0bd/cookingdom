using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Link;

namespace HoangLinh.Cooking.Test
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
                ClickButton();
            }
        }

        private void ClickButton()
        {
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
            if (pan.IsState(Pan.State.Empty) && isOn == false)
            {
                return true;
            }
            if (pan.IsState(Pan.State.WaitTurnOff) && isOn == true)
            {
                return true;
            }
            return false;
        }
    }
}