using Link;
using System.Collections.Generic;
using UnityEngine;

namespace HuyThanh.Cooking.TunaSaladFrench
{
    public class BanhMiChao : ItemIdleBase
    {
        [SerializeField] ItemMovingBase itemOrigin;
        [SerializeField] List<GameObject> banhMiPositions;

        public override bool OnTake(IItemMoving item)
        {
            if (item is BanhMi && !item.Equals(itemOrigin) && banhMiPositions.Count != 0 && itemOrigin is BanhMi)
            {
                item.ChangeState(BanhMi.State.Done);
                item.OnMove(banhMiPositions[0].transform.position, Quaternion.identity, 0.2f);
                item.TF.SetParent(banhMiPositions[0].transform);
                banhMiPositions.RemoveAt(0);

                MiniGame1.Instance.CheckDone(1f);

                return true;
            }

            if (item is Butter && !item.Equals(itemOrigin) && banhMiPositions.Count != 0 && itemOrigin is Butter)
            {
                item.ChangeState(Butter.State.Done);
                item.OnMove(banhMiPositions[0].transform.position, Quaternion.identity, 0.2f);
                item.TF.SetParent(banhMiPositions[0].transform);
                banhMiPositions.RemoveAt(0);

                MiniGame1.Instance.CheckDone(1f);

                return true;
            }

            if (item is Pearl && !item.Equals(itemOrigin) && banhMiPositions.Count != 0 && itemOrigin is Pearl)
            {
                item.ChangeState(Pearl.State.Done);
                item.OnMove(banhMiPositions[0].transform.position, item.TF.rotation, 0.2f);
                item.TF.SetParent(banhMiPositions[0].transform);
                banhMiPositions.RemoveAt(0);

                MiniGame1.Instance.CheckDone(1f);

                return true;
            }
            return base.OnTake(item);
        }
    }
}