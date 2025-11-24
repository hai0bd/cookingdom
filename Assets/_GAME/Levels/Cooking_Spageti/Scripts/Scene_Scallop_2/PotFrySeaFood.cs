using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace Link.Cooking.Spageti
{
    public class PotFrySeaFood : ItemIdleBase
    {
        [SerializeField] GameObject point;
        [SerializeField] List<Seafood> seafoods;

        [SerializeField] ParticleSystem steamVFX, sparkVFX, smokeVFX;
        [SerializeField] Transform oil;
        [SerializeField] bool isFire, isOil;
        [SerializeField] Stove stove;
        [SerializeField] HintText hintText_1, hintText_2, hintText_3;
        public override bool IsDone => seafoods.Count == 0;
        //nau so truoc
        //du so roi thi chuyen sang trang thai 
        public UnityEvent OnDoneEvent;


        void Awake()
        {
            stove.OnStoveFire = SetFire;
        }

        void OnDestroy()
        {
            SoundControl.Ins.StopFX(Fx.Fry);
        }
        
        void Start()
        {
            foreach (var item in seafoods)
            {
                item.OnDoneEvent.AddListener(OnRiped);
            }
            CheckCooking();
        }

        public enum State { Normal, Cooking, Cooked ,Done }
        [SerializeField] State state;
        public override void ChangeState<T>(T t)
        {
            state = (State)(object)t;

            switch (state)
            {
                case State.Normal:
                    break;
                case State.Cooking:
                    break;
                case State.Cooked:
                    OnDoneEvent?.Invoke();  
                    break;
                case State.Done:
                    break;
            }
        }
        public override bool IsState<T>(T t)
        {
            return state == (State)(object)t;
        }

        public override bool OnTake(IItemMoving item)
        {
            if (item is Bowl)
            {
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
                    hintText_2.OnActiveHint();
                });
                return true;
            }
            return base.OnTake(item);
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

            if (isFire) hintText_1.OnActiveHint();
        }

        private void CheckCooking()
        {
            if (isFire && isOil && IsState(State.Normal))
            {
                point.SetActive(true);
                if(stove != null) stove.SetInteract(false);
     
            }
        }

        public void OnRiped(Seafood seafood)
        {
            if (seafoods.Contains(seafood))
            {
                seafoods.Remove(seafood);
                if (seafoods.Count == 0)
                {
                    ChangeState(State.Cooked);
                    hintText_3.OnActiveHint();
                }
            }
        }

    }
}