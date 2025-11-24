using Link;
using UnityEngine;

namespace HuyThanh.Cooking.SarmaleRomania
{

    public class PotButton : ItemIdleBase
    {
        [SerializeField] PotCabbage potCabbage;
        [SerializeField] GameObject buttonOn, buttonOff;

        private bool isOn = false;

        public override void OnClickDown()
        {
            base.OnClickDown();

            if (potCabbage.IsState(PotCabbage.State.WaitForTurnOnButton, PotCabbage.State.WaitForTurnOff))
            {
                isOn = !isOn;
                potCabbage.OnClickButton();
                buttonOn.SetActive(isOn);
                buttonOff.SetActive(!isOn);
            }
        }
    }
}