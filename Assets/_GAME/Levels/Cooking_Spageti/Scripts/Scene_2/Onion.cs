using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;


namespace Link.Cooking.Spageti
{
    public class Onion : Tomato
    {
        [SerializeField] Transform[] pieces, targetPieces;
        public override void OnDone()
        {
            base.OnDone();
            for (int i = 0; i < pieces.Length; i++)
            {
                pieces[i].DOLocalMove(targetPieces[i].localPosition, 0.2f);
                pieces[i].DOLocalRotate(targetPieces[i].eulerAngles, 0.2f);
            }

        }

        protected override void CutSound()
        {
            SoundControl.Ins.PlayFX(Fx.Slash);
        }
    }
}