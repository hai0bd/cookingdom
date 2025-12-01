using Link;
using System;
using System.Collections;
using UnityEngine;

namespace Hai.Cooking.NewTest
{
    public class Pan : ItemIdleBase
    {
        public enum State
        {
            Normal,
            HeatOn,
            HaveOil,
            HaveGarlic,
            HaveMeat,
            HaveOnion,
            HaveSalt,
            HavePepper,
            Mixing,
            WaitForTurnOff,
            Done
        }

        [SerializeField] private State state;
        [SerializeField] private ParticleSystem oilVFX;
        [SerializeField] private ParticleSystem smokeVFX;

        //[SerializeField] private Drop2D garlicDrop;
        //[SerializeField] private Drop2D onionDrop;

        [SerializeField] private GameObject heatOn;
        [SerializeField] private GameObject haveOil;
        [SerializeField] private GameObject haveGarlic;
        [SerializeField] private GameObject haveOnion;
        [SerializeField] private GameObject haveMeat;
        [SerializeField] private GameObject haveSalt;
        [SerializeField] private GameObject havePepper;

        [SerializeField] private ItemAlpha rawMeatAlpha;
        [SerializeField] private ItemAlpha mixMeatAlpha;
        [SerializeField] private ItemAlpha cookedMeatAlpha;

        [SerializeField] private SpiceItemFryControl onionFryControl;
        [SerializeField] private SpiceItemFryControl garlicFryControl;

        [SerializeField] private HintText hintText_onion, hintText_garlic, hintText_meat;
        [SerializeField] private Transform meatSc;

        [SerializeField] private int takeMeatCount = 3;
        [SerializeField] private float[] sizefloat = {0, 0.5f, 0.75f};

        private float cookingRate = 0f;

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
                case State.HeatOn:
                    heatOn.SetActive(true);
                    break;
                case State.HaveOil:
                    haveOil.SetActive(true);
                    break;
                case State.HaveGarlic:
                    haveGarlic.SetActive(true);
                    //garlicDrop.OnActive();
                    break;
                case State.HaveMeat:
                    haveMeat.SetActive(true);
                    break;
                case State.HaveOnion:
                    haveOnion.SetActive(true);
                    //onionDrop.OnActive();
                    break;
                case State.HavePepper:
                    havePepper.SetActive(true);
                    ChangeState(State.Mixing);
                    break;
                case State.Mixing:
                    break;
                case State.WaitForTurnOff:
                    smokeVFX.Stop();
                    break;
                case State.Done:
                    OnDoneCooking();
                    break;
            }
        }

        public override bool OnTake(IItemMoving item)
        {
            if(item is SpiceItem spiceItem && item.IsState(SpiceItem.State.Normal))
            {
                //Debug.Log("On Take ItemMoving");
                return OnTakeSpice(spiceItem);
            }
            return base.OnTake(item);
        }

        public bool OnTakeSpice(SpiceItem spiceItem)
        {
            if (IsState(State.HeatOn) && spiceItem.IsSpiceType(SpiceType.Oil))
            {
                return PouringSpiceItem(spiceItem, State.HaveOil, 1.2f);
            }
            if (IsState(State.HaveOil) && spiceItem.IsSpiceType(SpiceType.Garlic))
            {
                return PouringSpiceItem(spiceItem, State.HaveGarlic, 2f);
            }
            if(IsState(State.HaveGarlic) && spiceItem.IsSpiceType(SpiceType.Meat))
            {
                return PouringSpiceItem(spiceItem, State.HaveMeat, 2f);
            }
            if(IsState(State.HaveMeat) && spiceItem.IsSpiceType(SpiceType.Onion))
            {
                return PouringSpiceItem(spiceItem, State.HaveOnion, 2f);
            }
            if(IsState(State.HaveOnion) && spiceItem.IsSpiceType(SpiceType.Salt))
            {
                return PouringSpiceItem(spiceItem, State.HaveSalt,1.2f);
            }
            if(IsState(State.HaveSalt) && spiceItem.IsSpiceType(SpiceType.Pepper))
            {
                return PouringSpiceItem(spiceItem, State.HavePepper, 1.2f);
            }
            return false;
        }

        public void TakeMeat()
        {
            takeMeatCount--;
            meatSc.localScale = Vector3.one * sizefloat[takeMeatCount];
            if (takeMeatCount <= 0)
            {
                ChangeState(State.WaitForTurnOff);
            }
            Debug.Log(takeMeatCount);
        }

        public void OnClickButton(bool isOn)
        {
            if(isOn && IsState(State.Normal))
            {
                ChangeState(State.HeatOn);
            }
            else if (!isOn && IsState(State.WaitForTurnOff))
            {
                ChangeState(State.Done);
            }
        }

        public void SetCookingRate()
        {
            if(IsState(State.Mixing))
            {
                cookingRate += Time.deltaTime * 0.5f;
                onionFryControl.SetRipeRate(cookingRate);
                garlicFryControl.SetRipeRate(cookingRate);
                if (cookingRate >= 2f && cookingRate < 3f)
                {
                    rawMeatAlpha.SetAlpha(3 - cookingRate);
                    mixMeatAlpha.SetAlpha(cookingRate - 2);
                }
                else if (cookingRate >= 3f && cookingRate < 4f)
                {
                    mixMeatAlpha.SetAlpha(4 - cookingRate);
                    cookedMeatAlpha.SetAlpha(cookingRate - 3);
                }
                else if (cookingRate >= 4f)
                {
                    ChangeState(State.WaitForTurnOff);
                }
            }
        }

        private bool PouringSpiceItem(SpiceItem item, State state, float time)
        {
            item.OnMove(TF.position + Vector3.up * 0.4f + Vector3.left * 0.15f, Quaternion.identity, 0.2f);
            item.ChangeState(SpiceItem.State.Pouring);
            StartCoroutine(WaitForPouring(time, state));
            return true;
        }

        private IEnumerator WaitForPouring(float time, State state)
        {
            yield return WaitForSecondCache.Get(time);
            ChangeState(state);
        }

        private void OnDoneCooking()
        {
            LevelControl.Ins.NextHint();
            LevelControl.Ins.CheckStep();
            oilVFX.Stop();
        }
    }
}