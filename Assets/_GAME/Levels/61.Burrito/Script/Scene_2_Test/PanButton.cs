using HuyThanh.Cooking.Burrito;
using Link;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Hai.Cooking.Burrito
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
            if (CanUseButton()) ButtonClick();
        }

        public void ButtonClick()
        {
            SoundControl.Ins.PlayFX(Fx.Button);
            if (isOn)
            {
                buttonOff.SetActive(false);
                buttonOn.SetActive(true);
                heatAlpha.DoAlpha(1f, 1f);
            }
            else
            {
                buttonOff.SetActive(true);
                buttonOn.SetActive(false);
                heatAlpha.DoAlpha(0f, 1f);
            }
            pan.OnClickButton(isOn);
        }

        public bool CanUseButton()
        {
            if (pan.IsState(Pan.State.Normal) && !isOn)
            {
                return true;
            }
            if (pan.IsState(Pan.State.WaitForTurnOff) && isOn)
            {
                return true;
            }

            return false;
        }
    }
}