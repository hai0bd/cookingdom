using Link;
using UnityEngine;

namespace HuyThanh.Cooking.Burrito
{

    public class Punctured : ItemMovingBase
    {
        public enum SpiceType
        {
            None,
            Bean,
            Corn,
        }
        public override bool IsCanMove => true;

        [SerializeField] private GameObject spriteOnTake;
        [SerializeField] private GameObject spriteOnDrop;

        [Header("Bean and Corn Sprite")]
        [SerializeField] private GameObject beanSprite;
        [SerializeField] private GameObject cornSprite;

        [SerializeField] Animation anim;
        [SerializeField] string animTakeSpice;

        private SpiceType currentSpiceType = SpiceType.None;

        public override void OnClickDown()
        {
            SoundControl.Ins.PlayFX(Fx.Take);
            ShowOnTake(isShow: true);
            base.OnClickDown();
        }

        public override void OnDrop()
        {
            if (currentSpiceType == SpiceType.None)
            {
                ShowOnTake(isShow: false);
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
                        ///show take spice sprite
                        ///
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
            }

            if (collision.TryGetComponent(out BowlPotItem bowlPotItem))
            {
                if (bowlPotItem.IsState(BowlPotItem.State.WaitingCookSpice) && currentSpiceType != SpiceType.None)
                {
                    SoundControl.Ins.PlayFX(Fx.Take);
                    currentSpiceType = SpiceType.None;
                    bowlPotItem.ActiveSpirte();
                    beanSprite.SetActive(false);
                    cornSprite.SetActive(false);
                    isBackWhenDrop = true;
                }
            }

        }
    }
}
