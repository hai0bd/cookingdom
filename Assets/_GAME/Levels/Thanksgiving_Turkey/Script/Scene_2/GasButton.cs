using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link.Turkey
{
    public class GasButton : ItemIdleBase
    {
        [SerializeField] Pan gasStove;

        public override void OnClickDown()
        {
            base.OnClickDown();
            gasStove.ActiveGas();
        }

    }
}