using HuyThanh.Cooking.Burrito;
using Link;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hai.Cooking.Burrito {
    public class BowlMeat : ItemIdleBase
    {
        [SerializeField] Transform meatGO;

        [SerializeField] Animation anim;
        [SerializeField] string animTakeMeat;
        [SerializeField] HintText hintText_doneTakeMeat;

        private float[] sizeFloat = { .5f, .75f, 1f };
        private int currentSizeIndex = 0;

        public void TakeMeat()
        {
            anim.Play(animTakeMeat);
            meatGO.localScale = Vector3.one * sizeFloat[currentSizeIndex];
            currentSizeIndex++;
            SoundControl.Ins.PlayFX(Fx.Click);
            if (currentSizeIndex == 3) 
                hintText_doneTakeMeat.OnActiveHint();
        }

        public bool CheckDone()
        {
            return currentSizeIndex == 3;
        }
    }
}