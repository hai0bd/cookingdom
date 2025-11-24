using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link.Cooking.Lobster
{
    public class ItemSteam : ItemMovingBase
    {
        public enum State { Origin, Cutting, Cut, Done, }
        [SerializeField, ReadOnly] State state = State.Origin;

        [SerializeField] AnimaBase2D[] items;

        public override bool IsCanMove => !IsState(State.Done, State.Cutting); 

        public override void ChangeState<T>(T t)
        {
            state = (State)(object)t;

            switch (state)
            {
                case State.Origin:
                    break;
                case State.Cutting:
                    DOVirtual.DelayedCall(1.2f, () => { ChangeState(State.Cut) ; OnActiveAnim(); });
                    break;
                case State.Cut:
                    break;
                case State.Done:
                    //TODO: Anim
                    break;
                default:
                    break;
            }

            items[(int)state - 1].gameObject.SetActive(false);
            items[(int)state].gameObject.SetActive(true);
        }

        public override void OnClickDown()
        {
            base.OnClickDown();
            SoundControl.Ins.PlayFX(LevelStep_1.Fx.Click);
        }

        public override void OnClickTake()
        {
            TF.DOScale(1, 0.1f);
            OrderLayer = LevelControl.Ins.GetHighestNoneContactLayer(this, TF.position, -8) + 1;
        }

        public void OnActiveAnim()
        {
            items[(int)state].OnActive();
        }

        public override bool IsState<T>(T t)
        {
            return state == (State)(object)t;
        }

        public override bool OnTake(IItemMoving item)
        {
            if (item is Knife && IsState(State.Origin) && LevelControl.Ins.IsHaveObject<CuttingBoard>(TF.position) != null)
            {
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                item.ChangeState(Knife.State.Cutting);
                ChangeState(State.Cutting);
                LevelControl.Ins.SetStep(LevelName.Lobster, Step.FruitCut);

                return true;
            }
            if(item is Towel && IsState(State.Origin)) LevelControl.Ins.LoseFullHeart(TF.position);
            return base.OnTake(item);
        }

        public override void OnDrop()
        {
            base.OnDrop();
            OnBack();
        }
    }
}

