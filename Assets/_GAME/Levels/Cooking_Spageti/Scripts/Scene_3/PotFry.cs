using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

namespace Link.Cooking.Spageti
{
    public class PotFry : ItemIdleBase
    {
        public enum  ItemCookType { Onion, Tomato, Meat, Noodle, Milk, Butter, Bacon, Mushroom, Pepper, Salt }
        public enum State { Normal, Cooking_1, Cooked_1 , Cooking_2, Cooked_2, Cooking_3, Cooked_3, Cooking_4, Done }
        [SerializeField] State state;

        [SerializeField] Transform oil;
        [SerializeField] ParticleSystem steamVFX, sparkVFX, smokeVFX;
        [SerializeField] Stove stove;
        [SerializeField] bool isFire, isOil, isPepper, isMeat, isTomato, isOnion, isSouce, isSalt;
        [SerializeField] ItemFryControl onionFry, tomatoFry, meatFry, pepperFry, souceFry, saltFry;
        [SerializeField] SortingGroup potSortingGroup;
        [SerializeField] ItemAlpha souce;
        [SerializeField] MeatDecor meatDecor;
        [SerializeField] ParticleSystem oilTrail;
        [SerializeField] float panRadius = 1f;
        [SerializeField] Sprite hint_1;

        [SerializeField] HintText hintText_1, hintText_2, hintText_3, hintText_4, hintText_5, hintText_6;

        int index = 0;
        float cookingRate = 0;
        public override bool IsDone => IsState(State.Done);

        [SerializeField] List<ItemMovingBase> orders;

        void Awake()
        {
            stove.OnStoveFire = SetFire;
        }

        void OnEnable()
        {
            if(hint_1 != null) LevelControl.Ins.SetHint(hint_1);
        }

        public override bool OnTake(IItemMoving item)
        {
            if(!isFire) return false;

            if (item is Pepper && orders[0].Equals(item))
            {
                orders.RemoveAt(0);
                item.OnDone();
                item.OnMove(TF.position + new Vector3(0.5f, 1, 0), Quaternion.identity, 0.2f);
                DOVirtual.DelayedCall(1f, ()=>
                {
                    pepperFry.OnInit(index++);
                    isPepper = true;
                    CheckCooking();
                });
                return true;
            }
            
            if (item is Salt salt && orders[0].Equals(item))
            {
                orders.RemoveAt(0);
                salt.gameObject.SetActive(false);
                DOVirtual.DelayedCall(.2f, ()=>
                {
                    saltFry.OnInit(index++);
                    isSalt = true;
                    CheckCooking();
                });
                return true;
            }
            if (item is Bowl && orders[0].Equals(item))
            {
                orders.RemoveAt(0);
                item.ChangeState(Bowl.State.Pouring);
                item.OnMove(TF.position + new Vector3(0.5f, 1, 0), Quaternion.identity, 0.2f);
                isOil = true;
                oil.gameObject.SetActive(true);
                oil.DOScale(1, 1f).SetDelay(1f).OnComplete(() =>
                {
                    steamVFX.gameObject.SetActive(isFire && isOil);
                    CheckCooking(); 
                    // if(isNoodle) waterStream.DOScale(0.5f, 1f);
                    if (isFire && isOil) SoundControl.Ins.PlayFX(Fx.Fry, true);
                });
                hintText_2.OnActiveHint();
                return true;
            }
            if(item is ItemNeedCook cook && isFire && isOil )
            {
                if(cook.itemCookType == ItemCookType.Onion && orders[0].Equals(item))
                {
                    orders.RemoveAt(0);
                    item.OnDone();
                    onionFry.OnInit(index++);
                    isOnion = true;
                    CheckCooking();
                    return true;
                }
                else if(cook.itemCookType == ItemCookType.Tomato && orders[0].Equals(item))
                {
                    orders.RemoveAt(0);
                    item.OnDone();
                    tomatoFry.OnInit(index++);
                    isTomato = true;
                    CheckCooking();
                    return true;
                }
                else if(cook.itemCookType == ItemCookType.Meat && IsState(State.Cooked_1) && orders[0].Equals(item))
                {
                    orders.RemoveAt(0);
                    item.OnDone();
                    meatFry.OnInit(index++);
                    isMeat = true;
                    ChangeState(State.Cooking_2);
                    return true;
                }
            }
            if (item is SauceJar jar && IsState(State.Cooked_2) && orders[0].Equals(item))
            {
                orders.RemoveAt(0);
                item.OnDone();
                item.OnMove(TF.position + new Vector3(0.5f, 1, 0), Quaternion.identity, 0.2f);
                item.SetOrder(50);
                souce.gameObject.SetActive(true);
                souce.transform.DOScale(1, 1.5f).SetDelay(1f).OnComplete(() =>
                {
                    isSouce = true;
                    ChangeState(State.Cooking_3);
                });
 
                return true;
            }
            if(!(item is Spoon || item is Noodle)) LevelControl.Ins.LoseHalfHeart();
            return false;
        }

        private void SetFire(bool fire)
        {
            isFire = fire;
            steamVFX.gameObject.SetActive(isFire && isOil);
            DOVirtual.DelayedCall(1f, CheckCooking);
            if (fire && IsState(State.Done))
            {
                smokeVFX.Stop();
                smokeVFX.Clear();
            }

            if (fire && isOil) SoundControl.Ins.PlayFX(Fx.Fry, true);
            if (!fire) SoundControl.Ins.StopFX(Fx.Fry);

            if (fire) hintText_1.OnActiveHint();
            if (!fire && IsState(State.Done)) hintText_6.OnActiveHint();
        }
        
        private void CheckCooking()
        {
            if (isFire && isOil && isPepper && isOnion && isTomato && IsState(State.Normal))
            {
                ChangeState(State.Cooking_1);
            }
        }

        public override void ChangeState<T>(T t)
        {
           state = (State)(object)t;
            switch (state)
            {
                case State.Normal:
                    break;
                case State.Cooking_1:
                    oilTrail.gameObject.SetActive(true);
                    stove.SetInteract(false);
                    SetActive(true, true, true, false, false, true, false);
                    break;

                case State.Cooked_1:
                    sparkVFX.Play();
                    smokeVFX.Play();
                    SetActive(false, false, false, false, false, false, true);
                    hintText_3.OnActiveHint();
                    break;

                case State.Cooking_2:
                    smokeVFX.Stop();
                    smokeVFX.Clear();
                    SetActive(true, true, true, true, false, true, false);
                    break;
                case State.Cooked_2:
                    smokeVFX.Play();
                    sparkVFX.Play();
                    SetActive(false, false, false, false, false, false, true);
                    hintText_4.OnActiveHint();
                    break;
                case State.Cooking_3:
                    potSortingGroup.enabled = false;
                    smokeVFX.Stop();
                    smokeVFX.Clear();
                    smokeVFX.Play();
                    SetActive(true, true, true, true, true, true, false);
                    break;
                case State.Cooked_3:
                    //sparkVFX.Play();
                    break;
                case State.Cooking_4:
                    smokeVFX.Stop();
                    smokeVFX.Clear();
                    stove.SetInteract(false);
                    break;
                case State.Done:
                    hintText_5.OnActiveHint();
                    sparkVFX.Play();
                    smokeVFX.Play();
                    SetActive(false, false, false, false, false, false, true);
                    stove.SetOnlyTurnOff();
                    stove.SetInteract(true);
                    meatDecor.SetScale();
                    OnDone();
                    oilTrail.gameObject.SetActive(false);
                    break;
            }
        }
        public override bool IsState<T>(T t)
        {
           return state == (State)(object)t;
        }

        private void SetActive(bool isPepper, bool isOnion, bool isTomato, bool isMeat, bool isSauce, bool isSalt, bool isPotGroup)
        {
            pepperFry.SetControl(isPepper);
            onionFry.SetControl(isOnion);
            tomatoFry.SetControl(isTomato);
            meatFry.SetControl(isMeat);
            souceFry.SetControl(isSauce);
            if(saltFry != null) saltFry.SetControl(isSalt);
            potSortingGroup.enabled = isPotGroup;
        }

        public void SetCookingRate(Vector3 point)
        {
            if (oilTrail != null)
            {
                if (Vector2.Distance(point, TF.position) <= panRadius)
                {
                    oilTrail.transform.position = point;
                }
            }

            if(IsState(State.Cooking_1, State.Cooking_2, State.Cooking_3, State.Cooking_4))
            {
                cookingRate += Time.deltaTime * 0.2f;
            }
            if(IsState(State.Cooking_1))
            {
                //-> 1
                onionFry.SetRipeRate(cookingRate);
                tomatoFry.SetRipeRate(cookingRate);
                if(cookingRate >= 1)
                {
                    ChangeState(State.Cooked_1);
                }
            }
            if (IsState(State.Cooking_2))
            {
                //-> 2
                meatFry.SetRipeRate(cookingRate - 1);
                if (cookingRate >= 2)
                {
                    ChangeState(State.Cooked_2);
                }
            }
            if(IsState(State.Cooking_3))
            {
                //-> 3
                meatFry.SetRipeRate(cookingRate - 1);
                float rate = (4 - cookingRate) * 0.5f;
                souce.SetAlpha(rate);
                onionFry.SetAlpha(rate);
                tomatoFry.SetAlpha(rate);
                pepperFry.SetAlpha(rate);
                if(saltFry != null) saltFry.SetAlpha(rate);
                if (cookingRate >= 3)
                {
                    ChangeState(State.Cooked_3);
                    ChangeState(State.Cooking_4);
                }
            }if (IsState(State.Cooking_4))
            {
                //Debug.Log("Cooking 4 " + Utilities.GetValue(cookingRate, 3f, 4f, 0.8f, 0.45f));
                //-> 4
                meatFry.SetAlpha(4 - cookingRate);
                float rate = Utilities.GetMapValue(cookingRate, 3f, 4f, 0.5f, 0);
                souce.SetAlpha(rate);
                onionFry.SetAlpha(rate);
                tomatoFry.SetAlpha(rate);
                pepperFry.SetAlpha(rate);
                if(saltFry != null) saltFry.SetAlpha(rate);
                meatFry.SetRadius(Utilities.GetMapValue(cookingRate, 3f, 4f, 0.8f, 0.6f));
                //add thit
                meatDecor.SetScaleLight();
                meatDecor.SetAlpha(cookingRate - 3);
                if (cookingRate >= 4)
                {
                    ChangeState(State.Done);
                }
            }
        }

        void OnDestroy()
        {
            SoundControl.Ins.StopFX(Fx.Fry);
        }

        
    }
}
