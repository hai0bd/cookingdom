using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link.Turkey
{
    public class OvenDoor : ItemIdleBase
    {
        [SerializeField] Oven oven;

        public override void OnClickDown()
        {
            base.OnClickDown();
            oven.OpenDoor();
        }
    }
}