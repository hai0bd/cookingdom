using Link;
using UnityEngine;

namespace HuyThanh.Cooking.TunaSaladFrench
{
    public class Bowl : ItemIdleBase
    {
        [SerializeField] ItemMovingBase item;

        [SerializeField] private bool canSaveOnStart = true;
        public override bool IsDone => item.IsState(ItemSpice.State.Cut);

        private void Start()
        {
            if (item != null && canSaveOnStart)
            {
                OnTake(item);
            }
        }

        public override bool OnTake(IItemMoving item)
        {
            if (item is ItemSpice && this.item.Equals(item))
            {
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                item.OnSave(0.2f);
                item.IfChangeState(ItemSpice.State.Cut, ItemSpice.State.Spice);
                LevelControl.Ins.CheckStep(0.2f);
                //SoundControl.Ins.PlayFX(LevelStep_1.Fx.Take);
                return true;
            }

            if (item is Garlic && this.item.Equals(item))
            {
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                item.OnSave(0.2f);
                item.IfChangeState(Garlic.State.Cut2, Garlic.State.Spice);


                return true;
            }

            if (item is CutItem && this.item.Equals(item))
            {
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                item.OnSave(0.2f);
                item.IfChangeState(CutItem.State.Spice, CutItem.State.Done);
                LevelControl.Ins.CheckStep(0.2f);
                return true;
            }

            if (item is Bean && this.item.Equals(item))
            {
                item.TF.SetParent(this.TF);
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                item.OnSave(0.2f);
                item.IfChangeState(Bean.State.Spice, Bean.State.Done);
                LevelControl.Ins.CheckStep(0.2f);
                return true;
            }

            if (item is XaLach && this.item.Equals(item))
            {
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                item.OnSave(0.2f);
                item.IfChangeState(XaLach.State.Spice, XaLach.State.Done);
                LevelControl.Ins.CheckStep(0.2f);
                return true;
            }

            if (item is Potato && this.item.Equals(item))
            {
                item.TF.SetParent(this.TF);
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                item.OnSave(0.2f);
                item.IfChangeState(Potato.State.Spice, Potato.State.Done);
                LevelControl.Ins.CheckStep(0.2f);
                return true;
            }

            if (item is Pearl && this.item.Equals(item))
            {
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                item.OnSave(0.2f);
                item.IfChangeState(Pearl.State.Sliced, Pearl.State.Done);

                if (item.IsState(Pearl.State.Done))
                    item.TF.SetParent(this.transform);
                return true;
            }

            if (item is Nut && this.item.Equals(item))
            {
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                item.OnSave(0.2f);
                return true;
            }

            if (item is Egg && this.item.Equals(item))
            {
                if (item.IsState(Egg.State.Cooked))
                {
                    return true;
                }
                //item.OnMove(TF.position, Quaternion.identity, 0.2f);
                //item.OnSave(0.2f);

                return true;
            }

            if (item is Herb && this.item.Equals(item))
            {
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                item.OnSave(0.2f);
                return true;
            }


            return false;
        }
    }
}

