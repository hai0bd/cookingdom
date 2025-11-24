using Satisgame;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Link.Turkey
{
    public class Plate : ItemMovingBase
    {
        public enum State { Stack, Ready, Full, Heart }
        [ShowInInspector] State state = State.Stack;
        [field: SerializeField] public ItemName[] ItemNames { get; private set; }
        [SerializeField] EmojiControl emoji;
        int orderLayer;

        public override bool IsDone => IsState(State.Full);    

        public override bool IsCanMove => state == State.Stack;

        public UnityEvent<Plate> OnTakePlate;

        private void Awake()
        {
            orderLayer = OrderLayer;
        }

        public override void ChangeState<T>(T t)
        {
            this.state = (State)(object)(t);
            switch (state)
            {
                case State.Stack:
                    break;
                case State.Ready:
                    OnTakePlate?.Invoke(this);
                    break;
                case State.Full:
                    LevelControl.Ins.CheckStep();
                    break;
                case State.Heart:
                    if (emoji != null)
                    {
                        emoji.ShowPositive();
                    }
                    break;
                default:
                    break;
            }
        }

        public override bool IsState<T>(T t)
        {
            return this.state == (State)(object)(t);
        }

        public override bool OnTake(IItemMoving item)
        {
            if (!IsNotStack())
                return false;

            if (item is Chicken && (item.IsState(Chicken.State.Water) || item.IsState(Chicken.State.NeedWash)))
            {
                if (emoji != null)
                {
                    emoji.ShowNegative();
                }
                return false;
            }

            if (item is Chicken && item.IsState(Chicken.State.Clean) && IsHave(ItemName.Chicken))
            {
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                item.ChangeState(Chicken.State.Dish);
                ChangeState(State.Full);
                return true;
            }
            if (item is Fruit && IsState(State.Ready) && item.IsState(Fruit.State.Piece) && IsHave((item as Fruit).fruitName))
            {
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                item.ChangeState(Fruit.State.Prepare);
                ChangeState(State.Full);
                return true;
            }
            return base.OnTake(item);
        }

        public override void OnDrop()
        {
            base.OnDrop();
            SoundControl.Ins.PlayFX(LevelStep_1.FX.Put);
            if (LevelControl.Ins.IsHaveObject<ItemMovingBase>(TF.position))
            {
                OrderLayer = LevelControl.Ins.GetHighestLayer(this, TF.position) + 1;
            }
            else
            {
                OrderLayer = -1;
            }
        }

        private bool IsHave(ItemName itemName)
        {
            for (int i = 0; i < ItemNames.Length; i++)
            {
                if (itemName == ItemNames[i])
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsNotStack()
        {
            IItemMoving itemCheck = null;
            RaycastHit2D[] hits = Physics2D.RaycastAll(TF.position, Vector2.zero);

            for (int i = 0; i < hits.Length; i++)
            {
                itemCheck = hits[i].collider.GetComponent<IItemMoving>();
                if (itemCheck != null && itemCheck != this as IItemMoving && itemCheck is Plate)
                {
                    return false;
                }
            }
            return true;
        }

        [Button]
        protected override void Editor()
        {
            emoji = GetComponentInChildren<EmojiControl>();
        }

        public override void OnClickDown()
        {
            base.OnClickDown();
            SoundControl.Ins.PlayFX(LevelStep_1.FX.Hold);
        }

        public override void OnClickTake()
        {
            base.OnClickTake();
            SoundControl.Ins.PlayFX(LevelStep_1.FX.Put);
            OrderLayer = -2;
        }
    }

}