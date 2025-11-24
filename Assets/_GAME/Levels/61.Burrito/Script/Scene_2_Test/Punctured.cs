using HuyThanh.Cooking.Burrito;
using Link;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hai.Cooking.Burrito
{
    public class Punctured : ItemMovingBase
    {
        public enum SpiceType
        {
            None = 0,
            Bean = 1,
            Corn = 2,
        }

        public override bool IsCanMove => true;

        [SerializeField] private GameObject spriteOnTake;
        [SerializeField] private GameObject spriteOnDrop;

        [SerializeField] private GameObject beanSprite;
        [SerializeField] private GameObject cornSprite;
        [SerializeField] private Animation anim;
        [SerializeField] private string animTakeSpice;

        private SpiceType currentSpiceType;


        public override void OnClickDown()
        {
            SoundControl.Ins.PlayFX(Fx.Click);
            ShowOnTake(true);
            base.OnClickDown();
        }

        public override void OnDrop()
        {
            if (currentSpiceType == SpiceType.None)
            {
                ShowOnTake(false);
            }
            SoundControl.Ins.PlayFX(Fx.PutDown);
            base.OnDrop();
        }

        public void ShowOnTake(bool isShow)
        {
            spriteOnTake.SetActive(isShow);
            spriteOnDrop.SetActive(!isShow);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out Pot pot))
            {
                if (pot.IsState(Pot.State.DoneCook) && currentSpiceType == SpiceType.None)
                {
                    currentSpiceType = pot.GetSpiceType();
                    if (pot.OnTakeSpice())
                    {
                        anim.Play(animTakeSpice);
                        isBackWhenDrop = false;

                        switch (currentSpiceType)
                        {
                            case SpiceType.Bean:
                                beanSprite.SetActive(true);
                                cornSprite.SetActive(false);
                                break;
                            case SpiceType.Corn:
                                beanSprite.SetActive(false);
                                cornSprite.SetActive(true);
                                break;
                        }
                    }
                }

                if (collision.TryGetComponent(out BowlPotItem bowlPotItem))
                {
                    if (bowlPotItem.IsState(BowlPotItem.State.WaitingCookSpice) && currentSpiceType == SpiceType.None)
                    {
                        SoundControl.Ins.PlayFX(Fx.Take);
                        currentSpiceType = SpiceType.None;
                        bowlPotItem.ActiveSprite();
                        beanSprite.SetActive(false);
                        cornSprite.SetActive(false);
                        isBackWhenDrop = true;
                    }
                }
            }
        }
    }
}