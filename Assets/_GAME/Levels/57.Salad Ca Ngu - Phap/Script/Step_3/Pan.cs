using DG.Tweening;
using Link;
using Satisgame;
using System.Collections;
using UnityEngine;

namespace HuyThanh.Cooking.TunaSaladFrench
{
    public class Pan : ItemIdleBase
    {
        public enum State
        {
            Normal,
            HaveOil,
            Hot,
            Cooking,
            Cooked,
            Done
        }
        /// <summary>
        /// state : -> chua co gi-> do dau vao-> bat bep -> cho do an -> nau xong -> tat bep la done
        /// </summary>

        [SerializeField] private State state;

        [SerializeField] private EmojiControl _emojiControl;
        [SerializeField] private ItemAlpha oilLayer;
        [SerializeField] private ItemAlpha oilBoil;
        [SerializeField] private ParticleSystem vfxOilBoil;
        [SerializeField] ClockTimer timer;

        [SerializeField] Sprite hint;

        private Coroutine boilSound;

        public override bool IsState<T>(T t)
        {
            return state == (State)(object)t;
        }

        public override void ChangeState<T>(T t)
        {
            state = (State)(object)t;

            switch (state)
            {
                case State.HaveOil:

                    SoundControl.Ins.PlayFX(Fx.OilPour);
                    oilLayer.DoAlpha(1f, 0.5f, 0.5f);
                    break;
                case State.Hot:
                    boilSound = StartCoroutine(BoilSound());
                    oilBoil.DoAlpha(1f, 2f);
                    DOVirtual.DelayedCall(2f, () => vfxOilBoil.Play());
                    break;
                case State.Cooked:
                    LevelControl.Ins.SetHint(hint);///hint cho buoc sau
                    break;
                case State.Done:

                    _emojiControl.ShowPositive();

                    StopCoroutine(boilSound);
                    SoundControl.Ins.StopFX(Fx.OilBoil);
                    oilBoil.DoAlpha(0f, .5f);
                    vfxOilBoil.Stop();
                    MiniGame5.Instance.CheckDone(0.2f);
                    break;
            }
        }

        IEnumerator BoilSound()
        {
            while (true)
            {
                SoundControl.Ins.PlayFX(Fx.OilBoil);
                yield return WaitForSecondCache.Get(10f);
                SoundControl.Ins.StopFX(Fx.OilBoil);
            }
        }
        public override bool OnTake(IItemMoving item)
        {
            if (item is Wine && this.IsState(Pan.State.Normal))
            {
                item.ChangeState(Wine.State.Pouring);
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                this.ChangeState(Pan.State.HaveOil);
                return true;
            }

            if (item is Nut nut1 && this.IsState(Pan.State.Hot) && item.IsState(Nut.State.Normal))
            {
                item.ChangeState(Nut.State.Cooking);
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                nut1.GetPan(this);
                this.ChangeState(Pan.State.Cooking);
                timer.Show(4.2f);
                DOVirtual.DelayedCall(0.2f, () => nut1.OrderLayer = -1);
                return true;
            }
            return base.OnTake(item);
        }

        public bool CanUseButton()
        {
            return IsState(Pan.State.HaveOil, Pan.State.Cooked);
        }

        public void OnStoveButtonClick(bool isOn)
        {
            if (this.IsState(Pan.State.HaveOil) && isOn == true)
            {
                ChangeState(Pan.State.Hot);
            }
            else if (this.IsState(Pan.State.Cooked) && isOn == false)
            {
                ChangeState(Pan.State.Done);
            }
        }
    }
}