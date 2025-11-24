using Link;
using System.Collections.Generic;
using UnityEngine;


namespace HuyThanh.Cooking.SarmaleRomania
{
    public class PlateHoldCabbage : ItemIdleBase
    {
        [SerializeField] CuttingBoard cuttingBoard;
        [SerializeField] List<Transform> targetPositions = new List<Transform>();
        [SerializeField] CabbageLeafCutting cabbageLeafCutting;

        public override bool OnTake(IItemMoving item)
        {
            if (item is CabbageLeafMoveToBowl cabbageLeaf)
            {
                item.OnMove(targetPositions[0].position, Quaternion.identity, 0.2f);
                item.ChangeState(CabbageLeafMoveToBowl.State.Done);
                cabbageLeaf.ShowOnDish();
                targetPositions.RemoveAt(0);

                if (targetPositions.Count == 0)
                {
                    cabbageLeafCutting.enabled = true;
                    cabbageLeafCutting.Show();
                    cuttingBoard.OnTake(cabbageLeafCutting);
                    cuttingBoard.MiddleLeavesDisapper();
                }
                return true;
            }
            return base.OnTake(item);
        }
    }

}
