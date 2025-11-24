using DG.Tweening;
using Link;
using Satisgame;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace HuyThanh.Cooking.TunaSaladFrench
{
    public class MortarSpice : ItemIdleBase
    {
        public enum State { Normal, Mix, Spice, Done }
        [SerializeField] State state;

        [BoxGroup("Anim Name")][SerializeField] Animation anim;
        [BoxGroup("Anim Name")][SerializeField] string animMix;

        [SerializeField] ItemAlpha origin, mixed;

        [SerializeField] ParticleSystem vfxBlink;
        [SerializeField] ItemMovingBase coriander; // mix xong thi cho rau mui vao
        /// game chi tang khi ma da cho rau mui vao

        [SerializeField] List<ItemMovingBase> orders;
        [SerializeField] EmojiControl _emojiControl;

        [SerializeField] Sprite hint_0, hint_1;
        private Pestle _pestle;

        public override bool OnTake(IItemMoving item)
        {

            if (orders.Count > 0 && IsState(MortarSpice.State.Normal))
            {
                if (item is Wine wine && orders[0].Equals(item))
                {
                    orders.RemoveAt(0);
                    item.ChangeState(Wine.State.Pouring);

                    if (wine.IsType(Wine.Type.Wine))
                    {
                        DOVirtual.DelayedCall(0.1f, () => SoundControl.Ins.PlayFX(Fx.WinePour));
                    }
                    else if (wine.IsType(Wine.Type.Oil))
                    {
                        DOVirtual.DelayedCall(0.1f, () => SoundControl.Ins.PlayFX(Fx.OilPour));
                    }
                    else if (wine.IsType(Wine.Type.Pepper))
                    {
                        DOVirtual.DelayedCall(0.1f, () => SoundControl.Ins.PlayFX(Fx.PepperPour));
                    }
                    item.OnMove(TF.position + Vector3.right * 0.1f, Quaternion.identity, 0.2f);
                    return true;
                }

                if (item is Tuna && item.IsState(Tuna.State.Spice) && orders[0].Equals(item))
                {
                    orders.RemoveAt(0);
                    item.ChangeState(Tuna.State.Pouring);
                    item.OnMove(TF.position + Vector3.right * 0.1f, Quaternion.identity, 0.2f);
                    return true;
                }

                if (item is Garlic && orders[0].Equals(item) && item.IsState(Garlic.State.Spice))
                {
                    orders.RemoveAt(0);
                    item.ChangeState(Garlic.State.Done);
                    item.OnMove(TF.position + Vector3.right * 0.1f, Quaternion.identity, 0.2f);
                    return true;
                }

                if (item is Spoon && orders[0].Equals(item) && item.IsState(Spoon.State.HavingSpice))
                {
                    orders.RemoveAt(0);
                    item.ChangeState(Spoon.State.Pouring);
                    item.OnMove(TF.position + Vector3.right * 0.1f + Vector3.up * 0.1f, Quaternion.identity, 0.2f);
                    return true;
                }

                if (item is ItemSpice && orders[0].Equals(item) && item.IsState(ItemSpice.State.Spice))
                {
                    if (item.TF.name == "Capers")
                    {
                        LevelControl.Ins.SetHint(hint_1);
                    }
                    orders.RemoveAt(0);
                    item.ChangeState(ItemSpice.State.Done);
                    item.OnMove(TF.position + Vector3.right * 0.1f, Quaternion.identity, 0.2f);
                    return true;
                }

                if (item is Pestle && orders[0].Equals(item) && item.IsState(Pestle.State.Normal))
                {
                    orders.RemoveAt(0);
                    item.ChangeState(Pestle.State.Mixing);
                    item.OnMove(TF.position + Vector3.right * 0.1f + Vector3.up * 0.3f, Quaternion.identity, 0.2f);

                    this._pestle = item as Pestle;
                    ChangeState(MortarSpice.State.Mix);
                    return true;
                }
            }
            else if (IsState(MortarSpice.State.Spice))
            {
                if (item is Herb && item.Equals(coriander) && item.IsState(Herb.State.Spice))
                {
                    _emojiControl.ShowPositive();
                    item.OnMove(TF.position + Vector3.right * 0.1f, Quaternion.identity, 0.2f);
                    item.ChangeState(Herb.State.Done);
                    ChangeState(MortarSpice.State.Done);
                    return true;
                }
            }

            return base.OnTake(item);
        }

        public override void ChangeState<T>(T t)
        {
            state = (State)(object)t;
            switch (state)
            {
                case MortarSpice.State.Mix:
                    anim.Play(animMix);

                    foreach (AnimationState state in anim)
                    {
                        state.speed = 0;
                    }
                    break;

                case MortarSpice.State.Done:
                    _emojiControl.ShowPositive();
                    LevelControl.Ins.CheckStep(1f);
                    break;
                default:

                    break;
            }
        }

        public override void OnClickDown()
        {
            base.OnClickDown();
            if (IsState(State.Mix))
            {
                PlayAnimOnClick();
                _pestle.PlayAnimOnClick();
                _pestle.OrderLayer = 51;
            }
        }

        Tween current_tween = null;
        public void PlayAnimOnClick()
        {
            if (current_tween != null)
                current_tween.Kill();

            SoundControl.Ins.PlayFX(Fx.MortarPestle);

            foreach (AnimationState state in anim)
            {
                state.speed = 1;
            }

            if (anim.isPlaying == false)
            {
                ChangeState(State.Spice);
                vfxBlink.Play();
                SoundControl.Ins.StopFX(Fx.MortarPestle);
                _pestle.ChangeState(Pestle.State.Done);
                return;
            }
            current_tween =
            DOVirtual.DelayedCall(0.2f, () =>
            {
                SoundControl.Ins.StopFX(Fx.MortarPestle);
                foreach (AnimationState state in anim)
                {

                    state.speed = 0;
                }
            });
        }

        public override bool IsState<T>(T t)
        {
            return state == (State)(object)t;
        }

        [SerializeField] Transform sauceTF;

        public void ScaleDownSauce()
        {
            sauceTF.DOScale(.9f, 0.2f);
        }
    }

}
