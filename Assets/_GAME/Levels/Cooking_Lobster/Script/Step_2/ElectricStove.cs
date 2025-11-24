using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Link.Cooking.Lobster
{
    public class ElectricStove : ItemIdleBase
    {
        public enum State { Contact, NoContact }
        [SerializeField, ReadOnly] State state;
    
        [SerializeField] GameObject on;
        [SerializeField] UnityEvent<bool> turnOnEvent;

        public bool IsOn => on.activeSelf;

        public override void OnClickDown()
        {
            base.OnClickDown();

            if (IsState(State.Contact))
            {
                on.SetActive(!IsOn);
                turnOnEvent?.Invoke(IsOn);
                LevelControl.Ins.CheckStep();
                SoundControl.Ins.PlayFX(LevelStep_1.Fx.ElectricTurn);
            }
        }

        public override void ChangeState<T>(T t)
        {
            state = (State)(object)t;
        }
        public override bool IsState<T>(T t)
        {
            return state == (State)(object)t;
        }
    }
}