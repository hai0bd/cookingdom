using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Link;
namespace LawNguyen.CookingGame.LRisottoY
{
    public class Lid : ItemMovingBase
    {
        [SerializeField] ClockTimer timer;
        public bool iscanmove =false;
        public override bool IsCanMove => iscanmove==true;
        private void Awake()
        {
            iscanmove = true;
        }
        public void TimerCooking()
        {
            timer.Show(5);
        }
        

        public override void OnBack()
        {
            base.OnBack();
        }
        public override void OnDrop()
        {
            base.OnDrop();
        }

    }
}
