using DG.Tweening;
using Link;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace LawNguyen.CookingGame.LRisottoY
{
    public class PasleySence4 : ItemMovingBase
    {
        [SerializeField] SpriteRenderer[] parsleyAr;

        public void Dofadepassley()
        {
            foreach (var par in parsleyAr)
            {
                par.DOFade(0f, 0.5f);
            }
        }
    }
}

