using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Link.Cooking.Spageti
{
    public class FlowerPack : ItemMovingBase
    {
        [SerializeField] Floating2D floating2D;
        [SerializeField] Collider2D subCollider;
        [SerializeField] Transform square, subPoint;
        [SerializeField] ParticleSystem vfx;
        [SerializeField] Bear bear;

        [SerializeField] AudioClip audioClip;

        public override bool OnTake(IItemMoving item)
        {
            if (item is FlowerPack flower && flower != this)
            {
                item.TF.SetParent(square);
                item.TF.DOLocalMove(subPoint.localPosition, 0.2f);
                item.TF.DOLocalRotate(subPoint.localRotation.eulerAngles, 0.2f);
                item.OnDone();
                subCollider.enabled = true;
                IsDone = true;
                //them vfx
                vfx.Play();
                SoundControl.Ins.PlayFX(audioClip);
                return true;
            }
            return false;
        }

        public override void OnDrop()
        {
            base.OnDrop();
            TF.DORotate(Utilities.TakeRandom(-2, -1, 1, 2) * Vector3.forward * 15, 0.2f);
            floating2D.Active(0.2f);
        }

        public override void OnClickDown()
        {
            base.OnClickDown();
            floating2D.OnStop();
            TF.DORotate(Vector3.zero, 0.2f);

            if (bear != null && bear.IsState(Bear.State.Hide))
            {
                bear.ChangeState(Bear.State.Start);
            }
            SoundControl.Ins.PlayFX(Fx.Click);
        }

        override public void OnClickTake()
        {
            base.OnClickTake();
            SoundControl.Ins.PlayFX(Fx.Take);
        }

        public override void OnDone()
        {
            if(IsDone)
            {
                gameObject.SetActive(false);
            }
            base.OnDone();
        }

        protected override void Editor()
        {
            base.Editor();
            if (floating2D == null)
            {
                floating2D = GetComponent<Floating2D>();
            }
        }


    }
}