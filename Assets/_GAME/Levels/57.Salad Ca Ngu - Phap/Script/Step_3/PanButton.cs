using Link;
using Satisgame;
using UnityEngine;

namespace HuyThanh.Cooking.TunaSaladFrench
{
    public class PanButton : ItemIdleBase
    {
        [SerializeField] GameObject onButton, offButton;
        [SerializeField] Pan pan;

        [SerializeField] private EmojiControl _emojiControl;

        bool isOn = false;
        public override void OnClickDown()
        {
            base.OnClickDown();

            if (pan.CanUseButton() == false)
            {

                return;
            }
            SoundControl.Ins.PlayFX(Fx.Button);
            isOn = !isOn;
            onButton.SetActive(isOn);
            offButton.SetActive(!isOn);

            pan.OnStoveButtonClick(isOn);
        }
    }
}