using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Link.Cooking.Lobster
{
    public class CuttingBoard : ItemIdleBase
    {
        IItemMoving item;
        [SerializeField] LobsterKnife lobsterKnife;
        [SerializeField] Transform knifePoint;

        private bool IsEmpty => item == null || Vector2.Distance(item.TF.position, TF.position) > 0.2f;

        public override bool OnTake(IItemMoving item)
        {
            if (this.item == item || IsEmpty)
            {
                if (item is Lobster && item.IsState(Lobster.State.Dirty))
                {
                    this.item = item;
                    item.ChangeState(Lobster.State.NeedClean);
                    item.OnMove(TF.position, Quaternion.identity, 0.2f);
                    LevelControl.Ins.SetStep(LevelName.Lobster, Step.LobsterInBoard_1);
                    return true;
                }
                else
                if (item is ItemSteam && item.IsState(ItemSteam.State.Origin))
                {
                    this.item = item;
                    //item.OnMove(TF.position, Quaternion.identity, 0.2f);
                    item.TF.position = TF.position;
                    (item as ItemSteam).OnActiveAnim();
                    item.OnSave(0.1f);
                    SoundControl.Ins.PlayFX(LevelStep_1.Fx.MultiTake);
                    LevelControl.Ins.SetStep(LevelName.Lobster, Step.FruitInBoard);
                    return true;
                }
                //doan nay de cut lobster
                else if (item is Lobster && item.IsState(Lobster.State.MoveToBoard))
                {
                    item.TF.position = TF.position;
                    item.ChangeState(Lobster.State.InBoard);
                    
                    lobsterKnife.OnDone();
                    lobsterKnife.TF.DOMove(knifePoint.position, 0.5f).OnComplete(()=> lobsterKnife.ChangeState(LobsterKnife.State.Done));
                    lobsterKnife.TF.DORotate(knifePoint.eulerAngles + Vector3.forward * 360, .5f, RotateMode.WorldAxisAdd);
                    //lobsterKnife.OnMove(knifePoint.position, Quaternion.Euler(knifePoint.rotation.eulerAngles + Vector3.forward * 360) , 0.2f);
                    LevelControl.Ins.SetStep(LevelName.Lobster, Step.LobsterInBoard_2);
                    return true;
                }else if (item is ItemSpice itemSpice && item.IsState(ItemSpice.State.Origin))
                {
                    this.item = item;
                    item.OnMove(TF.position, Quaternion.identity, 0.2f);
                    item.OnSave(0.2f);
                    SoundControl.Ins.PlayFX(LevelStep_1.Fx.Take);
                    // LevelControl.Ins.SetStep(LevelName.Lobster, Step.FruitInBoard);
                    if(itemSpice.ItemName == DecoreItem.NameType.Cucumber)
                    {
                        itemSpice.ChangeState(ItemSpice.State.Cutting);
                    }
         
                    return true;
                }
            }
            if (!IsEmpty && item is Knife && this.item is ItemSteam)
            {
                return this.item.OnTake(item);
            }
            item.OnBack();
            return base.OnTake(item);
        }
    }
}