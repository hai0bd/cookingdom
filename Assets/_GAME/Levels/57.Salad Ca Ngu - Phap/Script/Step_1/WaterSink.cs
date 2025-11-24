using DG.Tweening;
using Link;
using UnityEngine;

namespace HuyThanh.Cooking.TunaSaladFrench
{
    public class WaterSink : ItemIdleBase
    {
        [SerializeField] SpriteRenderer water;
        [SerializeField] GameObject waterFall;

        private Pot _pot;
        private bool HavePot => _pot != null && Vector2.Distance(TF.position, _pot.TF.position) < 0.35f;

        WaterCover waterCover;
        bool waterTapOn = false;
        bool waterCoverClose => waterCover != null && Vector2.Distance(waterCover.TF.position, transform.position) < 0.35f;
        bool isWater = false;

        public void OnOpenWater(bool isOn)
        {
            waterTapOn = isOn;
            CheckWash();
            waterFall.SetActive(isOn);

            if (isOn)
            {
                SoundControl.Ins.PlayFX(Fx.WaterFall, true);

            }
            else
            {
                SoundControl.Ins.StopFX(Fx.WaterFall);
            }
            if (HavePot)
            {
                _pot.IfChangeState(Pot.State.Normal, Pot.State.HaveWater);
            }
        }

        public override bool OnTake(IItemMoving item)
        {
            if (item is WaterCover)
            {
                this.waterCover = item as WaterCover;

                if (Vector2.Distance(item.TF.position, transform.position) < 0.35f)
                {
                    //neu o du gan thi dong lai
                    item.OnMove(TF.position, Quaternion.identity, 0.2f);
                    DOVirtual.DelayedCall(0.2f, () => item.OrderLayer = -45);
                    Invoke(nameof(OnCloseCover), 0.2f);
                    item.OrderLayer = 0;
                }
                else
                {
                    //neu xa qua tinh la mo
                    OnCloseCover();
                }

                return true;
            }
            if (item is Pot && item.IsState(Pot.State.Normal) && isWater == false)
            {
                _pot = item as Pot;
                if (waterTapOn)
                {/// voi nuoc bat va can them nuoc thi doi state
                    item.IfChangeState(Pot.State.Normal, Pot.State.HaveWater);
                }
                item.OnMove(TF.position + Vector3.left * 0.05f + Vector3.up * 0.25f, Quaternion.identity, 0.2f);
                //item.OnSave(0.2f);
                return true;
            }

            if (item is Pot && item.IsState(Pot.State.PourWater) && isWater == false)
            {
                if (waterTapOn == false)
                {/// voi nuoc can tat
                    item.ChangeState(Pot.State.PouringWater);
                }
                item.OnMove(TF.position + Vector3.left * 0.05f + Vector3.up * 0.25f, Quaternion.identity, 0.2f);
                //item.OnSave(0.2f);
                return true;
            }

            if (item is XaLach && item.IsState(XaLach.State.Dirty) && isWater)
            {
                item.ChangeState(XaLach.State.Washing);
                item.OnMove(TF.position + Vector3.right * 0.15f, Quaternion.identity, 0.2f);
                item.OnSave(0.2f);
                return true;
            }

            item.OnBack();

            return base.OnTake(item);
        }

        public void OnCloseCover()
        {
            CheckWash();
            LevelControl.Ins.CheckStep();
        }

        private void CheckWash()
        {
            if (waterTapOn && waterCoverClose) isWater = true;
            if (!waterCoverClose) isWater = false;
            if (isWater)
            {
                water.DOFade(1f, 0.4f);
                if (this._pot != null)
                {
                    _pot.OnBack();
                }
            }
            else if (!waterCoverClose)
            {
                waterCover?.OnBack();
                water.DOFade(0, 0.4f);
            }
        }

        public bool CheckDone()
        {
            return waterTapOn == false && waterCoverClose == false;
        }

    }

}
