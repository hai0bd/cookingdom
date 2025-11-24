using Link;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoangLinh.Cooking.Test
{
    public class WaterTap : ItemIdleBase
    {
        [SerializeField] WaterInSink waterInSink;

        private bool isOn = false;

        public override void OnClickDown()
        {
            base.OnClickDown();

            isOn = !isOn;
            waterInSink.OnOpenWater(isOn);
        } 

        public bool IsOff() { return isOn == false; }
    }
}