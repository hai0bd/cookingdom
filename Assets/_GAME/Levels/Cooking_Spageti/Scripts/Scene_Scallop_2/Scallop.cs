using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Link;
using UnityEngine;

namespace Link.Cooking.Spageti
{
    public class Scallop : ItemIdleBase
    {
        public enum State { Dirty, Step_1, Step_2, Done }

        public enum ItemType { Perfume, Massage, Flower, Fearther, Sticker, Pepper }
        [SerializeField] State state;
        [SerializeField] List<ItemType> itemTypes;
        [SerializeField] GameObject[] scallopGOs;
        [SerializeField] Animation anim;
        [SerializeField] Emoji emoji;

        [SerializeField] ParticleSystem blinkVFX, step1VFX, smokeVFX;

        List<ItemOpenScallop> items = new List<ItemOpenScallop>();

        public override bool IsDone => IsState(State.Done);

        public override void ChangeState<T>(T t)
        {
            state = (State)(object)t;
        }
        public override bool IsState<T>(T t)
        {
            return state == (State)(object)t;
        }

        public override bool OnTake(IItemMoving item)
        {
            if (IsState(State.Dirty, State.Done) && item is not ItemBack)
            {
                anim.Play("ItemShakeLight");
                LevelControl.Ins.LoseHalfHeart(TF.position);
                return false;
            }

            if (item is ItemOpenScallop itemOpen)
            {
                if (itemOpen.itemType == itemTypes[0])
                {
                    DoneItem(itemOpen);
                    if (IsState(State.Step_1))
                    {
                        ChangeState(State.Dirty);
                        DOVirtual.DelayedCall(itemOpen.delayBack, () =>
                        {
                            ChangeState(State.Step_2);
                            ActiveIndex(1);
                            anim.Play("ItemScaleLight");
                            //them vfx
                            step1VFX.Play();
                            SoundControl.Ins.PlayFX(Fx.Blink);
                        });

                        anim.Play("ItemScaleLight");
                        return true;
                    }
                    if (IsState(State.Step_2))
                    {
                        ChangeState(State.Dirty);
                        //them vfx
                        StartCoroutine(IEDone(itemOpen.delayBack));
                        anim.Play("ItemScaleLight");
                        return true;
                    }
                }
                else
                {
                    anim.Play("ItemShakeLight");
                    LevelControl.Ins.LoseHalfHeart(TF.position);
                }
            }

            return false;
        }

        private void DoneItem(ItemOpenScallop itemOpen)
        {
            itemTypes.RemoveAt(0);
            itemOpen.OnMove(TF.position + itemOpen.offset, Quaternion.Euler(itemOpen.rotate), 0.2f);
            if (itemOpen.itemType != ItemType.Sticker && itemOpen.itemType != ItemType.Flower)
                itemOpen.OnDone();

            if (itemOpen.itemType == ItemType.Sticker || itemOpen.itemType == ItemType.Flower)
            {
                items.Add(itemOpen);
                itemOpen.TF.SetParent(TF.GetChild(0));
                itemOpen.OnDone();
            }
            emoji.PlayEmoji(itemOpen.emojiType, 2f);
        }

        private void ActiveIndex(int index)
        {
            scallopGOs[index - 1].SetActive(false);
            scallopGOs[index].SetActive(true);
        }

        private IEnumerator IEDone(float delay)
        {
            yield return new WaitForSeconds(delay);
            foreach (var item in items)
            {
                item.DoAlpha(0, 0.5f);
            }
            //dien canh anim
            yield return new WaitForSeconds(0.75f);
            ActiveIndex(2);
            anim.Stop();
            anim.Play("ItemScaleLight");
            yield return new WaitForSeconds(0.3f);
            ActiveIndex(3);
            anim.Stop();
            anim.Play("ItemScaleLight");
            yield return new WaitForSeconds(0.2f);
            smokeVFX.Play();
            yield return new WaitForSeconds(0.2f);
            ActiveIndex(4);
            step1VFX.Play();
            anim.Stop();
            anim.Play("ItemScaleLight");
            SoundControl.Ins.PlayFX(Fx.Blink);

            yield return new WaitForSeconds(1.25f);
            ChangeState(State.Done);
            LevelControl.Ins.CheckStep();
        }

        public void OnClean()
        {
            IfChangeState(State.Dirty, State.Step_1);
            blinkVFX.Play();
            SoundControl.Ins.PlayFX(Fx.Blink);
            LevelControl.Ins.CheckStep();
        }

        public void EmojiAngry()
        {
            emoji.PlayEmoji(Emoji.EmojiType.Angry, 1.5f);
        }

    }
}