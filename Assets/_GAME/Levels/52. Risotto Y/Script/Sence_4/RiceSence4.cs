using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Link;
namespace LawNguyen.CookingGame.LRisottoY
{
    public class RiceSence4 : ItemMovingBase
    {
        [SerializeField] Animation animation;

       public void DropriceAim()
        {
            animation.Play();   
        }

    }

}

