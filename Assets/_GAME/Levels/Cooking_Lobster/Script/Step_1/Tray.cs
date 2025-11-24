using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link.Cooking.Lobster
{
    public class Tray : ItemIdleBase
    {
        private bool isDone;
        [SerializeField] SpriteRenderer line;
        public override bool IsDone => isDone;
        [SerializeField] GameObject wetVFX;

        public override bool OnTake(IItemMoving item)
        {
            if (item is Lobster && item.IsState(Lobster.State.Cleaned) && wetVFX == null)
            {
                item.ChangeState(Lobster.State.InTrayCleaned);
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                LevelControl.Ins.CheckStep(0.2f);
                isDone = true;
                line.sortingOrder = 2;
                LevelControl.Ins.SetStep(LevelName.Lobster, Step.LobsterInTray);
                return true;
            }

            if (item is Lobster && item.IsState(Lobster.State.Dirty) && wetVFX != null)
            {
                item.SetOrder(-5);
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                wetVFX.SetActive(false);
                return true;
            }

            return base.OnTake(item);
        }

        public void SetLineDown()
        {
            line.sortingOrder = -1;  
        }
    }
}