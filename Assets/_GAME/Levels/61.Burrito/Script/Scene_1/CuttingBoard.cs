using Link;
using UnityEngine;

namespace HuyThanh.Cooking.Burrito
{
    public class CuttingBoard : ItemIdleBase
    {
        [SerializeField] Animation anim;
        [SerializeField] string animChop;

        [SerializeField] Sprite cornHint;

        private IItemMoving item;
        private bool IsEmpty => item == null || Vector2.Distance(item.TF.position, TF.position) > 0.2f;

        public override bool OnTake(IItemMoving item)
        {
            #region BeefRaw
            if (IsEmpty && item is BeefRaw && item.IsState(BeefRaw.State.Normal))
            {
                this.item = item;
                item.OnMove(TF.position + Vector3.up * 0.1f, Quaternion.identity, 0.2f);
                anim.Play(animChop);
                return true;
            }

            if (!IsEmpty && item is Knife && this.item is BeefRaw && this.item.IsState(BeefRaw.State.Normal))
            {
                this.item.ChangeState(BeefRaw.State.Cutting);
                //OnMove cua dao o trong cuttingstate beef
                item.ChangeState(Knife.State.Cutting);
                return true;
            }
            #endregion

            #region BasketCutItem

            if (IsEmpty && item is BasketCutItem && item.IsState(BasketCutItem.State.Normal))
            {
                this.item = item;
                item.OnMove(TF.position + Vector3.up * 0.1f, Quaternion.identity, 0.2f);
                anim.Play(animChop);
                return true;
            }

            if (!IsEmpty && item is Knife && this.item is BasketCutItem && this.item.IsState(BasketCutItem.State.Normal))
            {
                this.item.ChangeState(BasketCutItem.State.Cutting1);
                //OnMove cua dao o trong cuttingstate basketcutitem
                item.ChangeState(Knife.State.Cutting);
                return true;
            }

            #endregion

            #region Garlic

            if (IsEmpty && item is GarlicCutItem && item.IsState(GarlicCutItem.State.Normal))
            {
                this.item = item;
                item.OnMove(TF.position + Vector3.up * 0.1f, Quaternion.identity, 0.2f);
                anim.Play(animChop);
                return true;
            }

            if (!IsEmpty && item is Knife && this.item is GarlicCutItem && this.item.IsState(GarlicCutItem.State.Normal))
            {
                this.item.ChangeState(GarlicCutItem.State.Cutting1);
                //OnMove cua dao o trong cuttingstate garliccutitem
                item.ChangeState(Knife.State.Cutting);
                return true;
            }
            #endregion

            #region CornCutItem

            if (IsEmpty && item is CornCutItem && item.IsState(CornCutItem.State.Normal))
            {
                this.item = item;
                LevelControl.Ins.SetHint(cornHint); /// dat vao thot thi set hint cho ngo
                item.OnMove(TF.position + Vector3.up * 0.1f, Quaternion.identity, 0.2f);
                anim.Play(animChop);
                return true;
            }

            if (!IsEmpty && item is Knife && this.item is CornCutItem && this.item.IsState(CornCutItem.State.Normal))
            {
                this.item.ChangeState(CornCutItem.State.Cutting);
                //OnMove cua dao o trong cuttingstate
                item.ChangeState(Knife.State.CornCutting);
                return true;
            }

            #endregion
            return base.OnTake(item);
        }

        public override void OnClickDown()
        {
            base.OnClickDown();

            if (!IsEmpty && item is BeefRaw && item.IsState(BeefRaw.State.Cutting))
            {
                item.OnClickDown();
                anim.Play(animChop);
                return;
            }

            if (!IsEmpty && item is BasketCutItem && (item.IsState(BasketCutItem.State.Cutting1) || item.IsState(BasketCutItem.State.Cutting2)))
            {
                item.OnClickDown();
                anim.Play(animChop);
                return;
            }

            if (!IsEmpty && item is GarlicCutItem && (item.IsState(GarlicCutItem.State.Cutting1) || item.IsState(GarlicCutItem.State.Cutting2)))
            {
                item.OnClickDown();
                anim.Play(animChop);
                return;
            }

            if (!IsEmpty && item is CornCutItem && item.IsState(CornCutItem.State.Cutting))
            {
                item.OnClickDown();
                anim.Play(animChop);
                return;
            }

        }
    }
}