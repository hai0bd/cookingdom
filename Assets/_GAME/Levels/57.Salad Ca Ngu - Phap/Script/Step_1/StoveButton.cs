using DG.Tweening;
using Link;
using UnityEngine;

namespace HuyThanh.Cooking.TunaSaladFrench
{
    public class StoveButton : ItemIdleBase
    {
        [SerializeField] GameObject onButton, offButton;
        [SerializeField] Stove stove;

        bool isOn = false;
        public override void OnClickDown()
        {
            base.OnClickDown();

            if (stove.CanUseButton() == false) return;

            SoundControl.Ins.PlayFX(Fx.Button);

            isOn = !isOn;
            onButton.SetActive(isOn);
            offButton.SetActive(!isOn);
            stove.OnStoveButtonClick(isOn);

            ///tranh spam nut bat tat
            Collider2D collider = GetComponent<Collider2D>();
            collider.enabled = false;
            DOVirtual.DelayedCall(1f, () => { collider.enabled = true; });
        }
    }
}