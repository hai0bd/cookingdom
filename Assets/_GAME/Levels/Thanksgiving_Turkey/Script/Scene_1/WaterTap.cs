using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link.Turkey
{
    public class WaterTap : ItemIdleBase
    {
        [SerializeField] WaterSink waterSink;
        bool isOn = false;

        public override void OnClickDown()
        {
            isOn = !isOn;
            waterSink.OnOpenWater(isOn);
        }
    }

}