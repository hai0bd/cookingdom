using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Link.Cooking.Lobster
{
    public class DecorePoint : ItemIdleBase
    {
        [SerializeField] SortingGroup sortingGroup;
        [SerializeField] Collider2D collider;

        public override int OrderLayer => sortingGroup.sortingOrder;
        [field: SerializeField] public DecoreItem.NameType Name { get; private set; }

        public Action<DecorePoint> OnDoneAction;

        public override bool OnTake(IItemMoving item)
        {
            if (item is DecoreItem decore && Name == decore.Name)
            {
                item.TF.SetParent(TF);
                item.OnMove(TF.position, TF.rotation, 0.2f);
                item.OnDone();
                item.SetOrder(sortingGroup.sortingOrder);
                OnDone();
                LevelControl.Ins.SetStep(LevelName.Lobster, Step.DecoreInFinish);
                return true;
            }
            else if (item is WoodBowl && Name == DecoreItem.NameType.WoodBowl)
            {
                item.TF.SetParent(TF);
                // item.TF.position = TF.position;
                item.OnMove(TF.position, TF.rotation, 0.2f);
                item.OnDone();
                item.SetOrder(sortingGroup.sortingOrder);
                OnDone();
                LevelControl.Ins.SetStep(LevelName.Lobster, Step.MixInFinish);
                LevelControl.Ins.SetHintTextDone(6, 5);
                return true;
            }
            else if (item is LobsterPiece piece && Name == piece.Name)
            {
                item.TF.SetParent(TF);
                item.OnMove(TF.position, TF.rotation, 0.2f);
                item.OnDone();
                item.SetOrder(sortingGroup.sortingOrder);
                OnDone();
                LevelControl.Ins.SetStep(LevelName.Lobster, Step.LobsterPieceInFinish);
                return true;
            }
            return base.OnTake(item);
        }

        public override void OnDone()
        {
            collider.enabled = false;
            base.OnDone();
            OnDoneAction?.Invoke(this); 
        }

        protected override void Editor()
        {
            sortingGroup = GetComponent<SortingGroup>();
            collider = GetComponent<Collider2D>();
            base.Editor();
        }
    }
}