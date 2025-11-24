using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using JetBrains.Annotations;
using Link.Cooking.Spageti_Bear;
using UnityEngine;

namespace Link.Cooking.Spageti
{
    public enum ItemType
    {
        None = -1,
        Tomato = 0,
        Noodle = 1,
        Meat = 2,
        Mushroom = 3,
        Scallops = 4,
        Shrimp = 5,
        SauceCream = 6,
        Cheese = 7,
        Leaf = 8,
    }

    public class MainDish : ItemIdleBase
    {


        [field: SerializeField] public Transform[] ItemPoint { get; private set; }
        [SerializeField] AnimaBase2D cheeseAnim;

        [SerializeField] List<ItemType> orders;

        [SerializeField] GameObject oliu;

        [SerializeField] List<ItemDecors> tomatoes, scallops, shrimps;

        [SerializeField] HintText hintText_1, hintText_2;

        public override bool OnTake(IItemMoving item)
        {
            if (item is Spoon spoon && spoon.IsHaveMeat)
            {
                spoon.OnDone();
                ItemPoint[(int)ItemType.Meat].gameObject.SetActive(true);
                hintText_1.OnActiveHint();
            }
            if (item is ItemDecor itemDecor && IsOrder(itemDecor.ItemType) && itemDecor.ItemType != ItemType.None)
            {
                ItemPoint[(int)itemDecor.ItemType].gameObject.SetActive(true);
                item.OnDone();

                if (itemDecor.ItemType == ItemType.Cheese)
                {
                    cheeseAnim.OnActive();
                }

                if (oliu != null) oliu.SetActive(true);
                return true;
            }
            if (item is SauceJar && IsOrder(ItemType.SauceCream))
            {
                item.OnMove(TF.position + new Vector3(0.5f, 1f, 0), Quaternion.identity, 0.2f);
                item.OnDone();
                DOVirtual.DelayedCall(1f, () =>
                {
                    ItemPoint[(int)ItemType.SauceCream].gameObject.SetActive(true);
                });
                return true;
            }

            if (item is PotMix potMix)
            {
                item.OnMove(TF.position + new Vector3(0.5f, 1f, 0), Quaternion.identity, 0.2f);
                item.ChangeState(PotMix.State.Finish);
                DOVirtual.DelayedCall(0.5f, () =>
                {
                    potMix.noodleDone.TF.SetParent(TF.parent);
                    potMix.noodleDone.TF.DOMove(TF.position, 1f).OnComplete(() =>
                    {
                        IsOrder(ItemType.Noodle);
                        potMix.OnDone();
                        potMix.OnDrop();
                        potMix.noodleDone.TF.SetParent(TF);

                        LevelControl.Ins.CheckStep(0.5f);
                    });
                    potMix.noodleDone.TF.DOScale(1.35f, 0.5f);
                    potMix.OrderLayer = -40;
                    potMix.SetOrder(-40);
                });
                return true;
            }
            return false;
        }

        public bool IsOrder(ItemType itemType)
        {
            if (orders.Count > 0 && orders[0] == itemType)
            {
                orders.RemoveAt(0);
                if (orders.Count > 0 && (orders[0] == ItemType.Scallops || orders[0] == ItemType.Shrimp))
                {
                    ItemPoint[(int)orders[0]].gameObject.SetActive(true);
                }
                if (orders.Count == 1)
                {
                    hintText_2.OnActiveHint();
                }   
                return true;
            }
            return false;
        }

        public void SetOliuDone()
        {
            orders.Remove(ItemType.None);
        }
        public void SetItemDone(ItemType itemType)
        {
            orders.Remove(itemType);
        }

        public void SetTomatoDone()
        {
            orders.Remove(ItemType.Tomato);
        }
        public void SetScallopDone()
        {
            orders.Remove(ItemType.Scallops);
        }
        public void SetShirmpDone()
        {
            orders.Remove(ItemType.Shrimp);
        }

        public void SetItemDone(ItemDecors itemDecor)
        {
            if (itemDecor.ItemType == ItemType.Tomato && orders[0] == ItemType.Tomato)
            {
                tomatoes.Remove(itemDecor);
                if (tomatoes.Count == 0)
                {
                    SetItemDone(ItemType.Tomato);
                }
            }
            else if (itemDecor.ItemType == ItemType.Scallops && orders[0] == ItemType.Scallops)
            {
                scallops.Remove(itemDecor);
                if (scallops.Count == 0)
                {
                    SetItemDone(ItemType.Scallops);
                }
            }
            else if (itemDecor.ItemType == ItemType.Shrimp && orders[0] == ItemType.Shrimp)
            {
                shrimps.Remove(itemDecor);
                if (shrimps.Count == 0)
                {
                    SetItemDone(ItemType.Shrimp);
                }
            }
        }


    }
}