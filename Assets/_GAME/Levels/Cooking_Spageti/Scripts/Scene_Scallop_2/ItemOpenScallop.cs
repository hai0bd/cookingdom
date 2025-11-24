using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link.Cooking.Spageti
{
    public class ItemOpenScallop : ItemMovingBase
    {
        public Scallop.ItemType itemType;
        [SerializeField] Animation anim;

        [SerializeField] ItemAlpha itemAlpha;
        [SerializeField] AudioClip clip;
        [SerializeField] HintText hintText;
        public float delayBack;
        public Vector3 offset;
        public Vector3 rotate;
        public Emoji.EmojiType emojiType;

        private bool isDone = false;
        public override bool IsDone => isDone;

        public override void OnDone()
        {
            // base.OnDone();
            isDone = true;
            if (anim != null)
            {
                anim.Play();
            }

            if (delayBack > 0 && !IsType(Scallop.ItemType.Sticker, Scallop.ItemType.Flower)) Invoke(nameof(OnDrop), delayBack);

            collider.enabled = false;

            if (clip != null)
            {
                SoundControl.Ins.PlayFX(clip);
            }
            
            hintText.OnActiveHint();
        }

        public bool IsType(params Scallop.ItemType[] types)
        {
            foreach (var type in types)
            {
                if (itemType == type) return true;
            }
            return false;
        }

        public override void OnDrop()
        {
            base.OnDrop();
            OnBack();
            if (!IsType(Scallop.ItemType.Sticker, Scallop.ItemType.Flower))
            {
                collider.enabled = true;
            }

            if(clip != null)
            {
                SoundControl.Ins.StopFX(clip);
            }
        }

        public void DoAlpha(float alpha, float time)
        {
            itemAlpha.DoAlpha(alpha, time);
        }

        public override void OnClickDown()
        {
            base.OnClickDown();
            SoundControl.Ins.PlayFX(Fx.Click);
        }

        public override void OnClickTake()
        {
            base.OnClickTake();
            SoundControl.Ins.PlayFX(Fx.Take);
        }

    }
}
