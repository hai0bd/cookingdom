using Hai.Cooking.Burrito;
using Link;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using HuyThanh.Cooking.Burrito;

namespace Hai.Cooking.Burrito
{
    public class Pan : ItemIdleBase
    {
        public enum State
        {
            Normal,
            HeatOn,
            HaveOil,
            HaveOnion,
            HaveGarlic,
            Mixing1,
            Meat,
            Chili,
            Salt,
            Turmeric,
            Peper,
            Tomato,
            Oregano,
            Mixing2,
            DoneMix2,
            WaitForTurnOff,
            Done
        }

        [SerializeField] private State state;
        [SerializeField] private Animation anim;
        [SerializeField] private string animOil, animMeat, animTomato;

        [SerializeField] private Drop2D onionDrop2D;
        [SerializeField] private Drop2D garlicDrop2D;
        [SerializeField] private Drop2D chiliDrop2D;
        [SerializeField] private Drop2D peperDrop2D;
        [SerializeField] private Drop2D tomatoDrop2D;
        [SerializeField] private Drop2D turmericDrop2D;
        [SerializeField] private Drop2D oreganoDrop2D;

        [SerializeField] private ItemAlpha rawMeatAlpha;
        [SerializeField] private ItemAlpha tomatoSauceAlpha;
        [SerializeField] private ItemAlpha mixMeatAlpha;
        [SerializeField] private ItemAlpha doneMeatAlpha;

        [SerializeField] private SpiceItemFry onionFryControl;
        [SerializeField] private SpiceItemFry garlicFryControl;
        [SerializeField] private SpiceItemFry pepperFryControl;
        [SerializeField] private SpiceItemFry chiliFryControl;
        [SerializeField] private SpiceItemFry saltFryControl;
        [SerializeField] private SpiceItemFry turmericFryControl;
        [SerializeField] private SpiceItemFry oreganoFryControl;

        [SerializeField] private ParticleSystem oilVFX;
        [SerializeField] private ParticleSystem smokeVFX;
        [SerializeField] private ParticleSystem blinkVFX;

        [SerializeField] private Transform meatGO;
        [SerializeField] private HintText hintText_garlic, hintText_addMeat, hintText_addSpice;

        private float[] sizeFloat = {0f, .5f, .75f, 1f};
        private int takeMeatCount = 0;
        private float cookingRate = 0;

        public override bool IsState<T>(T t)
        {
            return state == (State)(object)t;
        }

        public override void ChangeState<T>(T t)
        {
            state = (State)(object)t;

            switch (state)
            {
                case State.HaveOil:
                    anim.Play(animOil);
                    oilVFX.Play();
                    break;
                case State.HaveOnion:
                    onionDrop2D.OnActive();
                    break;
                case State.HaveGarlic:
                    garlicDrop2D.OnActive();
                    ChangeState(State.Mixing1);
                    break;
                case State.Meat:
                    hintText_garlic.OnActiveHint();
                    LevelControl.Ins.NextHint();
                    blinkVFX.Play();
                    break;
                case State.Peper:
                    hintText_addMeat.OnActiveHint();
                    break;
                case State.Mixing2:
                    hintText_addSpice.OnActiveHint();
                    break;
                case State.DoneMix2:
                    LevelControl.Ins.NextHint();
                    blinkVFX.Play();
                    SoundControl.Ins.PlayFX(Fx.DoneSomething);
                    smokeVFX.Play();
                    break;
                case State.WaitForTurnOff:
                    smokeVFX.Stop();
                    break;
                case State.Done:
                    LevelControl.Ins.NextHint();
                    LevelControl.Ins.CheckStep(1f);
                    oilVFX.Play();
                    break;
            }
        }

        public void OnClickButton(bool isOn)
        {
            if(IsState(State.Normal) && isOn)
            {
                ChangeState(State.HeatOn);
            }

            if (IsState(State.WaitForTurnOff) && !isOn)
            {
                ChangeState(State.Done);
            }

        }
        public void TakeMeat()
        {
            anim.Play(animMeat);
            meatGO.localScale = Vector3.one * sizeFloat[takeMeatCount];
            takeMeatCount++;

            if (takeMeatCount >= 3)
                ChangeState(State.WaitForTurnOff);
        }

        public void SetCookingRate(Vector3 point)
        {
            if(IsState(State.Mixing1) || IsState(State.Mixing2))
            {
                cookingRate += Time.deltaTime * 0.5f;
            }

            if (IsState(State.Mixing2))
            {
                anim.Play(animMeat);
                chiliFryControl.SetRipeRate(cookingRate);
                garlicFryControl.SetRipeRate(cookingRate);
                onionFryControl.SetRipeRate(cookingRate);
                oreganoFryControl.SetRipeRate(cookingRate);
                pepperFryControl.SetRipeRate(cookingRate);
                saltFryControl.SetRipeRate(cookingRate);
                turmericFryControl.SetRipeRate(cookingRate);

                if (cookingRate >= 2f && cookingRate < 3f)
                {
                    rawMeatAlpha.SetAlpha(3f -  cookingRate);
                    tomatoSauceAlpha.SetAlpha(3f - cookingRate);
                    mixMeatAlpha.SetAlpha(cookingRate - 2f);
                }

                if(cookingRate >= 3f && cookingRate < 4f)
                {
                    mixMeatAlpha.SetAlpha(4f - cookingRate);
                    doneMeatAlpha.SetAlpha(cookingRate - 3f);
                }

                if (cookingRate >= 4f)
                {
                    ChangeState(State.DoneMix2);
                }
            }

            if(IsState(State.Mixing1))
            {
                onionFryControl.SetRipeRate(cookingRate);
                garlicFryControl.SetRipeRate(cookingRate);

                if(cookingRate >= 1)
                {
                    ChangeState(State.Meat);
                }
            }
        }

        public override bool OnTake(IItemMoving item)
        {
            if(item is BowlPur && item.IsState(BowlPur.State.Normal) && IsState(State.HeatOn)){
                item.OnMove(TF.position + Vector3.up * .4f + Vector3.left * .15f, Quaternion.identity, .2f);
                item.ChangeState(BowlPur.State.Pouring);

                ChangeState(State.HeatOn);
                return true;
            }

            if (item is SpiceItem && item.IsState(SpiceItem.State.Normal)) 
            {
                return TakeSpiceItem(item as SpiceItem);
            }
            return base.OnTake(item);
        }

        public bool TakeSpiceItem(SpiceItem spiceItem)
        {
            if (IsState(State.HaveOil) && spiceItem.IsSpiceType(SpiceType.Onion))
            {
                spiceItem.OnMove(TF.position + Vector3.up * .4f + Vector3.left * .15f, Quaternion.identity, .2f);
                spiceItem.ChangeState(SpiceItem.State.Pouring);

                StartCoroutine(WaitToPourOnion());
                return true;
            }

            if (IsState(State.HaveOnion) && spiceItem.IsSpiceType(SpiceType.Garlic))
            {
                spiceItem.OnMove(TF.position + Vector3.up * .4f + Vector3.left * .15f, Quaternion.identity, .2f);
                spiceItem.ChangeState(SpiceItem.State.Pouring);

                StartCoroutine(WaitToPourGarlic());
                return true;
            }

            if (IsState(State.Meat) && spiceItem.IsSpiceType(SpiceType.Meat))
            {
                spiceItem.OnMove(TF.position + Vector3.up * .4f + Vector3.left * .15f, Quaternion.identity, .2f);
                spiceItem.ChangeState(SpiceItem.State.Pouring);

                StartCoroutine(WaitToPourMeat());
                return true;
            }

            if (IsState(State.Chili) && spiceItem.IsSpiceType(SpiceType.Chili))
            {
                spiceItem.OnMove(TF.position + Vector3.up * .4f + Vector3.left * .15f, Quaternion.identity, .2f);
                spiceItem.ChangeState(SpiceItem.State.Pouring);

                StartCoroutine(WaitToPourChili());
                return true;
            }

            if (IsState(State.Salt) && spiceItem.IsSpiceType(SpiceType.Salt))
            {
                spiceItem.OnMove(TF.position + Vector3.up * .4f + Vector3.left * .15f, Quaternion.identity, .2f);
                spiceItem.ChangeState(SpiceItem.State.Pouring);

                StartCoroutine(WaitToPourSalt());
                return true;
            }

            if (IsState(State.Peper) && spiceItem.IsSpiceType(SpiceType.Pepper))
            {
                spiceItem.OnMove(TF.position + Vector3.up * .4f + Vector3.left * .15f, Quaternion.identity, .2f);
                spiceItem.ChangeState(SpiceItem.State.Pouring);

                StartCoroutine(WaitToPourPepper());
                return true;
            }

            if (IsState(State.Tomato) && spiceItem.IsSpiceType(SpiceType.Tomato))
            {
                spiceItem.OnMove(TF.position + Vector3.up * .4f + Vector3.left * .15f, Quaternion.identity, .2f);
                spiceItem.ChangeState(SpiceItem.State.Pouring);

                StartCoroutine(WaitToPourTomato());
                return true;
            }

            if (IsState(State.Oregano) && spiceItem.IsSpiceType(SpiceType.Oregano))
            {
                spiceItem.OnMove(TF.position + Vector3.up * .4f + Vector3.left * .15f, Quaternion.identity, .2f);
                spiceItem.ChangeState(SpiceItem.State.Pouring);

                StartCoroutine(WaitToPourOregano());
                return true;
            }

            if(IsState(State.Turmeric) && spiceItem.IsSpiceType(SpiceType.Tumeric))
            {
                spiceItem.OnMove(TF.position + Vector3.up * .4f + Vector3.left * .15f, Quaternion.identity, .2f);
                spiceItem.ChangeState(SpiceItem.State.Pouring);

                StartCoroutine (WaitToPourTurmeric());
                return true;
            }
            return false;
        }

        private IEnumerator WaitToPourOnion()
        {
            yield return WaitForSecondCache.Get(.9f);
            onionDrop2D.OnActive();
            ChangeState(State.HaveOnion);
        }
        private IEnumerator WaitToPourGarlic()
        {
            yield return WaitForSecondCache.Get(.9f);
            garlicDrop2D.OnActive();
            ChangeState(State.HaveGarlic);
        }
        private IEnumerator WaitToPourMeat()
        {
            yield return WaitForSecondCache.Get(.7f);
            peperDrop2D.OnActive();
            anim.Play(animMeat);
            ChangeState(State.Peper);
        }
        private IEnumerator WaitToPourChili()
        {
            yield return WaitForSecondCache.Get(.9f);
            chiliDrop2D.OnActive();
            ChangeState(State.Salt);
        }
        private IEnumerator WaitToPourPepper()
        {
            yield return WaitForSecondCache.Get(.9f);
            peperDrop2D.OnActive();
            ChangeState(State.Chili);
        }
        private IEnumerator WaitToPourSalt()
        {
            yield return WaitForSecondCache.Get(.9f);
            ChangeState(State.Turmeric);
        }
        private IEnumerator WaitToPourTurmeric()
        {
            yield return WaitForSecondCache.Get(.9f);
            turmericDrop2D.OnActive();
            ChangeState (State.Tomato);
        }
        private IEnumerator WaitToPourTomato()
        {
            yield return WaitForSecondCache.Get(.3f);
            anim.Play(animTomato);
            ChangeState(State.Oregano);
        }
        private IEnumerator WaitToPourOregano()
        {
            yield return WaitForSecondCache.Get(.9f);
            oreganoDrop2D.OnActive();
            ChangeState(State.Mixing2);
        }
    }
}
