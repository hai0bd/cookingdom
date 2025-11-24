using Link;
using UnityEngine;

namespace HuyThanh.Cooking.SarmaleRomania
{
    public class PotRibTarget : ItemIdleBase
    {
        [SerializeField] PotCabbage potCabbage;
        [SerializeField] int targetLayer;
        private bool isHaveItem = false;

        public bool IsHaveItem => isHaveItem;
        public void SetHaveItem(bool isHave)
        {
            isHaveItem = isHave;
            potCabbage.CheckDoneRib();
        }
    }
}