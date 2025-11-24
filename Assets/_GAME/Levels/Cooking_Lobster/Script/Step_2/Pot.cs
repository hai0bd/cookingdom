using DG.Tweening;
using Satisgame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link.Cooking.Lobster
{
    public class Pot : ItemIdleBase
    {
        [SerializeField] EmojiControl emoji;
        [SerializeField] PotLid potLid;
        [SerializeField] Transform potLidPoint;
        [SerializeField] Transform beerWater;
        [SerializeField] Transform potParent;
        [SerializeField] ParticleSystem smoke;
        [SerializeField] Lobster lobster;
        [SerializeField] ElectricStove electricStove;

        List<IItemMoving> items = new List<IItemMoving>();
        public bool IsLidClose => Vector2.Distance(potLid.TF.position, potLidPoint.position) < 0.2f;
        public bool IsFullItem => items.Count >= 7;

        IItemMoving steamDish;
        [SerializeField] List<ItemMovingBase> orders;

        public override bool OnTake(IItemMoving item)
        {
            if (item is PotLid)
            {
                item.OnMove(potLidPoint.position, Quaternion.identity, 0.15f);
                if (IsFullItem)
                {
                    if (item.IsState(PotLid.State.Open)) emoji.ShowPositive();
                    item.IfChangeState(PotLid.State.Open, PotLid.State.Freeze);
                    item.IfChangeState(PotLid.State.StreamOpen, PotLid.State.Stream);

                    if(IsState(PotLid.State.Open)) LevelControl.Ins.SetStep(LevelName.Lobster, Step.CoverLid_1);
                    if(IsState(PotLid.State.StreamOpen)) LevelControl.Ins.SetStep(LevelName.Lobster, Step.CoverLid_2);
                    LevelControl.Ins.SetHintTextDone(3, 4);
                }
                LevelControl.Ins.CheckStep(0.175f);
                return true;
            }

            if (!IsLidClose)
            {
                if (item is SteamDish && orders[0].Equals(item))
                {
                    orders.RemoveAt(0);
                    item.TF.SetParent(potParent);
                    steamDish = item;
                    steamDish.OnMove(TF.position, Quaternion.identity, 0.1f);
                    steamDish.OnDone();
                    items.Add(item);
                    LevelControl.Ins.SetStep(LevelName.Lobster, Step.StreamDishInPot);
                    LevelControl.Ins.SetHintTextDone(3, 1);
                    return true;
                }
                if (steamDish != null)
                {
                    if (item is Lobster && item.IsState(Lobster.State.MoveToPot) && orders[0].Equals(item))
                    {
                        orders.RemoveAt(0);
                        item.TF.SetParent(TF);
                        item.TF.position = TF.position + 0.245f * Vector3.up;
                        item.ChangeState(Lobster.State.InPot);
                        items.Add(item);
                        LevelControl.Ins.SetStep(LevelName.Lobster, Step.LobsterInPot);
                        LevelControl.Ins.SetHintTextDone(3, 2);
                        return true;
                    }
                    if (item is ItemSteam && orders[0].Equals(item))
                    {
                        orders.RemoveAt(0);
                        item.TF.SetParent(potParent);
                        item.TF.position = TF.position + 0.245f * Vector3.up;
                        item.ChangeState(ItemSteam.State.Done);
                        //OrderLayer = items.Count - 8;
                        (item as ItemSteam).OnActiveAnim();
                        items.Add(item);
                        SoundControl.Ins.PlayFX(LevelStep_1.Fx.MultiTake);
                        LevelControl.Ins.SetStep(LevelName.Lobster, Step.FruitInPot);
                        return true;
                    }
                }
                if (item is Beer && orders[0].Equals(item))
                {
                    orders.RemoveAt(0);
                    item.TF.position = TF.position;
                    item.OnDone();
                    items.Add(item);
                    //show them nuoc bia
                    beerWater.DOScale(1, 0.7f).SetDelay(0.8f);
                    LevelControl.Ins.SetStep(LevelName.Lobster, Step.BearInPot);
                    LevelControl.Ins.SetHintTextDone(3, 3);
                    return true;
                }
            }
            emoji.ShowNegative();
            LevelControl.Ins.LoseFullHeart(TF.position);
            
            return base.OnTake(item);
        }

        public override void OnDone()
        {
            base.OnDone();
            potLid.ChangeState(PotLid.State.Stream);
            lobster.ChangeState(Lobster.State.Stream);
            electricStove.ChangeState(ElectricStove.State.Contact);
        }

        public void OnStream()
        {
            electricStove.ChangeState(ElectricStove.State.NoContact);
            DOVirtual.DelayedCall(6f, () => smoke.Play());
            DOVirtual.DelayedCall(12f, OnDone);
            LevelControl.Ins.SetStep(LevelName.Lobster, Step.TurnOnStove_1);
            LevelControl.Ins.SetHintTextDone(3, 5);
        }

        public void OffStove()
        {
            LevelControl.Ins.SetStep(LevelName.Lobster, Step.TurnOffStove_1);
        }

    }
}