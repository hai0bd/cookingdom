using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link.Turkey
{
    public class Decor : ItemMovingBase
    {
        public enum DecorType { Apple, Lemon, Tomato, Onion, Leaf }
        int orderLayer;
        public DecorType DecorName;
        public enum State { Stack, Tidy }
        State state = State.Stack;
        public override bool IsCanMove => state == State.Stack; 

        private void Start()
        {
            orderLayer = OrderLayer;
        }

        public override void OnClickDown()
        {
            base.OnClickDown();
            OrderLayer = 50;
            SoundControl.Ins.PlayFX(LevelStep_1.FX.Hold);
        }

        public override void OnClickTake()
        {
            base.OnClickTake();
            OrderLayer = orderLayer;
            SoundControl.Ins.PlayFX(LevelStep_1.FX.Put);
        }

        public override void OnDrop()
        {
            base.OnDrop();
            OrderLayer = 30;
        }

        public override void ChangeState<T>(T t)
        {
            this.state = (State)(object)(t);
        }

        public override bool IsState<T>(T t)
        {
            return this.state == (State)(object)(t);
        }


    }
}