using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Link.Cooking.Lobster
{
    public class SteamDish : ItemMovingBase
    {
        public override bool IsCanMove => base.IsCanMove;
        [SerializeField] ActionAnim anim;
        [SerializeField] Transform dish;

        public override void OnClickDown()
        {
            base.OnClickDown();
            SoundControl.Ins.PlayFX(LevelStep_1.Fx.Click);
        }

        public override void OnClickTake()
        {
            base.OnClickTake();
            OrderLayer -= 10;
            SoundControl.Ins.PlayFX(LevelStep_1.Fx.Take);
        }

        public override void OnDone()
        {
            base.OnDone();
            dish.DOLocalMoveY(0.323f, 0.1f);
            anim.Active();
        }
    }
}