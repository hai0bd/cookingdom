using Link;
using UnityEngine;

namespace HuyThanh.Cooking.SarmaleRomania
{
    public class CornPotButton : ItemIdleBase
    {
        [SerializeField] PotCorn potCorn;
        [SerializeField] GameObject buttonOn, buttonOff;

        private bool isOn = false;



        public override void OnClickDown()
        {
            base.OnClickDown();

            if (potCorn.CanUseButton())
            {
                isOn = !isOn;

                buttonOn.SetActive(isOn);
                buttonOff.SetActive(!isOn);
                potCorn.OnButtonClick();
            }
        }
    }
}