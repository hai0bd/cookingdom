using HoangLinh.Cooking.Test;
using Link;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoangLinh.Cooking.Test
{
    public enum Fx
    {
        PickUp,
        Drop,
        DoneSth,
        Correct,
        Wrong,
        WaterFall,
        WaterSplash,
        DropInWater,
        BeanPouring,
    }

    public class LevelStep_0 : LevelStepBase
    {
       

        [SerializeField] Basket basket;
        [SerializeField] DirtyItems onion, tomato, lettuce;
        [SerializeField] WaterTap waterTap;
        [SerializeField] WaterInSink waterInSink;
        [SerializeField] BeanBasket beanBasket;
        [SerializeField] BeanPlate beanPlate;


        private void Start()
        {
            //FOR TEST ONLY
            this.gameObject.SetActive(false);
        }
        public override bool IsDone()
        {
            if (onion.IsState(DirtyItems.State.Done) &&
                lettuce.IsState(DirtyItems.State.Done) &&
                waterTap.IsOff() &&
                waterInSink.isWater == false)
            {
                Debug.Log("Level Done");
                return true;
            }
            return false;
        }
    }
}