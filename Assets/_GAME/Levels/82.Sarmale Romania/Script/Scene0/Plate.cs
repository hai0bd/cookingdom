using Link;
using UnityEngine;
namespace HuyThanh.Cooking.SarmaleRomania
{
    public class Plate : ItemIdleBase
    {
        public enum PlateType { Cabbage, Beef, Smokermeat, Prokmeat }
        [SerializeField] PlateType plateType;

        private bool isDone = false;
        public override bool IsDone => isDone;


        public override bool OnTake(IItemMoving item)
        {
            if (plateType == PlateType.Cabbage && item is Cabbage && item.IsState(Cabbage.State.MovingPlate))
            {
                isDone = true;
                item.OnMove(TF.position + Vector3.left * 0.02f, Quaternion.identity, 0.2f);
                item.ChangeState(Cabbage.State.Done);
                LevelControl.Ins.CheckStep(1f);
                return true;
            }

            if (plateType == PlateType.Beef && item is Beef beef && beef.IsState(Beef.State.MovingPlate))
            {
                isDone = true;
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                item.ChangeState(Beef.State.Done);

                LevelControl.Ins.CheckStep(1f);
                return true;
            }

            if (plateType == PlateType.Smokermeat && item is SmokerRibs smokerRibs && smokerRibs.IsState(SmokerRibs.State.MovingPlate))
            {
                isDone = true;
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                item.ChangeState(SmokerRibs.State.Done);

                LevelControl.Ins.CheckStep(1f);
                return true;
            }

            if (plateType == PlateType.Prokmeat && item is FrokMeat frokMeat && frokMeat.IsState(FrokMeat.State.MovingPlate))
            {
                isDone = true;
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                item.ChangeState(FrokMeat.State.Done);

                LevelControl.Ins.CheckStep(1f);
                return true;
            }

            //if (type == PlateType.Onion && item is Onion onion && onion.IsState(Onion.State.movingplate))
            //{
            //    item.OnMove(TF.position, Quaternion.identity, 0.2f);
            //    item.ChangeState(Onion.State.done);
            //    isDone = true;
            //    LevelControl.Ins.CheckStep(1f);
            //    return true;
            //}
            //if (type == PlateType.Dill && item is Dill dill && dill.IsState(Dill.State.MovingPlate))
            //{
            //    item.OnMove(TF.position, Quaternion.identity, 0.2f);
            //    item.ChangeState(Dill.State.Done);

            //    dillcount--;
            //    if (dillcount <= 0)
            //    {
            //        isDone = true;
            //        LevelControl.Ins.CheckStep(1f);
            //    }

            //    return true;
            //}
            //if (type == PlateType.Dillpice && item is DillPice dillpie && dillpie.IsState(DillPice.State.movingplate))
            //{
            //    item.OnMove(TF.position, Quaternion.identity, 0.2f);
            //    item.ChangeState(DillPice.State.done);
            //    isDone = true;
            //    LevelControl.Ins.CheckStep(1f);
            //    return true;
            //}

            return false;
        }
    }
}

