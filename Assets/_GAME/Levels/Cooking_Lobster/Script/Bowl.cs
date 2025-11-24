using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link.Cooking.Lobster
{
    public class Bowl : ItemIdleBase
    {
        [SerializeField] ItemMovingBase item;

        public override bool IsDone =>  item.IsState(ItemSteam.State.Cut); 

        private void Start()
        {
            if (item != null)
            {
                OnTake(item);
            }
        }

        public override bool OnTake(IItemMoving item)
        {
            if (item is ItemSteam && this.item.Equals(item))
            {
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                item.OnSave(0.2f);
                LevelControl.Ins.CheckStep(0.2f);
                SoundControl.Ins.PlayFX(LevelStep_1.Fx.Take);
                return true;
            }  
            
            if (item is ItemSpice && this.item.Equals(item))
            {
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                item.OnSave(0.2f);
                item.IfChangeState(ItemSpice.State.Cut, ItemSpice.State.Spice);
                LevelControl.Ins.CheckStep(0.2f);
                SoundControl.Ins.PlayFX(LevelStep_1.Fx.Take);
                return true;
            }
            return false;
        }
    }
}