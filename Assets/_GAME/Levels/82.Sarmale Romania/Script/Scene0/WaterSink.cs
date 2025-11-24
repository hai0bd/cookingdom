using DG.Tweening;
using Link;
using UnityEngine;
namespace HuyThanh.Cooking.SarmaleRomania
{
    public class WaterSink : ItemIdleBase
    {
        [SerializeField] ItemAlpha waterAlpha;
        [SerializeField] ItemAlpha waterFallAlpha;
        [SerializeField] Transform itemTF, waterCoverTF;
        [SerializeField] ParticleSystem splashWaterVFX;

        private ItemMovingBase item;
        private WaterCover waterCover;

        bool waterTapOn = false;
        bool isWater = false;

        public bool waterCoverClose => waterCover != null && Vector2.Distance(waterCover.TF.position, waterCoverTF.position) < 0.4f;

        public bool haveItem => item != null && Vector2.Distance(item.TF.position, itemTF.position) < 0.2f;

        public override bool OnTake(IItemMoving item)
        {
            if (item is WaterCover)
            {
                this.waterCover = item as WaterCover;

                if (Vector2.Distance(item.TF.position, waterCoverTF.position) < 0.4f)
                {
                    //gan nen dong lai
                    item.OnMove(waterCoverTF.position, Quaternion.identity, 0.2f);
                    item.OrderLayer = -49;
                    DOVirtual.DelayedCall(0.2f, () => OnCloseCover());
                }
                else
                {
                    // xa qua nen tinh la mo
                    OnCloseCover();
                }

                return true;
            }

            if (item is Onion && item.IsState(Onion.State.Dirty) && !haveItem && isWater)
            {
                this.item = item as ItemMovingBase;
                item.OnMove(itemTF.position, Quaternion.identity, 0.2f);
                item.OnSave(0.2f);
                item.ChangeState(Onion.State.Cleaning);

                DOVirtual.DelayedCall(0.2f, () =>
                {
                    item.OrderLayer = -40;

                    splashWaterVFX.transform.position = item.TF.position;
                    splashWaterVFX.Play();
                });
                return true;
            }

            if (item is Cabbage && item.IsState(Cabbage.State.Dirty) && !haveItem && isWater)
            {
                this.item = item as ItemMovingBase;
                item.OnMove(itemTF.position, Quaternion.identity, 0.2f);
                item.OnSave(0.2f);
                item.ChangeState(Cabbage.State.Cleaning);

                DOVirtual.DelayedCall(0.2f, () =>
                {
                    item.OrderLayer = -40;

                    splashWaterVFX.transform.position = item.TF.position;
                    splashWaterVFX.Play();
                });
                return true;
            }
            if (item is Dill && item.IsState(Dill.State.Dirty) && !haveItem && isWater)
            {
                this.item = item as ItemMovingBase;
                item.OnMove(itemTF.position, Quaternion.identity, 0.2f);
                item.OnSave(0.2f);
                item.ChangeState(Dill.State.Cleaning);

                DOVirtual.DelayedCall(0.2f, () =>
                {
                    item.OrderLayer = -40;

                    splashWaterVFX.transform.position = item.TF.position;
                    splashWaterVFX.Play();
                });
                return true;
            }

            if (item is DillPice && item.IsState(DillPice.State.Dirty) && !haveItem && isWater)
            {
                this.item = item as ItemMovingBase;
                item.OnMove(itemTF.position, Quaternion.identity, 0.2f);
                item.OnSave(0.2f);
                item.ChangeState(DillPice.State.Cleaning);

                DOVirtual.DelayedCall(0.2f, () =>
                {
                    item.OrderLayer = -40;

                    splashWaterVFX.transform.position = item.TF.position;
                    splashWaterVFX.Play();
                });
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
                //SoundControl.Ins.PlayFX(Fx.WaterFall, true);
            }
            else
            {
                waterFallAlpha.DoAlpha(0f, 0.2f);
                //SoundControl.Ins.StopFX(Fx.WaterFall);
            }
        }

        private void CheckWash()
        {
            if (waterTapOn && waterCoverClose) isWater = true;
            if (!waterCoverClose) isWater = false;
            if (isWater)
            {
                waterAlpha.DoAlpha(0.8f, 0.4f);
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

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(waterCoverTF.position, 0.4f);
            Gizmos.DrawWireSphere(itemTF.position, 0.2f);
        }
    }
}
