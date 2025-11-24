using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

namespace Link.Cooking.Spageti
{
    public class Spoon : ItemMovingBase
    {
        public enum State { Normal, Salt, Pouring, Done }
        [SerializeField] State state;
        [SerializeField] Transform spoonPoint;
        [SerializeField] SortingGroup sortingHiltSpoon;
        [SerializeField] Transform spoonHilt;
        [SerializeField] CircleCollider2D ccollider2D;
        public override bool IsCanMove => IsState(State.Normal);

        private MeatDecor meatDecor;

        public bool IsHaveMeat => meatDecor != null;

        PotFry potFry;

        Dictionary <Collider2D, ItemFry> items = new Dictionary<Collider2D, ItemFry>();

        public override void ChangeState<T>(T t)
        {
           state = (State)(object)t;

           switch (state)
           {
               case State.Normal:
                   //active anim
                   break;
               case State.Salt:
                    //active anim
                    break;
               case State.Pouring:
                    OnDone();
                   //active anim
                   break;
               case State.Done:
                   OnBack();
                   break;
           }
        }
        public override bool IsState<T>(T t)
        {
           return state == (State)(object)t;
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if(!items.ContainsKey(collision))
            {
                items.Add(collision, collision.GetComponent<ItemFry>());
            }
            if (items.ContainsKey(collision) && items[collision] != null)
            {
                items[collision].AddForce(this, (items[collision].transform.position - spoonPoint.position).normalized);
                if(!SoundControl.Ins.IsPlaying(Fx.SpoonFry))
                {
                    SoundControl.Ins.PlayFX(Fx.SpoonFry);
                }
            }
            else
            if (meatDecor == null && collision.TryGetComponent(out meatDecor))
            {
                meatDecor.TF.SetParent(spoonPoint);
                meatDecor.TF.DOLocalMove(Vector3.zero, 0.2f);
                meatDecor.TF.DOLocalRotate(Vector3.zero, 0.2f);
                meatDecor.TF.DOScale(.9f, 0.2f);
                spoonHilt.gameObject.SetActive(false);
            }
        }

        public override void OnClickDown()
        {
            base.OnClickDown();
            OrderLayer = 25;
            TF.DORotate(Vector3.forward * 15, 0.2f);
            sortingHiltSpoon.sortingOrder = 40;
        }

        public override void OnDrop()
        {
            base.OnDrop();
            OnBack();
            TF.DORotate(Vector3.forward * -13.6f, 0.2f);
            sortingHiltSpoon.sortingOrder = 0;

            LevelControl.Ins.CheckStep(0.5f);
        }

        public override void OnDone()
        {
            base.OnDone();
            meatDecor.OnDone();
            ccollider2D.enabled = false;
        }

        void LateUpdate()
        {
            spoonHilt.SetPositionAndRotation(TF.position, TF.rotation);
            spoonHilt.localScale = TF.localScale;

            if (LevelControl.Ins.IsHaveObject<PotFry>(spoonPoint.position, out potFry))
            {
                potFry.SetCookingRate(spoonPoint.position);
            }
        }

        internal bool IsCollapse(Vector3 position, float radius)
        {
            return Vector3.Distance(position, TF.position) <= radius + 0.4f;
        }
    }
}
