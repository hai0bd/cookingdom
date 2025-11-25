using Link;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hai.Cooking.NewTest
{
    public class PanButton : ItemIdleBase
    {
        [SerializeField] private Pan pan;
        [SerializeField] private ItemAlpha heatAlpha;
        [SerializeField] private GameObject buttonOn, buttonOff;
        private bool isOn = false;

        public override void OnClickDown()
        {
            base.OnClickDown();
            //Debug.Log("OnClickDown");
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
                //heatAlpha.DoAlpha(1f, 1f);
            }
            else
            {
                buttonOn.SetActive(false);
                buttonOff.SetActive(true);
                //heatAlpha.DoAlpha(0, 1f);
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
            //Debug.Log("!CanUseButton");
            return false;
        }


    }
}