using Link;
using UnityEngine;

namespace HuyThanh.Cooking.SarmaleRomania
{
    public class PotCabbageRollTarget : ItemIdleBase
    {
        [SerializeField] PotCabbage potCabbage;
        [SerializeField] CabbageRollType type;
        [SerializeField] int targetLayer;

        private bool isHaveItem = false;

        public CabbageRollType Type => type;
        public bool IsHaveItem => isHaveItem;
        public int TargetLayer => targetLayer;
        public void SetHaveItem(bool isHave)
        {
            isHaveItem = isHave;
            potCabbage.CheckDoneCabbageRoll();
        }
    }
}
