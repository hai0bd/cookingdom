using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Link;
using DG.Tweening;
using Unity.Mathematics;

namespace LawNguyen.CookingGame.LRisottoY
{
    public class Potmoving : ItemMovingBase
    {

        public bool iscanmove =false;
        public override bool IsCanMove => iscanmove==true ;
        public Transform ricecookingdone;
       
        public override void OnBack()
        {
            base.OnBack();
        }

        public override void OnDrop()
        {
            base.OnDrop();
            OnBack();   
        }
        
        public void moveRiceInbroad(Vector3 position)
        {

            ricecookingdone.DOMove(position, 0.5f);
            OnDone();
        }



    }

}

