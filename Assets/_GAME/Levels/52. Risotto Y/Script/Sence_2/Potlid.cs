using DG.Tweening;
using Link;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LawNguyen.CookingGame.LRisottoY
{
    public class Potlid : ItemMovingBase
    {

        

        public override void OnBack()
        {
            base.OnBack();
        }
        public override void OnDrop()
        {
            base.OnDrop();
            OnBack();

        }

    }
}

