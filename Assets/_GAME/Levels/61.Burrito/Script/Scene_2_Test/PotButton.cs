using HuyThanh.Cooking.Burrito;
using Link;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Hai.Cooking.Burrito
{
    public class PotButton : ItemIdleBase
    {
        [SerializeField] private Pot pot;
        [SerializeField] private ItemAlpha itemAlpha;
        [SerializeField] private GameObject buttonOn, buttonOff;

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
            if (isOn)
            {
                buttonOff.SetActive(false);
                buttonOn.SetActive(true);
                itemAlpha.DoAlpha(1f, 1f);
            }
            else
            {
                buttonOff.SetActive(true);
                buttonOn.SetActive(false);
                itemAlpha.DoAlpha(0, 1f);
            }
            pot.OnClickButton(isOn);
        }

        private bool CanUseButton()
        {
            if (pot.IsState(Pot.State.HaveItem) && !isOn)
            {
                return true;
            }
            if (pot.IsState(Pot.State.DoneCook) && isOn  && !pot.HaveBowlPotItem())
            {
                return true;
            }
            return false;
        }
    }
}