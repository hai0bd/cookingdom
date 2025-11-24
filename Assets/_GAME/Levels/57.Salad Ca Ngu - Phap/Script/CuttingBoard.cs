using Link;
using Sirenix.OdinInspector;
using UnityEngine;


namespace HuyThanh.Cooking.TunaSaladFrench
{
    public class CuttingBoard : ItemIdleBase
    {
        [SerializeField, ReadOnly] IItemMoving item;
        [SerializeField] Knife knife;
        [SerializeField] Transform knifePoint;

        private bool IsEmpty => item == null || Vector2.Distance(item.TF.position, TF.position) > 0.2f;

        public override bool OnTake(IItemMoving item)
        {
            #region Garlic

            if (IsEmpty && item is Garlic && item.IsState(Garlic.State.Origin))
            {
                this.item = item;
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                item.OnSave(0.2f);

                return true;
            }

            if (!IsEmpty && item is Knife && this.item is Garlic && this.item.IsState(Garlic.State.Origin))
            {
                this.item.IfChangeState(Garlic.State.Origin, Garlic.State.Cutting1);
                item.ChangeState(Knife.State.CuttingGarlic);
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                return true;
            }

            #endregion

            /// can sua code cua ca cai nay
            if (IsEmpty && item is ItemSpice itemSpice && item.IsState(ItemSpice.State.Origin))
            {
                this.item = item;
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                item.OnSave(0.2f);
                //SoundControl.Ins.PlayFX(LevelStep_1.Fx.Take);+

                //itemSpice.ChangeState(ItemSpice.State.Cutting);

                return true;
            }

            #region Herb

            if (IsEmpty && item is Herb && item.IsState(Herb.State.Origin))
            {
                this.item = item;
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                item.OnSave(0.2f);

                return true;
            }

            if (!IsEmpty && item is Knife && this.item is Herb && this.item.IsState(Herb.State.Origin))
            {
                this.item.IfChangeState(Herb.State.Origin, Herb.State.Cutting1);
                item.ChangeState(Knife.State.CuttingGarlic);
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                return true;
            }

            #endregion

            #region  CutItem

            if (IsEmpty && item is CutItem && item.IsState(CutItem.State.Normal))
            {
                this.item = item;
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                item.OnSave(0.2f);
                return true;
            }

            if (!IsEmpty && item is Knife && this.item is CutItem)
            {
                return this.item.OnTake(item);
            }

            #endregion

            #region  Potato

            if (IsEmpty && item is Potato && item.IsState(Potato.State.Normal))
            {
                this.item = item;
                item.ChangeState(Potato.State.Grater);
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                item.OnSave(0.2f);
                return true;
            }

            if (!IsEmpty && item is Knife && this.item is Potato && this.item.IsState(Potato.State.DoneGrater))
            {
                this.item.ChangeState(Potato.State.Cutting);
                item.ChangeState(Knife.State.CuttingPotato);
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                return true;
            }
            #endregion

            #region Eggs

            if (IsEmpty && item is Egg egg1 && item.IsState(Egg.State.Cooked))
            {
                item.TF.SetParent(TF);
                this.item = item;
                egg1.PreBreaking();
                item.ChangeState(Egg.State.Breaking);

                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                return true;
            }
            #endregion

            #region Bean

            if (IsEmpty && item is Bean && item.IsState(Bean.State.Normal))
            {
                this.item = item;
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                item.OnSave(0.2f);
                return true;
            }

            if (item is Knife && this.item is Bean && this.item.IsState(Bean.State.Normal))
            {
                this.item.ChangeState(Bean.State.Cutting);
                item.ChangeState(Knife.State.Cutting);
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                return true;
            }

            #endregion

            //item.OnBack();
            return base.OnTake(item);
        }


        public override void OnClickDown()
        {
            base.OnClickDown();
            if (item is Egg egg1 && !IsEmpty && item.IsState(Egg.State.Breaking))
            {
                egg1.OnBreaking();
                return;
            }

            if (item is Potato && !IsEmpty && item.IsState(Potato.State.Cutting))
            {
                item.OnClickDown();
                return;
            }

            if (item is Garlic && !IsEmpty && (item.IsState(Garlic.State.Cutting1) || item.IsState(Garlic.State.Cutting2)))
            {
                item.OnClickDown();
                return;
            }

            if (item is Herb && !IsEmpty && (item.IsState(Herb.State.Cutting1) || item.IsState(Herb.State.Cutting2)))
            {
                item.OnClickDown();
                return;
            }
        }
    }

}
