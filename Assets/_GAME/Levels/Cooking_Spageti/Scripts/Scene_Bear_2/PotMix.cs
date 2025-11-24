using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Link.Cooking.Spageti;
using UnityEngine;
using UnityEngine.Rendering;

namespace Link.Cooking.Spageti_Bear
{
    public class PotMix : ItemMovingBase, IMixEllipse2D
    {
        private const int INDEX_SPACE = 5;
        public enum State { Empty, Cooking_1, Cooked_1, Cooking_2, Cooked_2, Cooking_3, Done, MoveDish, Finish }
        [SerializeField] State state;
        [SerializeField] EllipseControl2D[] elipses;
        [SerializeField] ParticleSystem splashVFX, steamVFX, smokeVFX;
        [SerializeField] Stove stove;
        [SerializeField] Spoon spoon;
        [SerializeField] SpriteRenderer milkTopSpr, milkBottomSpr;

        public override bool IsCanMove => base.IsCanMove && IsState(State.MoveDish);

        float time = 0;
        bool isPepper, isFire, isButter, isOnion, isBacon, isNoodle, isMushroom, isMilk;
        bool isButterOil;

        [SerializeField] SortingGroup butterGroup, milkGroup, onionGroup, mushroomGroup, baconGroup, pepperGroup, butterOilGroup, noodleGroup;
        [SerializeField] AnimaBase2D pepperDrop, butterDrop;
        [SerializeField] SpriteRenderer butterSpr, noodleSpr;
        [SerializeField] ItemAlpha itemAlpha;
        public Noodle noodleDone;

        [SerializeField] ItemsChangeAlpha2D mushroomAlpha, baconAlpha;
        List<PotFry.ItemCookType> indexs = new List<PotFry.ItemCookType>();
        [SerializeField] HintText hintText_1, hintText_2, hintText_3, hintText_4, hintText_5, hintText_6;
        Noodle noodle;
        public override bool IsDone => IsState(State.Finish); 
        
        void Awake()
        {
            stove.OnStoveFire = SetFire;

            foreach (var i in elipses)
            {
                i.OnInit();
            }
        }

        public override void ChangeState<T>(T t)
        {
            state = (State)(object)t;
            if (IsState(State.Finish))
            {
                smokeVFX.Stop();
                smokeVFX.Clear();
            }
        }

        public override bool IsState<T>(T t)
        {
            return state == (State)(object)t;
        }

        public override void OnClickDown()
        {
            base.OnClickDown();
            SoundControl.Ins.PlayFX(Fx.Click);
        }

        public override void OnDrop()
        {
            base.OnDrop();
            OnBack();
        }

        public override bool OnTake(IItemMoving item)
        {
            if(!isFire && !((item is Scoop s) || item is Spoon))
            {
                LevelControl.Ins.LoseHalfHeart(TF.position);
                return false;
            }

            if (item is Pepper)
            {
                item.OnDone();
                item.OnMove(TF.position + new Vector3(0.5f, 1, 0), Quaternion.identity, 0.2f);
                pepperGroup.sortingOrder = indexs.Count + INDEX_SPACE;
                DOVirtual.DelayedCall(1f, () =>
                {
                    isPepper = true;
                    indexs.Add(PotFry.ItemCookType.Pepper);
                    pepperGroup.gameObject.SetActive(true);
                    pepperDrop.OnActive();
                    CheckCooking();
                });
                return true;
            }
            if (item is SauceJar milk && IsState(State.Cooked_1))
            {
                item.OnDone();
                item.OnMove(TF.position + new Vector3(0.5f, 1, 0), Quaternion.identity, 0.2f);
                item.SetOrder(50);
                milkGroup.gameObject.SetActive(true);
                indexs.Add(PotFry.ItemCookType.Milk);
                milkGroup.sortingOrder = indexs.Count + INDEX_SPACE;
                milkGroup.transform.DOScale(1, 1.5f).SetDelay(1f).OnComplete(() =>
                {
                    isMilk = true;
                    CheckCooking();
                });
                return true;
            }
            if (item is ItemNeedCook butter && butter.itemCookType == PotFry.ItemCookType.Butter)
            {
                item.OnDone();
                item.OnMove(TF.position + new Vector3(0, 0.3f, 0), Quaternion.identity, 0.2f);
                indexs.Add(PotFry.ItemCookType.Butter);
                butterGroup.sortingOrder = indexs.Count + INDEX_SPACE;
                butterGroup.gameObject.SetActive(true);

                butterDrop.gameObject.SetActive(true);
                // butterOilGroup.sortingOrder = indexs.Count;
                butterDrop.OnActive();
                if (isFire)
                {
                    isButterOil = true;
                    butterSpr.DOFade(0, 1.5f).SetDelay(1f);
                    butterOilGroup.transform.DOScale(1, 1.5f).SetDelay(1f).OnComplete(() =>
                        {
                            isButter = true;
                            onionGroup.gameObject.SetActive(true);
                            // mushroomGroup.gameObject.SetActive(true);
                            // baconGroup.gameObject.SetActive(true);

                            CheckCooking();

                            if (isFire && isButter) steamVFX.gameObject.SetActive(true);
                            if (isFire && isButter) SoundControl.Ins.PlayFX(Fx.Fry, true);

                        });
                }
                hintText_2.OnActiveHint();
                return true;
            }
            if (item is Scoop scoop && scoop.IsNoodle && IsState(State.Cooked_2))
            {
                //tha mi ra
                noodle = scoop.noodle;
                noodle.TF.SetParent(TF);
                noodle.TF.DOScale(Vector3.one * 1.1f, 0.2f);
                noodle.TF.DOLocalMove(Vector3.up * 0.3f, 0.2f);
                noodle.OrderLayer = 50;

                DOVirtual.DelayedCall(0.2f, () =>
                {
                    noodleGroup.gameObject.SetActive(true);
                    isNoodle = true;
                    CheckCooking();
                });
            }

            if (!((item is Scoop) || item is Spoon || (item is ItemNeedCook cook && !cook.gameObject.activeSelf))) LevelControl.Ins.LoseHalfHeart(TF.position);

            return base.OnTake(item);
        }

        private void CheckCooking()
        {
            if (IsState(State.Empty) && isFire && isPepper && isButter && isOnion && isBacon && isMushroom)
            {
                ChangeState(State.Cooking_1);
                stove.SetInteract(false);
                spoon.TF.SetParent(TF);
            }
            if (IsState(State.Cooked_1) && isMilk)
            {
                ChangeState(State.Cooking_2);
                spoon.TF.SetParent(TF);
            }
            if (IsState(State.Cooked_2) && isNoodle)
            {
                ChangeState(State.Cooking_3);
                splashVFX.Play();
                SoundControl.Ins.PlayFX(Fx.Blink);
                spoon.TF.SetParent(TF);
            }
        }

        public void SetTime()
        {
            switch (state)
            {
                case State.Cooking_1:
                case State.Cooking_2:
                case State.Cooking_3:
                    time += Time.deltaTime;
                    break;
                case State.Done:
                    break;
                default:
                    break;
            }

            if (time >= 2 && IsState(State.Cooking_1))
            {
                ChangeState(State.Cooked_1);
                spoon.TF.SetParent(TF.parent);
                splashVFX.Play();
                SoundControl.Ins.PlayFX(Fx.Blink);
                hintText_3.OnActiveHint();
            }

            if (time >= 3.5f && time <= 7f)
            {
                float alphaMilk = Mathf.Clamp(time, 3.5f, 7f);
                SetMilkAlpha(Utilities.GetMapValue(alphaMilk, 3.5f, 7f, 1, 0));
            }

            if (time >= 7 && IsState(State.Cooking_2))
            {
                ChangeState(State.Cooked_2);
                spoon.TF.SetParent(TF.parent);
                splashVFX.Play();
                SoundControl.Ins.PlayFX(Fx.Blink);
                smokeVFX.Play();
                butterOilGroup.gameObject.SetActive(false);
                hintText_4.OnActiveHint();
            }
            if (time >= 14 && IsState(State.Cooking_3))
            {
                ChangeState(State.Done);
                spoon.TF.SetParent(TF.parent);
                splashVFX.Play();
                SoundControl.Ins.PlayFX(Fx.Blink);
                stove.SetInteract(true);
                stove.SetOnlyTurnOff();
                noodle.gameObject.SetActive(false);
                hintText_5.OnActiveHint();
            }

            foreach (var i in elipses)
            {
                i.SetTime(time).SetRotate(time);
            }

            milkGroup.transform.eulerAngles = Vector3.forward * time * -360f;
            if (noodle != null && time >= 7 && time <= 10)
            {
                noodle.TF.eulerAngles = Vector3.forward * time * -360f;
                float alpha = Mathf.Clamp(time, 7f, 10f);
                SetNoodleAlpha(Utilities.GetMapValue(alpha, 7f, 10f, 1, 0));
            }

            if (time >= 7 && time <= 14)
            {
                noodleGroup.transform.eulerAngles = Vector3.forward * time * -360f;
            }

            if (time >= 11 && time <= 14)
            {
                float alpha = Mathf.Clamp(time, 11f, 14f);
                SetNoodleDoneAlpha(Utilities.GetMapValue(alpha, 7f, 14f, 1, 0));
                noodleDone.TF.eulerAngles = Vector3.forward * time * -360f;
            }

            if (time <= 2f)
            {
                mushroomAlpha.SetAlpha(Utilities.GetMapValue(time, 0, 2f, 0, 1));
                baconAlpha.SetAlpha(Utilities.GetMapValue(time, 0, 2f, 0, 1));
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent(out ItemNeedCook itemNeedCook))
            {
                PotFry.ItemCookType type = itemNeedCook.itemCookType;
                if (type == PotFry.ItemCookType.Onion && !indexs.Contains(type) && onionGroup.gameObject.activeSelf)
                {
                    indexs.Add(type);
                    onionGroup.sortingOrder = indexs.Count + INDEX_SPACE;
                }
                if (type == PotFry.ItemCookType.Mushroom && !indexs.Contains(type) && mushroomGroup.gameObject.activeSelf)
                {
                    indexs.Add(type);
                    mushroomGroup.sortingOrder = indexs.Count + INDEX_SPACE;
                }
                if (type == PotFry.ItemCookType.Bacon && !indexs.Contains(type) && baconGroup.gameObject.activeSelf)
                {
                    indexs.Add(type);
                    baconGroup.sortingOrder = indexs.Count + INDEX_SPACE;
                }
            }
        }

        private void SetFire(bool fire)
        {
            isFire = fire;
            steamVFX.gameObject.SetActive(isFire && isButter);
            DOVirtual.DelayedCall(1f, CheckCooking);
            if (fire && IsState(State.Done))
            {
                smokeVFX.Stop();
                smokeVFX.Clear();
            }

            if (fire && isButter) SoundControl.Ins.PlayFX(Fx.Fry, true);
            if (!fire) SoundControl.Ins.StopFX(Fx.Fry);

            if (fire && !isButterOil && butterDrop.gameObject.activeSelf)
            {
                isButterOil = true;
                butterSpr.DOFade(0, 1.5f).SetDelay(1f);
                butterOilGroup.transform.DOScale(1, 1.5f).SetDelay(1f).OnComplete(() =>
                    {
                        isButter = true;
                        onionGroup.gameObject.SetActive(true);

                        CheckCooking();
                        if (isFire && isButter) steamVFX.gameObject.SetActive(true);
                        if (fire && isButter) SoundControl.Ins.PlayFX(Fx.Fry, true);
                    });
            }

            if(isFire) hintText_1.OnActiveHint();
            if(!isFire && IsState(State.Done)) hintText_6.OnActiveHint();
        }

        public void OnionDone()
        {
            isOnion = true;
            mushroomGroup.gameObject.SetActive(true);
            CheckCooking();
        }

        public void MushroomDone()
        {
            isMushroom = true;
            baconGroup.gameObject.SetActive(true);
            CheckCooking();
        }
        public void BaconDone()
        {
            isBacon = true;
            CheckCooking();
        }

        private void SetMilkAlpha(float alpha)
        {
            // 1 -> 0
            Color c = Color.white;
            c.a = alpha;
            milkTopSpr.color = c;
            c.a = 1 - alpha;
            milkBottomSpr.color = c;
        }
        private void SetNoodleAlpha(float alpha)
        {
            // 1 -> 0
            noodle.SetAlpha(alpha);
            Color c = Color.white;
            c.a = 1 - alpha;
            noodleSpr.color = c;
        }

        private void SetNoodleDoneAlpha(float alpha)
        {
            // 1 -> 0
            itemAlpha.SetAlpha(alpha);
            noodleDone.SetAlpha(1 - alpha);
        }
    }
}