using Link;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Hai.Cooking.NewTest
{
    public class Spatula : ItemMovingBase
    {
        [SerializeField] GameObject spriteOnTake, spriteOnDrop;

        [SerializeField] Transform spatulaPoint;
        [SerializeField] Pan pan;

        [SerializeField] private Transform spatulaHilt;
        [SerializeField] private GameObject spatulaHiltGO;
        [SerializeField] private SortingGroup sortingSpatulaHilt;

        private bool haveItem = false;

        private Dictionary<Collider2D, SpiceItemFry> items = new Dictionary<Collider2D, SpiceItemFry>();
        public override bool IsCanMove => true;

        public override void OnClickDown()
        {
            if (!haveItem)  ShowOnTake(true);
            SoundControl.Ins.PlayFX(Fx.Take);
            base.OnClickDown();

            OrderLayer = 25;
            sortingSpatulaHilt.sortingOrder = 40;
        }

        public override void OnDrop()
        {
            SoundControl.Ins.PlayFX(Fx.PutDown);
            if (!haveItem) ShowOnTake(false);
            sortingSpatulaHilt.sortingOrder = 0;
            base.OnDrop();
        }

        public void ShowOnTake(bool isShow)
        {
            spriteOnTake.SetActive(isShow);
            spriteOnDrop.SetActive(!isShow);
            spatulaHiltGO.SetActive(isShow);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.TryGetComponent(out Pan panMix) && pan.IsState(Pan.State.Mixing) && !haveItem)
            {
                haveItem = true;
                panMix.TakeMeat();
            }
            if(!items.ContainsKey(collision))
            {
                items.Add(collision, collision.GetComponent<SpiceItemFry>());
            }
            if(items.ContainsKey(collision) && items[collision] == null)
            {
                items[collision].AddForce(this, (transform.position - spatulaPoint.position).normalized);
                if(!SoundControl.Ins.IsPlaying(Fx.SpoonFry))
                {
                    SoundControl.Ins.PlayFX(Fx.SpoonFry);
                }
            }
        }

        private void LateUpdate()
        {
            spatulaHilt.SetPositionAndRotation(TF.position, TF.rotation);
            spatulaHilt.localScale = TF.localScale;

            if(LevelControl.Ins.IsHaveObject<Pan>(spatulaPoint.position))
            {
                pan.SetCookingRate();
            }
        }
    }
}