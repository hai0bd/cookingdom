using DG.Tweening;
using Link;
using System.Collections;
using UnityEngine;

namespace HuyThanh.Cooking.SarmaleRomania
{
    public class PotCabbage : ItemIdleBase
    {
        public enum State
        {
            Rosemary,
            CabbageMix,
            Dill,
            CabbageRolls,
            TomatoSoy,
            Ribs,
            WaterBowl,
            WaitForNapNoi,
            WaitForTurnOnButton,
            WaitForClock,
            Cooking,
            WaitForTurnOff,
            Done
        }

        [SerializeField] State state;

        [SerializeField] Transform napNoiTF;
        [SerializeField] GameObject cabbageRollGO;

        [SerializeField] Drop2D rosemaryDrop, dillDrop;
        [SerializeField] ItemAlpha cabbageAlpha, tomatoAlpha, waterAlpha;
        [SerializeField] Transform squareCabbage, squareTomato, squareWater;

        [SerializeField] CapyBat capyBat;

        [SerializeField] Clock clock;
        [SerializeField] MinuteHand kimPhut;

        private NapNoi napNoi;
        private int countCabbageRoll = 0;
        private int countRibs = 0;
        private bool isHaveNapNoi => (napNoi != null && Vector3.Distance(napNoi.TF.position, napNoiTF.position) < 0.3f);

        private void OnEnable()
        {
            kimPhut.onDone += (() => ChangeState(State.WaitForTurnOff));
        }
        private void OnDisable()
        {
            kimPhut.onDone -= (() => ChangeState(State.WaitForTurnOff));
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
                case State.CabbageRolls:
                    cabbageRollGO.SetActive(true);
                    break;

                case State.WaterBowl:
                    capyBat.StartCapySteal();
                    break;
                case State.Cooking:
                    break;

                case State.WaitForClock:
                    clock.OnCollition();
                    break;
                case State.WaitForTurnOff:

                    break;
            }
        }

        public override bool OnTake(IItemMoving item)
        {
            if (item is NapNoi)
            {
                if (napNoi == null) napNoi = item as NapNoi;
                item.OnMove(napNoiTF.position, Quaternion.identity, 0.2f);

                if (IsState(State.WaitForNapNoi))
                {
                    napNoi.SetCollider(false);
                    ChangeState(State.WaitForTurnOnButton);
                }
                return true;
            }

            if (!isHaveNapNoi && item is ItemPouring rosemaryPouring && IsState(State.Rosemary) && rosemaryPouring.IsPouringType(ItemPouring.PouringType.Rosemary))
            {
                StartCoroutine(WaitForDoneRosemary());
                item.OnMove(napNoiTF.position, Quaternion.identity, 0.2f);
                item.ChangeState(ItemPouring.State.Pouring);
                return true;
            }

            if (!isHaveNapNoi && item is ItemPouring cabbagePouring && IsState(State.CabbageMix) && cabbagePouring.IsPouringType(ItemPouring.PouringType.Cabbage))
            {
                StartCoroutine(WaitForDoneCabbage());
                item.OnMove(napNoiTF.position, Quaternion.identity, 0.2f);
                item.ChangeState(ItemPouring.State.Pouring);
                return true;
            }

            if (!isHaveNapNoi && item is ItemPouring dillPouring && IsState(State.Dill) && dillPouring.IsPouringType(ItemPouring.PouringType.Dill))
            {
                StartCoroutine(WaitForDoneDill());
                item.OnMove(napNoiTF.position, Quaternion.identity, 0.2f);
                item.ChangeState(ItemPouring.State.Pouring);
                return true;
            }

            if (!isHaveNapNoi && item is ItemPouring tomatoSauce && IsState(State.TomatoSoy) && tomatoSauce.IsPouringType(ItemPouring.PouringType.TomatoSauce))
            {
                StartCoroutine(WaitForDoneTomato());
                item.OnMove(napNoiTF.position, Quaternion.identity, 0.2f);
                item.ChangeState(ItemPouring.State.Pouring);

                return true;
            }

            if (!isHaveNapNoi && item is ItemPouring waterBowl && IsState(State.WaterBowl) && waterBowl.IsPouringType(ItemPouring.PouringType.WaterBowl))
            {
                StartCoroutine(WaitForDoneWaterPouring());
                item.OnMove(napNoiTF.position, Quaternion.identity, 0.2f);
                item.ChangeState(ItemPouring.State.Pouring);

                return true;
            }
            return base.OnTake(item);
        }

        IEnumerator WaitForDoneRosemary()
        {
            yield return WaitForSecondCache.Get(1f);
            rosemaryDrop.OnActive();
            ChangeState(State.CabbageMix);
        }

        IEnumerator WaitForDoneCabbage()
        {
            yield return WaitForSecondCache.Get(0.5f);
            cabbageAlpha.DoAlpha(1, 0.5f);
            squareCabbage.DOScale(1, 0.5f);
            yield return WaitForSecondCache.Get(0.5f);
            ChangeState(State.Dill);
        }

        IEnumerator WaitForDoneDill()
        {
            yield return WaitForSecondCache.Get(1f);
            dillDrop.OnActive();
            ChangeState(State.CabbageRolls);
        }

        IEnumerator WaitForDoneTomato()
        {
            yield return WaitForSecondCache.Get(0.5f);
            tomatoAlpha.DoAlpha(1, 0.5f);
            squareTomato.DOScale(1, 0.5f);
            yield return WaitForSecondCache.Get(0.5f);
            ChangeState(State.Ribs);
        }

        IEnumerator WaitForDoneWaterPouring()
        {
            yield return WaitForSecondCache.Get(0.5f);
            waterAlpha.DoAlpha(1, 0.5f);
            squareWater.DOScale(1, 0.5f);
            yield return WaitForSecondCache.Get(0.5f);
            ChangeState(State.WaitForNapNoi);
        }

        public void CheckDoneCabbageRoll()
        {
            countCabbageRoll++;
            if (countCabbageRoll == 6)
            {
                ChangeState(State.TomatoSoy);
            }
        }

        public void CheckDoneRib()
        {
            countRibs++;
            if (countRibs == 8)
            {
                ChangeState(State.WaterBowl);
            }
        }

        public void OnClickButton()
        {
            if (IsState(State.WaitForTurnOnButton))
            {
                ChangeState(State.WaitForClock);
            }
            if (IsState(State.WaitForTurnOff))
            {
                ChangeState(State.Done);
            }
        }
    }
}