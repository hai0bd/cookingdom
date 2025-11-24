using Link;
using System;
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
            HaveOnion,
            HaveGarlic,
            Mixing,
            WaitForTurnOff,
            Done
        }

        [SerializeField] private State state;

        [SerializeField] private HintText hintText_onion, hintText_garlic, hintText_meat;

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
                    
                    break;
                case State.HaveOil:

                    break;
                case State.HaveOnion:
                    break;
                case State.HaveGarlic:
                    break;
                case State.Mixing:
                    break;
                case State.WaitForTurnOff:
                    //smoke.VFX.Stop();
                    WaitForTurnOff();
                    break;
                case State.Done:
                    OnDoneCooking();
                    break;
            }
        }

        private void WaitForTurnOff()
        {
            
        }

        private void OnDoneCooking()
        {
            throw new NotImplementedException();
        }
    }
}