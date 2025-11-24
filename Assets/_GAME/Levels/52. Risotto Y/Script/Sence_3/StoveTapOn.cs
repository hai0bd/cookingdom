using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Link;
using System;
using Satisgame;
namespace LawNguyen.CookingGame.LRisottoY
{
    public class StoveTapOn : ItemIdleBase
    {
        [SerializeField] Plateend[] plateendAR;
        [SerializeField] EmojiControl emoji;
        public bool Ison =false;
        public GameObject stoveTapon;

        public Pot3 pot3;
        public event Action PotisTurnedOn;
        public override bool IsDone => !Ison && platedone;
        public  int platedoneint =0;  
        public bool platedone=false;


        public override void OnClickDown()
        {
            Ison=!Ison;
            pot3.PotIsOn(Ison);
            stoveTapon.SetActive(Ison);
            foreach (var plateend in plateendAR)
            {
                if (plateend.IsDone) platedoneint++;
                if(platedoneint==3) platedone=true;    
            }
            if (IsDone) emoji.ShowPositive();

            PotisTurnedOn?.Invoke();
           
           LevelControl.Ins.CheckStep();

            
          
        }

        




    }
}
