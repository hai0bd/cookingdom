using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Link;
using DG.Tweening;
namespace LawNguyen.CookingGame.LRisottoY
{
    public class BowlGlass : ItemIdleBase
    {
        [SerializeField] Clamm clamm;  
        
        public override bool OnTake(IItemMoving item)
        {
            if (item is CrapandChesse crapandChesse && clamm.IsState(Clamm.State.clammovinginplate))
            {
                item.OnMove(TF.position + new Vector3(0, 1f, 0), Quaternion.identity, 0.2f);

                DOVirtual.DelayedCall(0.2f, () =>
                {
                    crapandChesse.ExtractChese();
               
                });
               

                return true;    
            }
            return false;
        }
    }

}


