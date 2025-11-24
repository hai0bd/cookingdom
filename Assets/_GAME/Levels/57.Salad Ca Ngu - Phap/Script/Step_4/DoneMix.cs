using DG.Tweening;
using Link;
using Satisgame;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace HuyThanh.Cooking.TunaSaladFrench
{
    public class DoneMix : ItemIdleBase
    {
        public enum State
        {
            Normal,
            XaLach,
            Salad,
            Tuna,
            Tomato,
            Egg,
            Pepper,
            Done
        }

        [SerializeField] private State state;

        [SerializeField] private List<ItemMovingBase> order;
        [SerializeField] EmojiControl _emojiControl;

        [SerializeField] List<GameObject> xaLachTargets;
        [SerializeField] List<GameObject> tomatoTargets;
        [SerializeField] List<GameObject> eggTargets;

        [BoxGroup("Anim Cat Steal Tomato")][SerializeField] Animation animCat;
        [SerializeField] string animCatName;


        public override bool OnTake(IItemMoving item)
        {
            if (item is SpoonSaladSauce && item.IsState(SpoonSaladSauce.State.HaveSauce) && order[0].Equals(item) && this.IsState(State.Normal))
            {
                DOVirtual.DelayedCall(1f, () => order.RemoveAt(0));
                this.ChangeState(State.XaLach);
                item.ChangeState(SpoonSaladSauce.State.Pouring);
                item.OnMove(TF.position + Vector3.right * 0.2f + Vector3.up * 0.5f, item.TF.rotation, 0.2f);
                return true;
            }

            if (item is SaladBowlDoneMix && order[0].Equals(item) && this.IsState(State.Salad))
            {
                SoundControl.Ins.PlayFX(Fx.OilPour);
                DOVirtual.DelayedCall(2f, () =>
                {
                    order.RemoveAt(0);
                    ChangeState(State.Tuna);
                });
                item.OnMove(TF.position + Vector3.right * 0.2f, Quaternion.identity, 0.2f);
                item.ChangeState(SaladBowlDoneMix.State.Pouring);
                return true;
            }

            if (item is Tuna && item.IsState(Tuna.State.Spice) && order[0].Equals(item) && this.IsState(State.Tuna))
            {
                DOVirtual.DelayedCall(1f, () =>
                {
                    order.RemoveAt(0);
                    ChangeState(State.Tomato);
                });
                item.ChangeState(Tuna.State.Pouring);
                item.OnMove(TF.position + Vector3.right * 0.1f, Quaternion.identity, 0.2f);
                return true;
            }

            if (item is Wine && order[0].Equals(item) && IsState(State.Pepper))
            {
                if (item.TF.name == "Pepper")
                {
                    SoundControl.Ins.PlayFX(Fx.PepperPour);
                    DOVirtual.DelayedCall(1f, () => ChangeState(State.Done));
                    _emojiControl.ShowPositive();
                }
                DOVirtual.DelayedCall(1f, () => order.RemoveAt(0));
                item.OnMove(TF.position + Vector3.right * 0.2f, Quaternion.identity, 0.2f);
                item.ChangeState(Wine.State.Pouring);
                return true;
            }


            return base.OnTake(item);
        }

        public override bool IsState<T>(T t)
        {
            return state == (State)(object)t;
        }

        public override void ChangeState<T>(T t)
        {
            state = (State)(object)t;

            switch (state)
            {
                case State.Normal:
                    break;
                case State.XaLach:
                    foreach (var item in xaLachTargets)
                    {
                        item.SetActive(true);
                    }
                    break;
                case State.Tomato:
                    foreach (var item in tomatoTargets)
                    {
                        item.SetActive(true);
                    }
                    break;
                case State.Egg:
                    foreach (var item in eggTargets)
                    {
                        item.SetActive(true);
                    }
                    break;
                case State.Done:
                    LevelControl.Ins.CheckStep(1f);
                    break;
            }
        }

        public override void OnClickDown()
        {
            base.OnClickDown();
        }

        public void OnDoneXaLach(GameObject gO)
        {
            xaLachTargets.Remove(gO);
            if (xaLachTargets.Count == 0)
            {
                ChangeState(State.Salad);
            }
        }

        public void OnDoneTomato(GameObject gO)
        {
            tomatoTargets.Remove(gO);
            if (tomatoTargets.Count == 0)
            {
                SoundControl.Ins.PlayFX(Fx.CatSound);
                animCat.Play(animCatName);
                ChangeState(State.Egg);
            }
        }

        public void OnDoneEgg(GameObject gO)
        {
            eggTargets.Remove(gO);
            if (eggTargets.Count == 0)
            {
                ChangeState(State.Pepper);
            }
        }

        [Button("Play animcat")]
        public void PlayCatSteal()
        {
            SoundControl.Ins.PlayFX(Fx.CatSound);
            animCat.Play(animCatName);
        }
    }
}