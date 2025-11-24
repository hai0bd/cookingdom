using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Link;
namespace LawNguyen.CookingGame.LRisottoY
{
    public class spicecontaine : ItemMovingBase
    {   
        public enum TypeSpice {salt,peper };
       [SerializeField] public TypeSpice typeSpice; 
        [SerializeField] private Animation animation;

        public void Pour()
        {
            animation.Play();   
        }


    }

}


