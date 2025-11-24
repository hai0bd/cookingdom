using DG.Tweening;
using Link;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace HuyThanh.Cooking.SarmaleRomania
{
    public class PotCabbgeRaw : ItemIdleBase
    {
        public enum State
        {
            WaitForTurnOn,
            WaitingWater,
            WaitingDill,
            WaitingRosemary,
            WaitingSalt,
            WaitingCabbage,
            Cooking,
            WaitForTurnOff,
            WaitingFork,
            Done,
        }
        [SerializeField] State state;
        [SerializeField] Transform napNoiTF;
        [SerializeField] ItemAlpha squareWaterAlpha, squareWaterBoilAlpha, squareWaterCookAlpha, squareDillAlpha, squareDillBoilAlpha, squareSaltAlpha, squareWaterDoneCookedAlpha;
        [SerializeField] Drop2D dillDrop2D, rosemaryDrop2D, saltDrop2D;
        [SerializeField] ClockTimer clock;
        [SerializeField] ParticleSystem smokeVFX;
        [SerializeField] Animation animWaterBoil;
        [SerializeField] UnityEvent onDonePouringWaterEvent;


        public override bool IsState<T>(T t)
        {
            return state == (State)(object)t;
        }

        public override void ChangeState<T>(T t)
        {
            state = (State)(object)t;

            switch (state)
            {
                case State.WaitingFork:
                    StartCoroutine(WaitForWaterStopBoil());
                    break;
            }
        }

        public override bool OnTake(IItemMoving item)
        {
            if (item is ItemPouring waterBowl && waterBowl.IsPouringType(ItemPouring.PouringType.WaterBowl) && item.IsState(ItemPouring.State.Normal) && IsState(State.WaitingWater))
            {
                item.OnMove(napNoiTF.position, Quaternion.identity, 0.2f);
                item.ChangeState(ItemPouring.State.Pouring);
                StartCoroutine(WaitForPouringWater());
                return true;
            }

            if (item is ItemPouring dillBowl && dillBowl.IsPouringType(ItemPouring.PouringType.Dill) && item.IsState(ItemPouring.State.Normal) && IsState(State.WaitingDill))
            {
                item.OnMove(napNoiTF.position + Vector3.up * 0.2f, Quaternion.identity, 0.2f);
                item.ChangeState(ItemPouring.State.Pouring);
                StartCoroutine(WaitForDillPouring());
                return true;
            }

            if (item is ItemPouring rosemaryBowl && rosemaryBowl.IsPouringType(ItemPouring.PouringType.Rosemary) && item.IsState(ItemPouring.State.Normal) && IsState(State.WaitingRosemary))
            {
                item.OnMove(napNoiTF.position + Vector3.up * 0.2f, Quaternion.identity, 0.2f);
                item.ChangeState(ItemPouring.State.Pouring);
                StartCoroutine(WaitForRosemaryPouring());
                return true;
            }

            if (item is ItemPouring salt && salt.IsPouringType(ItemPouring.PouringType.Salt) && item.IsState(ItemPouring.State.Normal) && IsState(State.WaitingSalt))
            {
                item.OnMove(napNoiTF.position + Vector3.up * 0.2f, Quaternion.identity, 0.2f);
                item.ChangeState(ItemPouring.State.Pouring);
                StartCoroutine(WaitForSaltPouring());
                return true;
            }

            if (item is CabbageCook cabbage && cabbage.IsState(CabbageCook.State.Normal) && IsState(State.WaitingCabbage))
            {
                item.TF.SetParent(this.TF);
                item.TF.DOJump(TF.position + Vector3.up * 0.05f, 1, 1, 0.4f);
                cabbage.ChangeState(CabbageCook.State.Cooking);
                StartCoroutine(WaitForCooking());
                return true;
            }

            if (item is Fork fork && item.IsState(Fork.State.Normal) && IsState(State.WaitingFork))
            {
                item.OnMove(TF.position + Vector3.up * 0.2f, item.TF.rotation, 0.2f);
                item.ChangeState(Fork.State.StickingCabbage);
                return true;
            }

            return base.OnTake(item);
        }

        IEnumerator WaitForPouringWater()
        {
            squareWaterAlpha.transform.localScale = Vector3.zero;
            yield return WaitForSecondCache.Get(0.5f);
            squareWaterAlpha.transform.DOScale(1, 0.5f);
            squareWaterAlpha.DoAlpha(0.6f, 0.5f);
            yield return WaitForSecondCache.Get(0.5f);
            ChangeState(State.WaitingDill);
            yield return WaitForSecondCache.Get(0.5f);
            onDonePouringWaterEvent?.Invoke();
            yield return WaitForSecondCache.Get(0.5f);
            squareWaterAlpha.DoAlpha(0, 2f);
            squareWaterBoilAlpha.DoAlpha(0.6f, 2f);

            yield return WaitForSecondCache.Get(2f);
            smokeVFX?.Play();
            animWaterBoil.Play("WaterFlipBoil");
        }

        IEnumerator WaitForDillPouring()
        {
            yield return WaitForSecondCache.Get(0.8f);
            dillDrop2D.OnActive();
            yield return WaitForSecondCache.Get(0.2f);
            ChangeState(State.WaitingRosemary);
            squareDillAlpha.DoAlpha(0, 4f);
            squareDillBoilAlpha.DoAlpha(1f, 4f);
        }

        IEnumerator WaitForRosemaryPouring()
        {
            yield return WaitForSecondCache.Get(0.8f);
            rosemaryDrop2D.OnActive();
            yield return WaitForSecondCache.Get(0.2f);
            ChangeState(State.WaitingSalt);
        }
        IEnumerator WaitForSaltPouring()
        {
            yield return WaitForSecondCache.Get(0.8f);
            saltDrop2D.OnActive();
            yield return WaitForSecondCache.Get(0.2f);
            ChangeState(State.WaitingCabbage);
            squareSaltAlpha.DoAlpha(0, 2f);
        }

        IEnumerator WaitForCooking()
        {
            yield return WaitForSecondCache.Get(0.4f);
            clock.Show(4f);
            squareWaterBoilAlpha.DoAlpha(0, 4f);
            squareWaterCookAlpha.DoAlpha(1, 4f);
            ChangeState(State.Cooking);
            yield return WaitForSecondCache.Get(4f);
            ChangeState(State.WaitForTurnOff);
        }

        IEnumerator WaitForWaterStopBoil()
        {
            yield return WaitForSecondCache.Get(4f);
            squareWaterDoneCookedAlpha.DoAlpha(0.6f, 2f);
            squareWaterCookAlpha.DoAlpha(0, 2f);

        }
        public bool CanUseButton()
        {
            return IsState(State.WaitForTurnOff, State.WaitForTurnOn);
        }

        public void OnButtonClick(bool isOn)
        {
            if (IsState(State.WaitForTurnOn))
            {
                ChangeState(State.WaitingWater);
            }

            if (IsState(State.WaitForTurnOff))
            {
                ChangeState(State.WaitingFork);
            }
        }
    }
}