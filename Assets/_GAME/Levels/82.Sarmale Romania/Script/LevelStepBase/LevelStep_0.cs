using Link;
using UnityEngine;
namespace HuyThanh.Cooking.SarmaleRomania
{
    public class LevelStep_0 : LevelStepBase
    {
        [SerializeField] ItemIdleBase[] plate;
        [SerializeField] WaterSink waterSink;

        public int DoneDill = 0;

        public override bool IsDone()
        {
            foreach (ItemIdleBase item in plate)
            {
                if (!item.IsDone)
                {
                    return false;
                }
            }
            if (!waterSink.waterCoverClose) { return false; }

            return true;
        }
    }
}

