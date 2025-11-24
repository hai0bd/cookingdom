using DG.Tweening;
using Link;
using UnityEngine;

namespace HuyThanh.Cooking.Burrito
{
    public class Pot : ItemIdleBase
    {
        public enum State
        {
            Normal,
            HaveWater,
            HaveItem,
            Cooking,
            DoneCook,
            Done
        }

        [SerializeField] private State state;

        [SerializeField] ItemAlpha waterLayerAlpha;
        [SerializeField] ItemAlpha boilWaterAlpha;

        [SerializeField] ParticleSystem boilVFX;
        [SerializeField] ParticleSystem steamVFX;

        [SerializeField] ClockTimer _clockTimer;

        [SerializeField] WaterJug waterJug;

        [Header("Pot Item")]
        [SerializeField] Transform potItem;
        [SerializeField] Animation anim;
        [SerializeField] string animPotItemTake;

        [SerializeField] GameObject beanSpriteItem, cornSpriteItem;

        private BowlPotItem bowlPotItem;
        private int spiceTake;
        private int numberOfCook = 2;

        private void OnEnable()
        {
            _clockTimer._OnTimeOut.AddListener(() =>
            {
                if (IsState(State.Cooking))
                {
                    ChangeState(State.DoneCook);
                }
            });
        }

        private void OnDisable()
        {
            _clockTimer._OnTimeOut.RemoveListener(() =>
            {
                if (IsState(State.Cooking))
                {
                    ChangeState(State.DoneCook);
                }
            });
        }

        public override bool IsState<T>(T t)
        {
            return state == (State)(object)t;
        }

        public override void ChangeState<T>(T t)
        {
            state = (State)(object)t;

            switch (state)
            {
                case State.Normal:
                    break;
                case State.HaveWater:
                    waterLayerAlpha.DoAlpha(1f, 2f);
                    break;
                case State.HaveItem:
                    potItem.localScale = Vector3.one;
                    break;
                case State.Cooking:
                    SoundControl.Ins.PlayFX(Fx.WaterBoil, true);
                    boilVFX.Play();
                    boilWaterAlpha.DoAlpha(0.5f, 2f);
                    waterLayerAlpha.DoAlpha(0f, 2f);
                    _clockTimer.Show(8f);
                    break;
                case State.DoneCook:
                    spiceTake = 3;
                    boilVFX.Stop();
                    steamVFX.Play();
                    break;
                case State.Done:
                    SoundControl.Ins.StopFX(Fx.WaterBoil);
                    numberOfCook--;
                    if (numberOfCook > 0)
                    {
                        ResetStateToNormal();
                    }
                    LevelControl.Ins.NextHint();
                    LevelControl.Ins.CheckStep(1f);
                    break;
            }
        }

        public bool CheckDone()
        {
            return numberOfCook == 0 && IsState(State.Done);
        }
        public override bool OnTake(IItemMoving item)
        {
            if (item is WaterJug && item.IsState(WaterJug.State.HaveWater) && this.IsState(State.Normal))
            {
                item.ChangeState(WaterJug.State.Pouring);
                item.OnMove(TF.position, Quaternion.identity, 0.2f);

                ChangeState(Pot.State.HaveWater);
                return true;
            }

            if (bowlPotItem == null && item is BowlPotItem && item.IsState(BowlPotItem.State.Normal) && this.IsState(State.HaveWater))
            {
                bowlPotItem = item as BowlPotItem;

                item.ChangeState(BowlPotItem.State.Pouring);
                item.OnMove(TF.position, Quaternion.identity, 0.2f);

                ChangeState(Pot.State.HaveItem);
                return true;
            }
            return base.OnTake(item);
        }

        public void OnClickButton(bool isOn)
        {
            if (isOn && IsState(Pot.State.HaveItem))// neu la bat thi phai o state haveitem
            {
                ChangeState(Pot.State.Cooking);
            }
            if (isOn == false && IsState(Pot.State.DoneCook) && bowlPotItem == null) // neu tat bep
            {
                ChangeState(Pot.State.Done);
            }
        }

        public bool HaveBowlPotItem()
        {
            return bowlPotItem != null;
        }

        public Punctured.SpiceType GetSpiceType()
        {
            if (bowlPotItem != null)
            {
                return bowlPotItem.SpiceType;
            }
            return Punctured.SpiceType.None;
        }

        public bool OnTakeSpice()
        {
            if (spiceTake <= 0)
            {
                //Debug.LogError("Spice take is less than 0, can't take more spice");
                return false;
            }
            spiceTake--;
            potItem.localScale -= Vector3.one / 3f;

            if (spiceTake == 0)
            {
                bowlPotItem = null;
            }

            return true;
        }

        private void ResetStateToNormal()
        {
            waterJug.Refill();
            TF.DOMove(TF.position + Vector3.left * 5f, 0.5f).OnComplete(() =>
            {
                TF.DOMove(TF.position + Vector3.right * 5f, 0.5f);
                ChangeState(State.Normal);
                boilVFX.Stop();
                steamVFX.Stop();
                waterLayerAlpha.DoAlpha(0f, 0f);
                boilWaterAlpha.DoAlpha(0f, 0f);

                cornSpriteItem.SetActive(false);
                beanSpriteItem.SetActive(false);
            });
        }
    }

}