using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Link;
using Satisgame;
using DG.Tweening;
using Unity.VisualScripting.Antlr3.Runtime.Tree;

namespace LawNguyen.CookingGame.LRisottoY
{
    public class Bowlrice : ItemIdleBase
    {

        [SerializeField] ParticleSystem spankelow;
        public override bool IsDone => showSpankelow==true;
        bool showSpankelow=false;    
        public override bool OnTake(IItemMoving item)
        {
            if (item is Potmoving potmoving) 
            {
                potmoving.moveRiceInbroad(TF.position);
                potmoving.OnBack();

      
                DOVirtual.DelayedCall(0.2f, () => spankelow.Play());
                DOVirtual.DelayedCall(0.5f, () => showSpankelow=true);
                DOVirtual.DelayedCall(0.6f, () => LevelControl.Ins.CheckStep());

                return true;    
            }

            return false;
        }



    }
}

