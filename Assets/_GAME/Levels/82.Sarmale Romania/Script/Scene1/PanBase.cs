using DG.Tweening;
using Link;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

namespace HuyThanh.Cooking.SarmaleRomania
{
    public class PanBase : ItemMovingBase
    {
        public enum State
        {
            WaitForTurnOn,
            WaitingOil,
            WaitingOnion,
            Mixing1,
            WaitingRice,
            WaitingTomatoSauce,
            WaitingChili,
            Mixing2,
            WaitingBeef,
            WaitingPork,
            Mixing3,
            WaitingDill,
            WaitingPepper,
            WaitingSalt,
            Mixing4,
            WaitForTurnOff,
            DoneMixing,
            Pouring,
            Done
        }

        [SerializeField] State state;
        [FoldoutGroup("VFX")][SerializeField] ParticleSystem blinkVFX;

        [FoldoutGroup("Oil Pouring")][SerializeField] Transform squareOil;
        [FoldoutGroup("Oil Pouring")][SerializeField] ItemAlpha squareOilAlpha;

        [FoldoutGroup("Onion Pouring")][SerializeField] Drop2D onionDrop2D;
        [FoldoutGroup("Onion Fry Control")][SerializeField] SpiceItemFryControl onionFryControl;

        [FoldoutGroup("Rice Pouring")]
        [SerializeField] ItemAlpha squareRiceAlpha;

        [FoldoutGroup("Rice Fry Control")]
        [SerializeField] SpiceItemFryRound riceFryControl;

        [FoldoutGroup("Tomato Sauce")]
        [SerializeField] ItemAlpha squareTomatoSauceAlpha;

        [FoldoutGroup("Tomato Sauce Control")]
        [SerializeField] SpiceItemFryRound tomatoSauceFryControl;

        [FoldoutGroup("Chili Pouring")]
        [SerializeField] Drop2D chillDrop2D;

        [FoldoutGroup("Chili Fry Control")]
        [SerializeField] SpiceItemFryControl chiliFryControl;

        [FoldoutGroup("Mixing2 Fry Control")][SerializeField] SpiceItemFryRound mixing2FryControl;
        [FoldoutGroup("Mixing2 Done Fry Control")][SerializeField] SpiceItemFryRound mixing2DoneFryControl;

        [FoldoutGroup("Beef Pouring")]
        [SerializeField] Drop2D beefDrop2D;
        [FoldoutGroup("Beef Pouring")]
        [SerializeField] SpiceItemFryControl beefFryControl;

        [FoldoutGroup("Pork Pouring")]
        [SerializeField] Drop2D porkDrop2D;
        [FoldoutGroup("Pork Pouring")]
        [SerializeField] SpiceItemFryControl porkFryControl;

        [FoldoutGroup("Mixing3 Fry Control")][SerializeField] SpiceItemFryRound mixing3FryControl, mixing3BaseFryControl, mixing3DoneFryControl;

        [FoldoutGroup("Dill Pouring")][SerializeField] ItemAlpha squareDillAlpha;
        [FoldoutGroup("Dill Pouring")][SerializeField] SpiceItemFryRound dillFryControl;

        [FoldoutGroup("Pepper Pouring")][SerializeField] Drop2D pepperDrop2D;
        [FoldoutGroup("Pepper Pouring")][SerializeField] SpiceItemFryControl pepperFryControl;

        [FoldoutGroup("Salt Pouring")][SerializeField] Drop2D saltDrop2D;
        [FoldoutGroup("Salt Pouring")][SerializeField] SpiceItemFryControl saltFryControl;

        [FoldoutGroup("Done Mixing")]
        [SerializeField] SpiceItemFryRound doneMixingFryControl;

        [SerializeField] BoxCollider2D boxCollider2D;
        [SerializeField] Animation anim;
        [SerializeField] string animPouringName;

        private float cookingRate = 0;

        public override bool IsCanMove => IsState(State.DoneMixing);

        public override bool IsState<T>(T t)
        {
            return state == (State)(object)t;
        }

        public override void ChangeState<T>(T t)
        {
            state = (State)(object)t;

            switch (state)
            {
                case State.WaitingOil:
                    break;
                case State.WaitingOnion:
                    break;
                case State.DoneMixing:
                    boxCollider2D.enabled = true;
                    break;
                case State.Pouring:
                    BasePouring();
                    break;
                case State.Done:
                    boxCollider2D.enabled = false;
                    OnBack();
                    break;
            }
        }

        public override bool OnTake(IItemMoving item)
        {
            if (item is ItemPouring oil && oil.IsPouringType(ItemPouring.PouringType.Oil) && item.IsState(ItemPouring.State.Normal) && IsState(State.WaitingOil))
            {
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                item.ChangeState(ItemPouring.State.Pouring);
                OilPouring();
                return true;
            }

            if (item is ItemPouring onion && onion.IsPouringType(ItemPouring.PouringType.Onion) && item.IsState(ItemPouring.State.Normal) && IsState(State.WaitingOnion))
            {
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                item.ChangeState(ItemPouring.State.Pouring);
                OnionPouring();
                return true;
            }

            if (item is ItemPouring rice && rice.IsPouringType(ItemPouring.PouringType.Rice) && item.IsState(ItemPouring.State.Normal) && IsState(State.WaitingRice))
            {
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                item.ChangeState(ItemPouring.State.Pouring);
                RicePouring();
                return true;
            }

            if (item is ItemPouring tomatoSauce && tomatoSauce.IsPouringType(ItemPouring.PouringType.TomatoSauce) && item.IsState(ItemPouring.State.Normal) && IsState(State.WaitingTomatoSauce))
            {
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                item.ChangeState(ItemPouring.State.Pouring);
                TomatoSaucePouring();
                return true;
            }

            if (item is ItemPouring chili && chili.IsPouringType(ItemPouring.PouringType.Chili) && item.IsState(ItemPouring.State.Normal) && IsState(State.WaitingChili))
            {
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                item.ChangeState(ItemPouring.State.Pouring);
                ChiliPouring();
                return true;
            }

            if (item is ItemPouring beef && beef.IsPouringType(ItemPouring.PouringType.Beef) && item.IsState(ItemPouring.State.Normal) && IsState(State.WaitingBeef))
            {
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                item.ChangeState(ItemPouring.State.Pouring);
                BeefPouring();
                return true;
            }

            if (item is ItemPouring pork && pork.IsPouringType(ItemPouring.PouringType.Pork) && item.IsState(ItemPouring.State.Normal) && IsState(State.WaitingPork))
            {
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                item.ChangeState(ItemPouring.State.Pouring);
                PorkPouring();
                return true;
            }

            if (item is ItemPouring dill && dill.IsPouringType(ItemPouring.PouringType.Dill) && item.IsState(ItemPouring.State.Normal) && IsState(State.WaitingDill))
            {
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                item.ChangeState(ItemPouring.State.Pouring);
                DillPouring();
                return true;
            }

            if (item is ItemPouring pepper && pepper.IsPouringType(ItemPouring.PouringType.Pepper) && item.IsState(ItemPouring.State.Normal) && IsState(State.WaitingPepper))
            {
                item.OnMove(TF.position + Vector3.up * 0.5f, Quaternion.identity, 0.2f);
                item.ChangeState(ItemPouring.State.Pouring);
                PepperPouring();
                return true;
            }

            if (item is ItemPouring salt && salt.IsPouringType(ItemPouring.PouringType.Salt) && item.IsState(ItemPouring.State.Normal) && IsState(State.WaitingSalt))
            {
                item.OnMove(TF.position + Vector3.up * 0.5f, Quaternion.identity, 0.2f);
                item.ChangeState(ItemPouring.State.Pouring);
                SaltPouring();
                return true;
            }

            return base.OnTake(item);
        }

        public void SetCookingRate(Vector3 position)
        {
            if (IsState(State.Mixing1)) ///cookingrate from 0 to 2
            {
                cookingRate += Time.deltaTime * 0.5f;
                onionFryControl.SetRipeRate(cookingRate);
                if (cookingRate >= 2f)
                {
                    blinkVFX.Play();
                    ChangeState(State.WaitingRice);
                }
            }

            if (IsState(State.Mixing2))
            {
                cookingRate += Time.deltaTime * 0.5f;
                if (cookingRate >= 2f && cookingRate < 3f)
                {
                    onionFryControl.SetRipeRate(cookingRate);
                    riceFryControl.SetRipeRate(cookingRate - 2f);
                    tomatoSauceFryControl.SetRipeRate(cookingRate - 2f);
                    chiliFryControl.SetRipeRate(cookingRate);
                    mixing2FryControl.SetRipeRate(3 - cookingRate);
                }

                if (cookingRate >= 3f && cookingRate < 4f)
                {
                    onionFryControl.SetAlpha(4 - cookingRate);
                    chiliFryControl.SetAlpha(4 - cookingRate);
                    mixing2FryControl.SetRipeRate(cookingRate - 3f);
                    mixing2DoneFryControl.SetRipeRate(4 - cookingRate);
                }

                if (cookingRate >= 4f)
                {
                    blinkVFX.Play();
                    squareOilAlpha.SetAlpha(0);
                    ChangeState(State.WaitingBeef);
                }
            }

            if (IsState(State.Mixing3))
            {
                cookingRate += Time.deltaTime * 0.5f;
                if (cookingRate >= 4f && cookingRate < 5f)
                {
                    beefFryControl.SetAlpha(5 - cookingRate);
                    porkFryControl.SetAlpha(5 - cookingRate);
                    mixing2DoneFryControl.SetRipeRate(cookingRate - 4f);
                    mixing3FryControl.SetRipeRate(5 - cookingRate);
                }

                if (cookingRate >= 5f && cookingRate < 6f)
                {
                    mixing3FryControl.SetRipeRate(cookingRate - 5f);
                    mixing3BaseFryControl.SetRipeRate(6 - cookingRate);
                }

                if (cookingRate >= 6f && cookingRate < 7f)
                {
                    mixing3BaseFryControl.SetRipeRate(cookingRate - 6f);
                    mixing3DoneFryControl.SetRipeRate(7 - cookingRate);
                }

                if (cookingRate >= 7f)
                {
                    blinkVFX.Play();
                    ChangeState(State.WaitingDill);
                }
            }

            if (IsState(State.Mixing4))
            {
                cookingRate += Time.deltaTime * 0.5f;
                if (cookingRate >= 7f && cookingRate < 8f)
                {
                    dillFryControl.SetRipeRate(cookingRate - 7f);
                    pepperFryControl.SetAlpha(8 - cookingRate);
                    saltFryControl.SetAlpha(8 - cookingRate);
                    mixing3DoneFryControl.SetRipeRate(cookingRate - 7f);
                    doneMixingFryControl.SetRipeRate(8 - cookingRate);
                }

                if (cookingRate >= 8f)
                {
                    blinkVFX.Play();
                    ChangeState(State.WaitForTurnOff);
                }
            }
        }

        public bool CanUseButton()
        {
            return IsState(State.WaitForTurnOff, State.WaitForTurnOn);
        }

        public void OnButtonClick(bool isOn)
        {
            if (IsState(State.WaitForTurnOn))
            {
                ChangeState(State.WaitingOil);
            }
            if (IsState(State.WaitForTurnOff))
            {
                ChangeState(State.DoneMixing);
            }
        }

        [Button("Oil Pouring")]
        public void OilPouring()
        {
            StartCoroutine(WaitForOilPouring());
        }
        IEnumerator WaitForOilPouring()
        {
            squareOil.localScale = Vector3.zero;
            yield return WaitForSecondCache.Get(0.5f);
            squareOil.DOScale(1, 0.5f);
            squareOilAlpha.DoAlpha(0.6f, 0.5f);
            yield return WaitForSecondCache.Get(0.5f);
            ChangeState(State.WaitingOnion);
        }

        [Button("Onion Pouring")]
        public void OnionPouring()
        {
            StartCoroutine(WaitForOnionPouring());
        }

        IEnumerator WaitForOnionPouring()
        {
            yield return WaitForSecondCache.Get(0.5f);
            onionDrop2D.OnActive();
            yield return WaitForSecondCache.Get(0.5f);
            ChangeState(State.Mixing1);
        }

        [Button("Rice Pouring")]
        public void RicePouring()
        {
            StartCoroutine(WaitForRicePouring());
        }

        IEnumerator WaitForRicePouring()
        {
            yield return WaitForSecondCache.Get(0.5f);
            squareRiceAlpha.DoAlpha(1, 0.5f);
            yield return WaitForSecondCache.Get(0.5f);
            ChangeState(State.WaitingTomatoSauce);
        }

        [Button("Tomato Sauce Pouring")]
        public void TomatoSaucePouring()
        {
            StartCoroutine(WaitForTomatoSaucePouring());
        }

        IEnumerator WaitForTomatoSaucePouring()
        {
            squareTomatoSauceAlpha.transform.localScale = Vector3.zero;
            yield return WaitForSecondCache.Get(0.5f);
            squareTomatoSauceAlpha.transform.DOScale(1, 0.5f);
            squareTomatoSauceAlpha.DoAlpha(1, 0.5f);
            yield return WaitForSecondCache.Get(0.5f);
            ChangeState(State.WaitingChili);
        }

        [Button("Chili Pouring")]
        public void ChiliPouring()
        {
            StartCoroutine(WaitForChiliPouring());
        }

        IEnumerator WaitForChiliPouring()
        {
            yield return WaitForSecondCache.Get(0.5f);
            chillDrop2D.OnActive();
            yield return WaitForSecondCache.Get(0.5f);
            ChangeState(State.Mixing2);
        }

        [Button("Beef Pouring")]
        public void BeefPouring()
        {
            StartCoroutine(WaitForBeefPouring());
        }

        IEnumerator WaitForBeefPouring()
        {
            yield return WaitForSecondCache.Get(0.5f);
            beefDrop2D.OnActive();
            yield return WaitForSecondCache.Get(0.5f);
            ChangeState(State.WaitingPork);
        }

        [Button("Pork Pouring")]
        public void PorkPouring()
        {
            StartCoroutine(WaitForPorkPouring());
        }

        IEnumerator WaitForPorkPouring()
        {
            yield return WaitForSecondCache.Get(0.5f);
            porkDrop2D.OnActive();
            yield return WaitForSecondCache.Get(0.5f);
            ChangeState(State.Mixing3);
        }

        [Button("Dill Pouring")]
        public void DillPouring()
        {
            StartCoroutine(WaitForDillPouring());
        }

        IEnumerator WaitForDillPouring()
        {
            squareDillAlpha.transform.localScale = Vector3.zero;
            yield return WaitForSecondCache.Get(0.5f);
            squareDillAlpha.DoAlpha(1, 0.5f);
            squareDillAlpha.transform.DOScale(1, 0.5f);
            yield return WaitForSecondCache.Get(0.5f);
            ChangeState(State.WaitingPepper);
        }

        [Button("Pepper Pouring")]
        public void PepperPouring()
        {
            StartCoroutine(WaitForPepperPouring());
        }

        IEnumerator WaitForPepperPouring()
        {
            yield return WaitForSecondCache.Get(0.5f);
            pepperDrop2D.OnActive();
            yield return WaitForSecondCache.Get(0.5f);
            ChangeState(State.WaitingSalt);
        }

        [Button("Salt Pouring")]
        public void SaltPouring()
        {
            StartCoroutine(WaitForSaltPouring());
        }

        IEnumerator WaitForSaltPouring()
        {
            yield return WaitForSecondCache.Get(0.5f);
            saltDrop2D.OnActive();
            yield return WaitForSecondCache.Get(0.5f);
            ChangeState(State.Mixing4);
        }

        [Button("Base Pouring")]

        public void BasePouring()
        {
            anim.Play(animPouringName);
            StartCoroutine(WaitForBasePouring());
        }

        IEnumerator WaitForBasePouring()
        {
            yield return WaitForSecondCache.Get(1.5f);
            ChangeState(State.Done);
        }
    }

}
