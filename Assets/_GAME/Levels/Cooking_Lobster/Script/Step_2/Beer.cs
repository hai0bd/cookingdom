using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link.Cooking.Lobster
{
    public class Beer : ItemMovingBase
    {
        [SerializeField] Animation animation;
        [SerializeField] ParticleSystem smokeVFX;
        [SerializeField] Transform cap;
        [SerializeField] SpriteRenderer capRenderer;

        public override bool IsCanMove => base.IsCanMove;

        public override void OnClickDown()
        {
            base.OnClickDown();
            capRenderer.DOFade(0, 0.4f).SetDelay(0.2f);
            cap.DOLocalRotate(-125f * Vector3.forward, 0.4f);

            SoundControl.Ins.PlayFX(LevelStep_1.Fx.Click);
        }
        public override void OnClickTake()
        {
            base.OnClickTake();
            smokeVFX.Play();
            SoundControl.Ins.PlayFX(LevelStep_1.Fx.BeerCrack);
        }

        public override void OnDone()
        {
            base.OnDone();
            //anim
            animation.Play();

            SoundControl.Ins.PlayFX(LevelStep_1.Fx.Pour);
            DOVirtual.DelayedCall(1.3f, () =>
            {
                TF.DOMove(TF.position + Vector3.right * 4, .5f).OnComplete(() => gameObject.SetActive(false));
                SoundControl.Ins.PlayFX(LevelStep_1.Fx.Move);
            });
        }

        public override void OnDrop()
        {
            base.OnDrop();
            OnBack();
        }
    }
}