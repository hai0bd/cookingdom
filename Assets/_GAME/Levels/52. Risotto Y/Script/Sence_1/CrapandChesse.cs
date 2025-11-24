using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Link;
namespace LawNguyen.CookingGame.LRisottoY
{
    public class CrapandChesse : ItemMovingBase
    {
        [SerializeField] CHese chese;
        [SerializeField] GameObject crap;
        [SerializeField] Sprite hintCrapenschesse;


        public void ExtractChese()
        {
            ActiveBoxChese();
            DeActiveBoxCrap();  
            CheseSetparnetNull();   

        }


        public void ActiveBoxChese()
        {   
            chese.Activeboxcolider();
            chese.OrderLayer = 10;
            chese.OnSave(0);
        }
        public void DeActiveBoxCrap()
        {
           collider.enabled = false;
           crap.GetComponent<BoxCollider2D>().enabled = true;
        }
        public void CheseSetparnetNull()
        { 
            chese.Setparentnull();
        }

        public override void OnClickDown()
        {
            base.OnClickDown();
            if (hintCrapenschesse != null) LevelControl.Ins.SetHint(hintCrapenschesse);
        }
    }

}

