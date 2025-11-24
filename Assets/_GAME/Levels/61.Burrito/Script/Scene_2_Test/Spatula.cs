using HuyThanh.Cooking.Burrito;
using Link;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Hai.Cooking.Burrito
{
    public class Spatula : ItemMovingBase
    {
        [SerializeField] GameObject spriteOnDrop, spriteOnTake;
        [SerializeField] Transform spatulaPoint;
        [SerializeField] Pan pan;

        [SerializeField] GameObject spatulaHiltGO;
        [SerializeField] SortingGroup sortingSpatulaHilt;
        [SerializeField] Transform spatulaHilt;

        [SerializeField] Animation anim;
        [SerializeField] string animTakeSpice;
        [SerializeField] GameObject meatGO;

        private bool haveItem = false;

        public override bool IsCanMove => true;

        Dictionary<Collider2D, SpiceItemFry> items = new Dictionary<Collider2D, SpiceItemFry>();

        public override void OnClickDown()
        {
            if (!haveItem)
            {
                ShowOnTake(true);
            }
            base.OnClickDown();
            SoundControl.Ins.PlayFX(Fx.Take);
            OrderLayer = 25;
            sortingSpatulaHilt.sortingOrder = 40;

        }

        public override void OnDrop()
        {
            if (!haveItem)
            {
                ShowOnTake(false);
            }
            base.OnDrop();
            SoundControl.Ins.PlayFX(Fx.PutDown);
            sortingSpatulaHilt.sortingOrder = 0;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.TryGetComponent(out BowlMeat bowlMeat) && haveItem)
            {
                haveItem = false;
                meatGO.SetActive(false);
                bowlMeat.TakeMeat();
                isBackWhenDrop = true;
                return;
            }
            if(!pan.IsState(Pan.State.Mixing1) && !pan.IsState(Pan.State.Mixing2) && !pan.IsState(Pan.State.DoneMix2))
            {
                return;
            }
            if(collision.TryGetComponent(out Pan panMix) && pan.IsState(Pan.State.DoneMix2) && !haveItem)
            {
                haveItem = true;
                isBackWhenDrop = false;
                meatGO.SetActive(true);
                anim.Play(animTakeSpice);
                panMix.TakeMeat();
            }
            if(!items.ContainsKey(collision))
            {
                items.Add(collision, collision.GetComponent <SpiceItemFry>());
            }

            if (items.ContainsKey(collision) && items[collision] != null)
            {
                items[collision].AddForce(this, (items[collision].transform.position - spatulaPoint.position).normalized);
                if(!SoundControl.Ins.IsPlaying(Fx.SpoonFry))
                {
                    SoundControl.Ins.PlayFX(Fx.SpoonFry);
                }
            }
        }

        public void ShowOnTake(bool isShow)
        {
            spriteOnDrop.SetActive(isShow);
            spriteOnTake.SetActive(!isShow);
            spatulaHiltGO.SetActive(isShow);
        }

        private void LateUpdate()
        {
            spatulaHilt.SetPositionAndRotation(TF.position, TF.rotation);
            spatulaHilt.localScale = TF.localScale;
            if (LevelControl.Ins.IsHaveObject<Pan>(spatulaPoint.position))
            {
                pan.SetCookingRate(spatulaHilt.position);
            }
        }
    }
}
