using Link;
using UnityEngine;

namespace HuyThanh.Cooking.SarmaleRomania
{
    public class PlateVegetable : ItemIdleBase
    {
        public enum PlateType
        {
            None,
            Onion,
            Dill,
            DillSpice,
        }

        [SerializeField] PlateType plateType;
        [SerializeField] LevelStep_0 levelStep_0;
        public override bool IsDone => isDone;
        bool isDone = false;
        private int dillcount = 3;

        public override bool OnTake(IItemMoving item)
        {
            if (plateType == PlateType.None && item is Dill && item.IsState(Dill.State.MovingPlate) && levelStep_0.DoneDill != 0)
            {
                LevelControl.Ins.LoseHalfHeart(TF.position);
                return false;
            }

            if (plateType == PlateType.None && item is Dill && item.IsState(Dill.State.MovingPlate) && levelStep_0.DoneDill == 0)
            {
                levelStep_0.DoneDill++;
                isDone = true;
                plateType = PlateType.Dill;
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                item.ChangeState(Dill.State.Done);
                return true;
            }

            if (plateType == PlateType.Dill && item is Dill dill && item.IsState(Dill.State.MovingPlate) && levelStep_0.DoneDill != 0)
            {
                levelStep_0.DoneDill++;
                isDone = true;
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                item.ChangeState(Dill.State.Done);
                return true;
            }

            if (plateType == PlateType.None && item is Onion onion && item.IsState(Onion.State.MovingPlate))
            {
                isDone = true;
                plateType = PlateType.Onion;
                item.OnMove(TF.position + Vector3.left * 0.05f + Vector3.up * 0.1f, Quaternion.identity, 0.2f);
                item.ChangeState(Onion.State.Done);
                return true;
            }

            if (plateType == PlateType.None && item is DillPice dillPice && item.IsState(DillPice.State.MovingPlate))
            {
                isDone = true;
                plateType = PlateType.DillSpice;
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                item.ChangeState(DillPice.State.Done);
                return true;
            }

            return base.OnTake(item);
        }
    }
}