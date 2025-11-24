using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Assertions.Must;

namespace Link.Cooking.Spageti_Bear
{
    public class Scoop : ItemMovingBase
    {
        [SerializeField] private Transform scoopPoint;

        public Noodle noodle;
        public bool IsNoodle = false;

        public override void OnDrop()
        {
            base.OnDrop();
            OnBack();
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.TryGetComponent(out Noodle noodle) && !IsNoodle)
            {
                this.noodle = noodle;
                IsNoodle = true;
                noodle.OrderLayer = 50;
                noodle.TF.SetParent(scoopPoint);
                noodle.TF.DOLocalMove(Vector3.zero, 0.4f);
            }
        }
    }
}