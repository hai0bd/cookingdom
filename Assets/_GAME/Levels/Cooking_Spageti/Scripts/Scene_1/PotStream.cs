using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Link.Cooking.Spageti
{
    public class PotStream : ItemIdleBase
    {
        public enum State { Normal, Cooking, Done }
        [SerializeField] State state;

        [SerializeField] Transform water, waterStream;
        [SerializeField] ParticleSystem steamVFX, sparkVFX;
        [SerializeField] Stove stove;
        [SerializeField] SpriteRenderer hotWater;

        [SerializeField] HintText hintText_1, hintText_2, hintText_3, hintText_4;

        Noodle noodle;
        Salt salt;

        bool isFire, isWater, isSalt, isNoodle;

        void Awake()
        {
            stove.OnStoveFire = SetFire;
        }

        public override bool OnTake(IItemMoving item)
        {
            if(!isFire && item is not Scoop) 
            {
                LevelControl.Ins.LoseHalfHeart(TF.position);
                return false;
            }

            if (item is Salt)
            {
                item.TF.position = TF.position;
                item.TF.SetParent(TF);
                item.OnDone();
                item.SetOrder(-10);
                isSalt = true;
                DOVirtual.DelayedCall(0.5f, CheckCooking);
                salt = item as Salt;
                hintText_3.OnActiveHint();
                return true;
            }
            if (item is Bowl)
            {
                item.ChangeState(Bowl.State.Pouring);
                item.OnMove(TF.position + new Vector3(0.5f, 1, 0), Quaternion.identity, 0.2f);
                isWater = true;
                water.gameObject.SetActive(true);
                water.DOScale(1, 1f).SetDelay(1f).OnComplete(() =>
                {
                    steamVFX.gameObject.SetActive(isFire && isWater);
                    CheckCooking(); 
                    if(isNoodle) waterStream.DOScale(0.5f, 1f);
                });
                if(isFire && isWater) SoundControl.Ins.PlayFX(Fx.Boil, true);
                hintText_2.OnActiveHint();
                return true;
            }
            if (item is Noodle && isFire && isWater && isSalt && !isNoodle)
            {
                item.ChangeState(Noodle.State.InPot);
                item.OnMove(TF.position + new Vector3(0, .25f, 0), Quaternion.identity, 0.2f);
                item.TF.SetParent(TF);
                isNoodle = true;
                noodle = item as Noodle;
                DOVirtual.DelayedCall(1.5f, CheckCooking);
                waterStream.DOScale(0.8f, 1.5f);

                return true;
            }
            if(item is not Scoop) LevelControl.Ins.LoseHalfHeart(TF.position);
            return false;
        }

        private void SetFire(bool fire)
        {
            isFire = fire;
            steamVFX.gameObject.SetActive(isFire && isWater);
            DOVirtual.DelayedCall(1f, CheckCooking);

            if (isFire && isWater) SoundControl.Ins.PlayFX(Fx.Boil, true);
            if (!isFire) SoundControl.Ins.StopFX(Fx.Boil);

            if (isFire) hintText_1.OnActiveHint();
            if (!isFire && IsState(State.Done)) hintText_4.OnActiveHint();
        }

        private void CheckCooking()
        {
            if (isFire && isWater && isNoodle && IsState(State.Normal))
            {
                ChangeState(State.Cooking);
            }
        }

        public override void ChangeState<T>(T t)
        {
           state = (State)(object)t;
            switch (state)
            {
                case State.Normal:
                    break;
                case State.Cooking:
                    stove.SetInteract(false);
                    StartCoroutine(IECooking());
                    salt.gameObject.SetActive(false);   
                    break;
                case State.Done:
                    stove.SetInteract(true);
                    OnDone();
                    break;
            }
        }
        public override bool IsState<T>(T t)
        {
           return state == (State)(object)t;
        }

        private IEnumerator IECooking()
        {
            waterStream.DOScale(1, 4f);
            yield return new WaitForSeconds(1f);
            noodle.ChangeState(Noodle.State.Cooking);
            waterStream.DOScale(1f, 4f);
            yield return new WaitForSeconds(4f);
            noodle.ChangeState(Noodle.State.Stream);
            hotWater.DOColor(Color.yellow, 4f);
            hotWater.DOFade(.5f, 4f);
            yield return new WaitForSeconds(6f);
            noodle.ChangeState(Noodle.State.Cooked);
            ChangeState(State.Done);
            sparkVFX.Play();
            stove.SetOnlyTurnOff();
        }
    }
}
