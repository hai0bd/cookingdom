using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Link.Cooking.Lobster
{
    public class DecoreItem : ItemMovingBase
    {
        public enum NameType { Piece_1, Piece_2, Tomato, Lettuce, Lemon, WoodBowl, Cucumber }
        [field: SerializeField] public DecoreItem.NameType Name { get; private set; }

        public override bool IsCanMove => base.IsCanMove;

        public UnityEvent<DecoreItem> OnDoneEvent;

        protected override void Start()
        {
            base.Start();
            collider.enabled = false;
            Invoke(nameof(EnableCollider), delaySaveTime);
        }

        private void EnableCollider()
        {
            collider.enabled = true;
        }

        public override void OnDone()
        {
            base.OnDone();
            OnDoneEvent?.Invoke(this);
        }

        public override void OnClickDown()
        {
            base.OnClickDown();
            TF.DORotate(Vector3.zero, 0.2f);
            SoundControl.Ins.PlayFX(LevelStep_1.Fx.Click);
        }

        public override void OnClickTake()
        {
            base.OnClickTake();
            SoundControl.Ins.PlayFX(LevelStep_1.Fx.Take);
        }

        public override void OnDrop()
        {
            base.OnDrop();
            MoveLean();
        }

        public void MoveLean()
        {
            TF.DORotate(Vector3.forward * Random.Range(1, 3) * Mathf.Sign(Random.Range(-1, 1)) * 15, 0.2f);
        }
    }
}