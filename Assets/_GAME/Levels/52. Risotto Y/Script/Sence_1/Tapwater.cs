using Link;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LawNguyen.CookingGame.LRisottoY
{
    public class Tapwater : ItemIdleBase
    {

        public bool IsOn=false;
       [SerializeField]  WaterSink waterSink;

        public event Action<UnityEngine.Object> OnCompleteEvent;




        public override void OnClickDown()
        {
            IsOn= !IsOn;
            waterSink.OnOpenWater(IsOn);
            if (IsOn)
            {
                SoundControl.Ins.PlayFX(SoundFXEnum.Soundname.watersink,true);   
            }else
            {
                SoundControl.Ins.StopFX(SoundFXEnum.Soundname.watersink);
            }
           
            OnCompleteEvent?.Invoke(this);
           
       
        }



    }
}
