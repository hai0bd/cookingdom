using System.Collections;
using System.Collections.Generic;
using AnhPD;
using DG.Tweening;
using UnityEngine;

namespace Link.Cooking.Spageti
{
    public class Mushroom : ItemMovingBase
    {
        public enum MushroomType
        {
            Mushroom0,
            Mushroom1,
            Mushroom2,
            Mushroom3,
            Mushroom4,
            Mushroom5,
        }

        [field:SerializeField] public MushroomType Type { get; private set; }
        [SerializeField] Floating2D floating2D;

        [SerializeField] Bear bear;
        [SerializeField] ParticleSystem sparkVFX;

        public override bool IsDone => !collider.enabled;

        public override void OnDone()
        {
            base.OnDone();
            TF.DOScale(0.9f, 0.2f);

            LevelControl.Ins.CheckStep(0.1f);
        }

        public override void OnDrop()
        {
            base.OnDrop();
            TF.DORotate(Utilities.TakeRandom(-2, -1, 1, 2) * Vector3.forward * 15, 0.2f);
            floating2D.Active(0.2f);
        }

        public override void OnClickTake()
        {
            base.OnClickTake();
            SoundControl.Ins.PlayFX(Fx.Take);
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

        public override void SetControl(bool isControl)
        {
            base.SetControl(isControl);
            if(!IsDone && isControl)
            {
                sparkVFX.Play();
            }
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