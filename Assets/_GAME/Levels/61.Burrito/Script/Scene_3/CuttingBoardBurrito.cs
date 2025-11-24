using Link;
using UnityEngine;

namespace HuyThanh.Cooking.Burrito
{
    public class CuttingBoardBurrito : ItemIdleBase
    {
        public bool isHaveCrust => crust != null && Vector3.Distance(TF.position, crust.TF.position) < 0.1f;
        private Crust crust;
        public override bool OnTake(IItemMoving item)
        {
            if (!isHaveCrust && item is Crust && item.IsState(Crust.State.Normal))
            {
                crust = (Crust)item;
                item.OnSave(0.2f);
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                item.ChangeState(Crust.State.OnCuttingBoard);
                return true;
            }

            if (isHaveCrust && item is Spoon spoon && crust.CanTakeSpoon(spoon))
            {
                crust.RemoveSpoon();
                crust.SpiceActive();
                item.OnMove(TF.position + Vector3.up * 0.5f + spoon.GetPouringPosition(), Quaternion.identity, 0.2f);
                item.ChangeState(Spoon.State.Pouring);
                return true;
            }
            return base.OnTake(item);
        }

        public override void OnClickDown()
        {
            base.OnClickDown();

            if (isHaveCrust && crust.IsState(Crust.State.DoneSpice))
            {
                SoundControl.Ins.PlayFX(Fx.Click);
                crust.ChangeState(Crust.State.Pack1);
                return;
            }

            if (isHaveCrust && crust.IsState(Crust.State.Pack1))
            {
                SoundControl.Ins.PlayFX(Fx.Click);
                crust.ChangeState(Crust.State.Pack2);
                return;
            }

            if (isHaveCrust && crust.IsState(Crust.State.Pack2))
            {
                SoundControl.Ins.PlayFX(Fx.Click);
                crust.ChangeState(Crust.State.Scrolling);
                return;
            }
        }


        public void TurnCollider(bool isOn)
        {
            gameObject.GetComponent<Collider2D>().enabled = isOn;
        }
    }
}