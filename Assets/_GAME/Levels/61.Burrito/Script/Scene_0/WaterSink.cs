using DG.Tweening;
using Link;
using UnityEngine;

namespace HuyThanh.Cooking.Burrito
{
    public class WaterSink : ItemIdleBase
    {
        [SerializeField] ItemAlpha waterAlpha;
        [SerializeField] ItemAlpha waterFallAlpha;

        [SerializeField] ParticleSystem vfxSplashWater;

        ItemMovingBase item;
        WaterCover waterCover;

        bool waterTapOn = false;
        bool isWater = false;

        bool waterCoverClose => waterCover != null && Vector2.Distance(waterCover.TF.position, TF.position) < 0.75f;

        public bool haveItem => item != null && Vector2.Distance(item.TF.position, TF.position) < 0.2f;

        public override bool OnTake(IItemMoving item)
        {
            if (item is WaterCover)
            {
                this.waterCover = item as WaterCover;

                if (Vector2.Distance(item.TF.position, TF.position) < 0.75f)
                {
                    //gan nen dong lai
                    item.OnMove(TF.position + Vector3.down * (0.49f + 0.075f) + Vector3.right * (0.35f - 0.075f), Quaternion.identity, 0.2f);
                    DOVirtual.DelayedCall(0.2f, () => OnCloseCover());
                    item.OrderLayer = -50;
                }
                else
                {
                    // xa qua nen tinh la mo
                    OnCloseCover();
                }

                return true;
            }

            if (item is DirtyItem && item.IsState(DirtyItem.State.Dirty) && isWater && !haveItem)
            {
                this.item = item as ItemMovingBase;
                item.OnMove(TF.position + Vector3.left * 0.15f, Quaternion.identity, 0.2f);
                item.OnSave(0.2f);
                item.ChangeState(DirtyItem.State.Cleaning);

                DOVirtual.DelayedCall(0.2f, () =>
                {
                    item.OrderLayer = -1;

                    vfxSplashWater.transform.position = item.TF.position;
                    vfxSplashWater.Play();
                });
                return true;
            }

            if (item is Colander && item.IsState(Colander.State.HaveBean) && isWater && !haveItem)
            {
                this.item = item as ItemMovingBase;
                item.OnMove(TF.position + Vector3.left * 0.15f, Quaternion.identity, 0.2f);
                item.OnSave(0.2f);
                item.ChangeState(Colander.State.Washing);

                return true;
            }

            if (item is MeatClean && item.IsState(MeatClean.State.Dirty) && isWater && !haveItem)
            {
                this.item = item as ItemMovingBase;
                item.OnMove(TF.position + Vector3.left * 0.15f, Quaternion.Euler(0, 0, 90f), 0.2f);

                item.ChangeState(MeatClean.State.Cleaning);

                DOVirtual.DelayedCall(0.2f, () =>
                {
                    item.OrderLayer = -1;

                    vfxSplashWater.transform.position = item.TF.position;
                    vfxSplashWater.Play();
                });
                return true;
            }

            if ((item is GarlicRotten || item is TrashItem) && isWater && !haveItem)
            {
                item.OnBack();
                LevelControl.Ins.LoseHalfHeart(TF.position);

                return true;
            }


            return base.OnTake(item);
        }

        public void OnCloseCover()
        {
            CheckWash();
        }
        public void OnOpenWater(bool isOn)
        {
            waterTapOn = isOn;
            CheckWash();

            if (isOn)
            {
                ///bat nuoc
                waterFallAlpha.DoAlpha(1f, 0.2f);
                SoundControl.Ins.PlayFX(Fx.WaterFall, true);
            }
            else
            {
                waterFallAlpha.DoAlpha(0f, 0.2f);
                SoundControl.Ins.StopFX(Fx.WaterFall);
            }
        }

        private void CheckWash()
        {
            if (waterTapOn && waterCoverClose) isWater = true;
            if (!waterCoverClose) isWater = false;
            if (isWater)
            {
                waterAlpha.DoAlpha(1f, 0.4f);
            }
            else if (!waterCoverClose)
            {
                waterCover?.OnBack();
                waterAlpha.DoAlpha(0, 0.4f);
                LevelControl.Ins.CheckStep(1f);
            }
        }

        public override void OnClickDown()
        {
            base.OnClickDown();

            if (item != null && haveItem)
            {
                item.OnClickDown();
                return;
            }
        }

        public bool CheckWin()
        {
            return isWater == false && waterCoverClose == false;
        }
    }
}
