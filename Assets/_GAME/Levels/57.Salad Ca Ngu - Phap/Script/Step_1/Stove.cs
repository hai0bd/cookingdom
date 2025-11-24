
using DG.Tweening;
using Link;
using Satisgame;
using UnityEngine;

namespace HuyThanh.Cooking.TunaSaladFrench
{
    public class Stove : ItemIdleBase
    {
        [SerializeField] SpriteRenderer heatSprite;
        [SerializeField] private ClockTimer _clockTimer;
        [SerializeField] private Pot pot;
        [SerializeField] private NapNoi napNoi;
        [SerializeField] private Egg egg;
        [SerializeField] private Potato potato;
        [SerializeField] private Bean bean;
        [SerializeField] EmojiControl _emoji;

        private bool IsEmpty => (pot == null || Vector3.Distance(TF.position, pot.TF.position) > 0.5f);
        private bool IsEmptyFood => (egg == null || Vector3.Distance(TF.position, egg.TF.position) > 0.35f) &&
                                    (bean == null || Vector3.Distance(TF.position, bean.TF.position) > 0.35f) &&
                                    (potato == null || Vector3.Distance(TF.position, potato.TF.position) > 0.35f);
        private bool isOn;

        private IItemMoving item;
        public void OnStoveButtonClick(bool isOn)
        {
            this.isOn = isOn;

            heatSprite.DOFade(this.isOn ? 1f : 0f, 0.4f);
            LevelControl.Ins.CheckStep();

            if (!IsEmpty)
            {
                if (isOn == false)
                {
                    if (haveCookEgg && haveCookBean && haveCookPotato)
                    {
                        pot.ChangeState(Pot.State.PourWater);
                        return;
                    }
                }

                if (this.isOn) pot.ChangeState(Pot.State.Boil);
                else pot.ChangeState(Pot.State.NotBoil);
            }
        }
        public override void OnClickDown()
        {
            base.OnClickDown();
        }

        public override bool OnTake(IItemMoving item)
        {
            if (item is Pot pot1 && (item.IsState(Pot.State.HaveWater) || item.IsState(Pot.State.DonePour) || item.IsState(Pot.State.PourWater)))
            {
                this.pot = pot1;
                item.OnMove(TF.position + Vector3.left * 0.2f, Quaternion.identity, 0.2f);
                DOVirtual.DelayedCall(0.21f, () => pot.OrderLayer = -49);

                if (this.isOn) pot.ChangeState(Pot.State.Boil);
                return true;
            }

            if ((IsEmptyFood || this.item == item) && item is Egg egg1 && item.IsState(Egg.State.Normal) && pot != null && pot.IsState(Pot.State.Boil) && !pot.IsHaveCover)
            {
                this.item = item as ItemMovingBase;
                item.OnMove(TF.position + Vector3.left * 0.2f + Vector3.up * 0.1f, Quaternion.identity, 0.2f);
                DOVirtual.DelayedCall(0.2f, () => egg1.OrderLayer = 2);
                this.egg = egg1;
                return true;
            }

            if ((IsEmptyFood || this.item == item) && item is Potato potato1 && item.IsState(Potato.State.Raw) && pot != null && pot.IsState(Pot.State.Boil) && !pot.IsHaveCover)
            {
                this.item = item as ItemMovingBase;
                item.OnMove(TF.position + Vector3.left * 0.2f, Quaternion.identity, 0.2f);
                DOVirtual.DelayedCall(0.2f, () => potato1.OrderLayer = 2);
                this.potato = potato1;
                return true;
            }

            if ((IsEmptyFood || this.item == item) && item is Bean bean1 && item.IsState(Bean.State.Raw) && pot != null && pot.IsState(Pot.State.Boil) && !pot.IsHaveCover)
            {
                this.item = item as ItemMovingBase;
                item.OnMove(TF.position + Vector3.left * 0.2f, Quaternion.identity, 0.2f);
                DOVirtual.DelayedCall(0.2f, () => bean1.OrderLayer = 2);
                this.bean = bean1;
                return true;
            }

            if (item is NapNoi napNoi1 && pot != null && Vector2.Distance(pot.TF.position, TF.position) < 0.4f && pot.IsState(Pot.State.Boil))
            {
                item.OnMove(TF.position + Vector3.left * 0.2f + Vector3.up * 0.2f, Quaternion.identity, 0.2f);
                DOVirtual.DelayedCall(0.2f, () => napNoi1.OrderLayer = 4);
                pot.ChangeSteam();
                if (egg != null && Vector3.Distance(TF.position, egg.TF.position) < 0.5f && egg.IsState(Egg.State.Normal))
                {
                    egg.ChangeState(Egg.State.Cooking);
                    _clockTimer.Show(4f);
                    napNoi1.ChangeState(NapNoi.State.Cooking);
                    haveCookEgg = true;
                }
                if (bean != null && Vector3.Distance(TF.position, bean.TF.position) < 0.5f && bean.IsState(Bean.State.Raw))
                {
                    bean.ChangeState(Bean.State.Cooking);
                    _clockTimer.Show(4f);
                    napNoi1.ChangeState(NapNoi.State.Cooking);
                    haveCookBean = true;
                }
                if (potato != null && Vector3.Distance(TF.position, potato.TF.position) < 0.5f && potato.IsState(Potato.State.Raw))
                {
                    potato.ChangeState(Potato.State.Cooking);
                    _clockTimer.Show(4f);
                    napNoi1.ChangeState(NapNoi.State.Cooking);
                    haveCookPotato = true;
                }


                return true;
            }

            if (item is NapNoi napNoi2 && pot != null && Vector2.Distance(pot.TF.position, TF.position) < 0.4f && pot.IsState(Pot.State.DonePour) && IsEmptyFood)
            {
                item.OnMove(TF.position + Vector3.left * 0.2f + Vector3.up * 0.2f, Quaternion.identity, 0.2f);
                DOVirtual.DelayedCall(0.2f, () => napNoi2.OrderLayer = 2);
                pot.ChangeState(Pot.State.Done);
                napNoi2.ChangeState(NapNoi.State.Done);///khoa lai cai noi
                LevelControl.Ins.CheckStep(0.2f);
                return true;
            }

            item.OnBack();
            return base.OnTake(item);
        }

        private bool haveCookPotato, haveCookEgg, haveCookBean;
        public bool CanUseButton()
        {
            //return pot == null || !pot.IsState(Pot.State.Boil) || (haveCookBean && haveCookEgg && haveCookPotato);
            return pot != null && (pot.IsState(Pot.State.NotBoil) || pot.IsState(Pot.State.Boil) || pot.IsState(Pot.State.HaveWater)) &&
                !napNoi.IsState(NapNoi.State.Cooking) && IsEmptyFood;
        }

    }
}