using DG.Tweening;
using Link;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoangLinh.Cooking.Test
{
    public class Pan : ItemIdleBase
    {
        public enum State
        {
            Empty,
            Heated,
            HaveOil,
            HaveOnion,
            HaveGarlic,
            FirstMix,
            Meat,
            Pepper,
            Chilli,
            Salt,
            Tumeric,
            TomatoSauce,
            Oregano,
            SecondMix,
            DoneMix,
            WaitTurnOff,
            Done
        }

        [SerializeField] State state;

        [SerializeField] Animation anim;
        [SerializeField] string animOil, animMeat, animTomato;

        [Header("ItemAlpha")]
        [SerializeField] ItemAlpha tomatoSauceAlpha;
        [SerializeField] ItemAlpha rawMeatAlpha;
        [SerializeField] ItemAlpha mixMeatAlpha;
        [SerializeField] ItemAlpha cookedMeatAlpha;

        [Header("Drop2D")]
        [SerializeField] Drop2D onionDrop2D;
        [SerializeField] Drop2D garlicDrop2D;

        [Header("FryCtrl")]
        [SerializeField] SpiceItemFryCtrl onionFryCtrl;
        [SerializeField] SpiceItemFryCtrl garlicFryCtrl;

        [Header("VFX")]
        [SerializeField] ParticleSystem oilVFX;

        float cookingRate = 0;

        public override bool IsState<T>(T t)
        {
            return state == (State)(object)t;
        }

        public override void ChangeState<T>(T t)
        {
            state = (State)(object)t;

            switch (state)
            {
                case State.Empty:
                    break;
                case State.HaveOil:
                    anim.Play(animOil);
                    DOVirtual.DelayedCall(2f, () =>
                    {
                        oilVFX.Play();
                    });
                    break;
                case State.HaveOnion:
                    onionDrop2D.OnActive();
                    break;
                case State.HaveGarlic:
                    garlicDrop2D.OnActive();
                    ChangeState(State.FirstMix);
                    break;
                case State.FirstMix:
                    onionFryCtrl.SetControl(true);
                    garlicFryCtrl.SetControl(true);
                    break;
                case State.SecondMix:
                    onionFryCtrl.SetControl(true);
                    garlicFryCtrl.SetControl(true);
                    break;
                case State.Done:
                    oilVFX.Stop();
                    break;
            }
        }

        public override bool OnTake(IItemMoving item)
        {
            if (IsState(State.Heated) && item is Oil && item.IsState(Oil.State.Normal))
            {
                item.ChangeState(Oil.State.Pouring);
                this.ChangeState(State.HaveOil);
                return true;
            }

            if (item is SpiceItems && item.IsState(SpiceItems.State.Normal))
            {
                return TakeSpiceItem(item as SpiceItems);
            }
            return base.OnTake(item);
        }

        public bool TakeSpiceItem(SpiceItems spiceItem)
        {
            if (IsState(State.HaveOil) && spiceItem.IsSpiceType(SpiceType.Onion))
            {
                spiceItem.ChangeState(SpiceItems.State.Pouring);
                StartCoroutine(WaitToPourOnion());
                return true;
            }

            if (IsState(State.HaveOnion) && spiceItem.IsSpiceType(SpiceType.Garlic))
            {
                spiceItem.ChangeState(SpiceItems.State.Pouring);
                StartCoroutine(WaitToPourGarlic());
                return true;
            }


            return false;
        }

        public void OnClickButton(bool isOn)
        {
            if (isOn == true && IsState(Pan.State.Empty))
            {
                ChangeState(Pan.State.Heated);
            }

            if (isOn == false && IsState(Pan.State.WaitTurnOff))
            {
                ChangeState(Pan.State.Done);

            }
        }

        public void SetCookingRate(Vector3 point)
        {
            if (IsState(State.FirstMix) || IsState(State.SecondMix))
            {
                cookingRate += Time.deltaTime * 0.5f;
            }

            if (IsState(State.FirstMix))
            {
                onionFryCtrl.SetRipeRate(cookingRate);
                garlicFryCtrl.SetRipeRate(cookingRate);
            }

            if (cookingRate >= 1)
            {
                ChangeState(State.Meat);
            }

            if (IsState(State.SecondMix))
            {
                onionFryCtrl.SetRipeRate(cookingRate);
                garlicFryCtrl.SetRipeRate(cookingRate);
            }
        }

        #region Delay Actions To Change Anims
        IEnumerator WaitToPourOnion()
        {
            yield return new WaitForSeconds(1f);
            ChangeState(State.HaveOnion);
        }

        IEnumerator WaitToPourGarlic()
        {
            yield return new WaitForSeconds(1f);
            ChangeState(State.HaveGarlic);
        }

        IEnumerator WaitToPourMeat()
        {
            yield return new WaitForSeconds(1f);
            ChangeState(State.Meat);
        }

        IEnumerator WaitToPourSalt()
        {
            yield return new WaitForSeconds(1f);
            ChangeState(State.Salt);
        }

        IEnumerator WaitToPourPepper()
        {
            yield return new WaitForSeconds(1f);
            ChangeState(State.Pepper);
        }

        IEnumerator WaitToPourChilli()
        {
            yield return new WaitForSeconds(1f);
            ChangeState(State.Chilli);
        }

        IEnumerator WaitToPourTumeric()
        {
            yield return new WaitForSeconds(1f);
            ChangeState(State.Tumeric);
        }

        IEnumerator WaitToPourSauce()
        {
            yield return new WaitForSeconds(1f);
            ChangeState(State.TomatoSauce);
        }

        IEnumerator WaitToPourOregano()
        {
            yield return new WaitForSeconds(1f);
            ChangeState(State.Oregano);
        }


        #endregion

    }
}

