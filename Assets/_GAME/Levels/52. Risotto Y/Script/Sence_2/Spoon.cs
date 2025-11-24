using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Link;

namespace LawNguyen.CookingGame.LRisottoY
{
    public class Spoon : ItemMovingBase
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

