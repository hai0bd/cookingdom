using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Link;
using UnityEditor.Tilemaps;
using Link.Cooking.Spageti;

namespace HoangLinh.Cooking.Test
{
    public class Spatula : ItemMovingBase
    {
        [SerializeField] GameObject spriteInBox, spriteOnClick;
        [SerializeField] Transform spatulaPoint;

        [SerializeField] Pan pan;

        [SerializeField] Animation anim;
        [SerializeField] string animTakeSpice;
        [SerializeField] GameObject meatGO;

        private bool haveItem = false;
        public override bool IsCanMove => true;

        Dictionary<Collider2D, SpiceItemFry> items = new Dictionary<Collider2D, SpiceItemFry>();

        private void OnTriggerEnter2D(Collider2D collision)
        {
            
            if(!pan.IsState(Pan.State.FirstMix) && !pan.IsState(Pan.State.SecondMix) && !pan.IsState(Pan.State.DoneMix))
            {
                return;
            }

            if (!items.ContainsKey(collision))
            {
                items.Add(collision, collision.GetComponent<SpiceItemFry>());

            }
            if (items.ContainsKey(collision) && items[collision] != null)
            {
                items[collision].AddForce(this, (items[collision].transform.position - spatulaPoint.position).normalized);
            }

            Debug.Log($"[Spatula] OnTriggerEnter2D with: {collision.name}", gameObject);

        }


        private void LateUpdate()
        {
            // spatulaHilt.SetPositionAndRotation(TF.position, TF.rotation);
            // spatulaHilt.localScale = TF.localScale;
            
            if (LevelControl.Ins.IsHaveObject<Pan>(spatulaPoint.position))
            {
                pan.SetCookingRate(spatulaPoint.position);
            }
        }
        

        public override void OnClickDown()
        {
            base.OnClickDown();
            ShowOnClick(true);
        }

        public void ShowOnClick(bool isShow)
        {
            spriteInBox.SetActive(!isShow);
            spriteOnClick.SetActive(isShow);
        }

        public override void OnDrop()
        {
            base.OnDrop();
            ShowOnClick(false);
        }


    }
}