using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Link;
using DG.Tweening;
using Unity.VisualScripting;
using HuyThanh.Cooking.Burrito;

namespace HoangLinh.Cooking.Test
{
    public class BeanBasket : ItemMovingBase
    {
        public enum State
        {
            Empty,
            WithBeans,
            Washing,
            DoneWashing,
            Pouring,
            Done
        }

        [SerializeField] State state;
        [SerializeField] int numberTapToClean = 5;
        [SerializeField] Animation animClean, animBasket;
        [SerializeField] string animCleaning, animPouring;
        [SerializeField] ParticleSystem vfxBlink;
        [SerializeField] ItemAlpha waterAlpha;
        [SerializeField] ItemsChangeAlpha2D itemsChangeAlpha2D;
        [SerializeField] Vector3 oldPosition;

        private Tween delayedCallWaterLayer;


        public override bool IsCanMove => IsState(State.WithBeans, State.DoneWashing);

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
                case State.WithBeans:
                    this.SetOrder(49);
                    break;
                case State.Washing:
                    waterAlpha.DoAlpha(1f, 0.1f);
                    break;
                case State.DoneWashing:
                    
                    break;
                case State.Pouring:
                    StartCoroutine(MoveBasket());
                    break;
                case State.Done:
                    OnMove(oldPosition, Quaternion.identity, 0.2f);
                    OnSave(0.2f);
                    LevelControl.Ins.CheckStep(1f);
                    break;
            }
        }
        public override bool OnTake(IItemMoving item)
        {
            if (item is BeanPlate && item.IsState(BeanPlate.State.HaveBean))
            {
                item.OnMove(this.TF.position, this.TF.rotation, 0.3f);
                DOVirtual.DelayedCall(0.9f, () =>
                {
                    ChangeState(State.WithBeans);
                    itemsChangeAlpha2D.DoAlpha(0, 0.3f);
                });
                item.ChangeState(BeanPlate.State.Pouring);
                return true;
            }


            return base.OnTake(item);
        }

        public override void OnClickDown()
        {
            if (IsState(State.Washing))
            {
                SoundControl.Ins.PlayFX(Fx.WaterSplash);
                numberTapToClean--;
                animClean.Play(animCleaning);

                if (numberTapToClean == 0)
                {
                    animClean.Stop();
                    vfxBlink.transform.position = TF.position;
                    vfxBlink.Play();
                    SoundControl.Ins.PlayFX(Fx.DoneSth);
                    ChangeState(State.DoneWashing);
                }
                return;
            }

            if (IsState(State.DoneWashing) && LevelControl.Ins.IsHaveObject<WaterInSink>(TF.position))
            {
                if (delayedCallWaterLayer != null && delayedCallWaterLayer.IsActive())
                {
                    delayedCallWaterLayer.Kill();
                }
                waterAlpha.DoAlpha(0, 0.1f);

                base.OnClickDown();
               
            }

            
        }

        public override void OnDrop()
        {
            delayedCallWaterLayer = DOVirtual.DelayedCall(0.3f, () =>
            {
                if (IsState(State.DoneWashing) && LevelControl.Ins.IsHaveObject<WaterInSink>(TF.position))
                {
                    waterAlpha.DoAlpha(1, 0.1f);
                }
            });


            base.OnDrop();
        }

        protected override void Editor()
        {
            base.Editor();
            itemsChangeAlpha2D = GetComponent<ItemsChangeAlpha2D>();
        }

        IEnumerator MoveBasket()
        {
            Vector3 targetPosition = TF.position + new Vector3(.5f, 1.5f, 0f);
            float moveDuration = 0.5f;

            TF.DOMove(targetPosition, moveDuration).SetEase(Ease.OutQuad);

            yield return new WaitForSeconds(moveDuration);

            animBasket.Play(animPouring);
            yield return new WaitForSeconds(0.8f);
            ChangeState(State.Done);

        }
    }
    
}