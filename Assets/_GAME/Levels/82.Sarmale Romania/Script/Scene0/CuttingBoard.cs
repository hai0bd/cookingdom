using DG.Tweening;
using Link;
using System.Collections.Generic;
using UnityEngine;
namespace HuyThanh.Cooking.SarmaleRomania
{
    public class CuttingBoard : ItemIdleBase
    {

        [SerializeField] Collider2D _collider;
        [SerializeField] Animation anim;
        [SerializeField] string animBonce;

        [SerializeField] List<ItemAlpha> cabbageMiddleLeaves;
        [SerializeField] List<ItemAlpha> cabbageMiddleLeavesAlpha;
        [SerializeField] List<Transform> cabbageMiddleLeavesTargetTF;

        private IItemMoving item;
        private bool IsEmpty => item == null || Vector2.Distance(item.TF.position, TF.position) > 0.3f;

        public override bool OnTake(IItemMoving item)
        {
            #region Beef
            if (IsEmpty && item is Beef beef && beef.IsState(Beef.State.Normal))
            {
                this.item = item;
                anim.Play(animBonce);
                item.OnMove(TF.position + new Vector3(0, 0.1f, 0), Quaternion.identity, 0.2f);
                item.OnSave(0.2f);
                return true;
            }


            if (!IsEmpty && item is Knife && this.item is Beef && this.item.IsState(Beef.State.Normal))
            {
                this.item.ChangeState(Beef.State.Cutting);
                //OnMove cua dao o trong cuttingstate beef
                item.ChangeState(Burrito.Knife.State.Cutting);
                return true;
            }
            #endregion

            #region FrokMeat
            if (IsEmpty && item is FrokMeat frokMeat && frokMeat.IsState(FrokMeat.State.Normal))
            {
                this.item = item;
                anim.Play(animBonce);
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                item.OnSave(0.2f);

                return true;
            }


            if (!IsEmpty && item is Knife && this.item is FrokMeat && this.item.IsState(FrokMeat.State.Normal))
            {

                this.item.ChangeState(FrokMeat.State.Cutting);
                //OnMove cua dao o trong cuttingstate beef
                item.ChangeState(Burrito.Knife.State.Cutting);
                return true;
            }
            #endregion

            #region SmokerRibs
            if (IsEmpty && item is SmokerRibs smokerRibs && smokerRibs.IsState(SmokerRibs.State.Normal))
            {
                this.item = item;
                anim.Play(animBonce);
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                item.OnSave(0.2f);
                return true;
            }


            if (!IsEmpty && item is ChoppingCleaver && this.item is SmokerRibs && this.item.IsState(SmokerRibs.State.Normal))
            {

                this.item.ChangeState(SmokerRibs.State.Cutting);
                //OnMove cua dao o trong cuttingstate beef
                item.ChangeState(ChoppingCleaver.State.Piceing);
                return true;
            }
            #endregion

            #region onion
            if (IsEmpty && item is Onion onion && onion.IsState(Onion.State.DoneCleaning))
            {
                this.item = item;
                anim.Play(animBonce);
                item.ChangeState(Onion.State.ReadyPeel);
                item.OnMove(TF.position + new Vector3(0, 0.1f, 0), Quaternion.identity, 0.2f);
                item.OnSave(0.2f);
                return true;
            }


            if (!IsEmpty && item is Knife && this.item is Onion && this.item.IsState(Onion.State.ReadyPeel))
            {
                this.item.ChangeState(Onion.State.Peel);
                item.OnMove(TF.position + new Vector3(0, 0.3f, 0), Quaternion.identity, 0.2f);

                item.ChangeState(Knife.State.PeelOnion);

                return true;
            }



            if (!IsEmpty && item is Knife && this.item is Onion && this.item.IsState(Onion.State.ReadyCutting))
            {
                this.item.ChangeState(Onion.State.Cutting);

                item.ChangeState(Knife.State.Cutting);
                return true;
            }
            if (!IsEmpty && item is Knife && this.item is Onion && this.item.IsState(Onion.State.ReadyPice))
            {
                this.item.ChangeState(Onion.State.Pice);

                return true;
            }
            #endregion

            #region Cabage
            if (IsEmpty && item is Cabbage cabbage && cabbage.IsState(Cabbage.State.DoneCleaning))
            {
                this.item = item;
                anim.Play(animBonce);
                cabbage.SetCuttingBoard(this);
                item.ChangeState(Cabbage.State.Peel);
                item.OnMove(TF.position + Vector3.up * 0.1f, Quaternion.identity, 0.2f);
                item.OnSave(0.2f);
                return true;
            }

            if (!IsEmpty && item is Knife && this.item is Cabbage && this.item.IsState(Cabbage.State.RemoveCore))
            {
                this.item.ChangeState(Cabbage.State.MoveCoreTotrash);
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                item.ChangeState(Knife.State.RemoveCoreCabbage);
                return true;
            }
            #endregion

            #region Dill
            if (IsEmpty && item is Dill dill && dill.IsState(Dill.State.DoneCleaning))
            {
                this.item = item;
                this.item.ChangeState(Dill.State.ReadyCutting);
                anim.Play(animBonce);
                item.OnMove(TF.position + Vector3.up * 0.1f, Quaternion.identity, 0.2f);
                item.OnSave(0.2f);
                return true;
            }

            if (!IsEmpty && item is Knife && this.item is Dill && this.item.IsState(Dill.State.ReadyCutting))
            {
                item.OnMove(this.item.TF.position + Vector3.right * 0.3f + Vector3.down * 0.2f, Quaternion.identity, 0.2f);
                item.ChangeState(Knife.State.CuttingDill);
                this.item.ChangeState(Dill.State.Cutting);
                return true;
            }
            #endregion

            #region Dill Pice
            if (IsEmpty && item is DillPice dillpice && dillpice.IsState(DillPice.State.DoneCleaning))
            {
                this.item = item;
                this.item.ChangeState(DillPice.State.ReadyCutting);
                anim.Play(animBonce);

                item.OnMove(TF.position + new Vector3(0, 0.1f, 0), Quaternion.identity, 0.2f);
                item.OnSave(0.2f);
                return true;
            }

            if (!IsEmpty && item is Knife && this.item is DillPice && this.item.IsState(DillPice.State.ReadyCutting))
            {
                this.item.ChangeState(DillPice.State.Cutting);
                item.OnMove(this.item.TF.position + new Vector3(0.4f, -0.4f, 0), Quaternion.identity, 0.2f);
                item.ChangeState(Knife.State.CuttingDill);
                return true;
            }

            if (!IsEmpty && item is Knife && this.item is DillPice && this.item.IsState(DillPice.State.ReadyPice1))
            {
                item.ChangeState(Knife.State.Cutting);
                item.OnMove(this.item.TF.position + new Vector3(-0.4f, -0.4f, 0), Quaternion.identity, 0.2f);
                this.item.ChangeState(DillPice.State.Pice1);

                return true;
            }
            #endregion

            #region CabbageLeaf

            if (IsEmpty && item is CabbageLeaf && item.IsState(CabbageLeaf.State.Normal))
            {
                this.item = item;
                item.OnMove(TF.position + new Vector3(0, 0.1f, 0), Quaternion.identity, 0.2f);
                item.ChangeState(CabbageLeaf.State.ReadyCutting);
                return true;
            }

            if (!IsEmpty && item is Knife && this.item is CabbageLeaf cabbageLeaf && this.item.IsState(CabbageLeaf.State.ReadyCutting))
            {
                cabbageLeaf.SetKnife(item as Knife);
                cabbageLeaf.SetCuttingBoard(this);
                item.ChangeState(Knife.State.Cutting);
                this.item.ChangeState(CabbageLeaf.State.Cutting);

                return true;
            }
            #endregion

            #region CabbageLeafCutting

            if (IsEmpty && item is CabbageLeafCutting && item.IsState(CabbageLeafCutting.State.Normal))
            {
                this.item = item;
                item.OnMove(TF.position + new Vector3(0, 0.1f, 0) + Vector3.left * 0.2f, Quaternion.identity, 0.2f);
                item.ChangeState(CabbageLeafCutting.State.ReadyCutting);
                return true;
            }

            if (!IsEmpty && item is Knife && this.item is CabbageLeafCutting cabbageLeafCutting && this.item.IsState(CabbageLeafCutting.State.ReadyCutting))
            {
                cabbageLeafCutting.SetKnife(item as Knife);
                item.ChangeState(Knife.State.Cutting);
                item.OnMove(TF.position + Vector3.down * 0.2f + Vector3.left * 0.2f, Quaternion.identity, 0.2f);
                this.item.ChangeState(CabbageLeafCutting.State.Cutting);

                return true;
            }
            #endregion

            return false;
        }

        public override void OnClickDown()
        {
            base.OnClickDown();

            if (!IsEmpty && item is Beef && item.IsState(Beef.State.Cutting))
            {
                item.OnClickDown();
                //anim.Play(animBonce);
                return;
            }

            if (!IsEmpty && item is Beef && item.IsState(Beef.State.Piecing))
            {
                item.OnClickDown();
                // anim.Play(animBonce);
                return;
            }
            if (!IsEmpty && item is FrokMeat && item.IsState(FrokMeat.State.Cutting))
            {
                item.OnClickDown();
                // anim.Play(animBonce);
                return;
            }

            if (!IsEmpty && item is FrokMeat && item.IsState(FrokMeat.State.Piecing))
            {
                item.OnClickDown();
                // anim.Play(animBonce);
                return;
            }

            if (!IsEmpty && item is SmokerRibs && item.IsState(SmokerRibs.State.Cutting))
            {
                item.OnClickDown();
                //anim.Play(animBonce);
                return;
            }

            if (!IsEmpty && item is DillPice && item.IsState(DillPice.State.Pice1))
            {
                item.OnClickDown();
                // anim.Play(animBonce);
                return;
            }

            if (!IsEmpty && item is DillPice && item.IsState(DillPice.State.Pice2))
            {
                item.OnClickDown();
                // anim.Play(animBonce);
                return;
            }

            if (!IsEmpty && item is Onion && item.IsState(Onion.State.Cutting))
            {
                item.OnClickDown();
                //anim.Play(animBonce);
                return;
            }

            if (!IsEmpty && item is CabbageLeaf && item.IsState(CabbageLeaf.State.Cutting))
            {
                item.OnClickDown();
                return;
            }

            if (!IsEmpty && item is CabbageLeafCutting && item.IsState(CabbageLeafCutting.State.Cutting))
            {
                item.OnClickDown();
                return;
            }

            if (!IsEmpty && item is CabbageLeafCutting && item.IsState(CabbageLeafCutting.State.Cutting2))
            {
                item.OnClickDown();
                return;
            }
        }

        public void SetCollider(bool value)
        {
            _collider.enabled = value;
        }

        public void DoMiddleLeavesMove()
        {
            cabbageMiddleLeaves[0].DoAlpha(1, 0.1f);
            cabbageMiddleLeaves[0].transform.DOMove(cabbageMiddleLeavesTargetTF[0].position, 0.2f);

            cabbageMiddleLeaves.RemoveAt(0);
            cabbageMiddleLeavesTargetTF.RemoveAt(0);
        }

        public void MiddleLeavesDisapper()
        {
            foreach (ItemAlpha itemAlpha in cabbageMiddleLeavesAlpha)
            {
                itemAlpha.DoAlpha(0, 0.2f);
            }
        }

        public void ReleaseCabbageLeaf()
        {
            item = null;
        }
    }
}

