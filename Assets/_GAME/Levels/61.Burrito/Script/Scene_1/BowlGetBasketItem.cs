using Link;
using UnityEngine;

namespace HuyThanh.Cooking.Burrito
{

    public class BowlGetBasketItem : ItemIdleBase
    {
        public enum GetItemType
        {
            Small,
            Big
        }
        [SerializeField] ItemMovingBase item;
        [SerializeField] GetItemType type;

        public override bool IsDone => item != null;

        public override bool OnTake(IItemMoving item)
        {

            if (type == GetItemType.Small && this.item == null)
            {
                if (item.TF.name == "Onion" && item.IsState(BasketCutItem.State.Spice))
                {
                    this.item = item as ItemMovingBase;
                    item.ChangeState(BasketCutItem.State.Done);
                    item.OnMove(TF.position, Quaternion.identity, 0.2f);
                    LevelControl.Ins.CheckStep(1f);
                    return true;
                }

                if (item.TF.name == "Garlic" && item.IsState(GarlicCutItem.State.Spice))
                {
                    this.item = item as ItemMovingBase;
                    item.ChangeState(GarlicCutItem.State.Done);
                    item.OnMove(TF.position, Quaternion.identity, 0.2f);
                    LevelControl.Ins.CheckStep(1f);
                    return true;
                }
            }

            if (type == GetItemType.Big && this.item == null)
            {
                if ((item.TF.name == "Tomato" || item.TF.name == "Vegetable") && item.IsState(BasketCutItem.State.Spice))
                {
                    this.item = item as ItemMovingBase;
                    item.ChangeState(BasketCutItem.State.Done);
                    item.OnMove(TF.position, Quaternion.identity, 0.2f);
                    LevelControl.Ins.CheckStep(1f);
                    return true;
                }

                if (item is CornCutItem && item.IsState(CornCutItem.State.Spice))
                {
                    this.item = item as ItemMovingBase;
                    item.ChangeState(CornCutItem.State.Done);
                    item.OnMove(TF.position, Quaternion.identity, 0.2f);
                    LevelControl.Ins.CheckStep(1f);
                    return true;
                }
            }
            return base.OnTake(item);
        }
    }

}