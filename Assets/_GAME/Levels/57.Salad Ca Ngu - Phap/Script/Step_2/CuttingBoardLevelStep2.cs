using Link;
using UnityEngine;

namespace HuyThanh.Cooking.TunaSaladFrench
{
    public class CuttingBoardLevelStep2 : ItemIdleBase
    {
        private IItemMoving _item;

        private bool IsEmpty => _item == null || Vector2.Distance(_item.TF.position, TF.position) > 0.2f;
        public override bool OnTake(IItemMoving item)
        {
            #region BanhMi

            if (IsEmpty && item is BanhMi && item.IsState(BanhMi.State.Normal) && LevelControl.Ins.IsHaveObject<Butter>(TF.position) == false && LevelControl.Ins.IsHaveObject<Pearl>(TF.position) == false)
            {
                this._item = item;
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                return true;
            }

            if (!IsEmpty && this._item is BanhMi && this._item.IsState(BanhMi.State.Normal) && item is Knife)
            {
                item.OnMove(TF.position + Vector3.right * 0.2f, Quaternion.identity, 0.2f);
                item.ChangeState(Knife.State.CuttingBread);
                this._item.ChangeState(BanhMi.State.Cutting);
                return true;
            }

            #endregion

            #region Pearl

            if (IsEmpty && item is Pearl && item.IsState(Pearl.State.Normal) && CheckHaveObject() == false)
            {
                this._item = item;
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                return true;
            }

            if (!IsEmpty && item is Knife && this._item is Pearl && this._item.IsState(Pearl.State.Normal))
            {
                item.OnMove(TF.position + Vector3.right * 0.2f, Quaternion.identity, 0.2f);
                item.ChangeState(Knife.State.CuttingPearl);
                this._item.ChangeState(Pearl.State.Cutting);
                return true;
            }

            #endregion

            #region Butter

            if (IsEmpty && item is Butter && item.IsState(Butter.State.Normal) && LevelControl.Ins.IsHaveObject<BanhMi>(TF.position) == false && LevelControl.Ins.IsHaveObject<Pearl>(TF.position) == false)
            {
                this._item = item;
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                return true;
            }

            if (!IsEmpty && item is Knife && this._item is Butter && this._item.IsState(Butter.State.Normal))
            {
                item.OnMove(TF.position + Vector3.right * 0.2f, Quaternion.identity, 0.2f);
                item.ChangeState(Knife.State.CuttingButter);
                this._item.ChangeState(Pearl.State.Cutting);
                return true;
            }
            #endregion

            return base.OnTake(item);
        }

        private bool CheckHaveObject()
        {
            return LevelControl.Ins.IsHaveObject<BanhMi>(TF.position) || LevelControl.Ins.IsHaveObject<Butter>(TF.position);
        }
    }
}