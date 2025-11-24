using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Link;
namespace LawNguyen.CookingGame.LRisottoY
{
    public class ParsleySpinkle : ItemMovingBase
    {
        [SerializeField] GameObject parsleySpinkle;
        public void Activebox()
        {
         collider.enabled = true;  
        }
        public void Deactivegameobject()
        {
            collider.enabled = false;
            parsleySpinkle.SetActive(false);
        }


    }
}

