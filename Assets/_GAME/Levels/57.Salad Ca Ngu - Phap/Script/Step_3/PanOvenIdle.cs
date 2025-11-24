using DG.Tweening;
using Link;
using System.Collections.Generic;
using UnityEngine;

namespace HuyThanh.Cooking.TunaSaladFrench
{

    public class PanOvenIdle : ItemIdleBase
    {
        public enum State
        {
            Butter,
            Pearl,
            Chill,
            Nut,
            DoneSpice,
            Done
        }
        [SerializeField] State state;

        [SerializeField] private List<PanItemTarget> butterItems;
        [SerializeField] private List<PanItemTarget> pearlItems;
        [SerializeField] private ItemAlpha chillAlpha;
        [SerializeField] private ItemAlpha nutAlpha;

        [SerializeField] Animation anim;
        [SerializeField] string nutFallAnim;

        [SerializeField] private PanOvenMoving panItemMoving;
        [SerializeField] MiniGame3 miniGame3;
        [SerializeField] MiniGame4 miniGame4;

        [SerializeField] Sprite hint;

        public override bool OnTake(IItemMoving item)
        {
            if (IsState(PanOvenIdle.State.Chill) && item is SugarSpoon)
            {
                item.OnMove(TF.position, item.TF.rotation, 0.2f);
                item.ChangeState(SugarSpoon.State.Pouring);
                SoundControl.Ins.PlayFX(Fx.SaltPouring);
                DOVirtual.DelayedCall(1f, () => this.ChangeState(PanOvenIdle.State.Nut));
                this.ActiveChill();
                return true;
            }

            if (IsState(PanOvenIdle.State.Nut) && item is Nut && item.IsState(Nut.State.Cooked))
            {
                item.TF.SetParent(TF);
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                item.ChangeState(Nut.State.Done);
                this.ChangeState(PanOvenIdle.State.DoneSpice);

                anim.Play(nutFallAnim);
                nutAlpha.DoAlpha(1f, .3f);
                return true;
            }

            if (item is PanOvenMoving && item.IsState(PanOvenMoving.State.Baked))
            {
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                item.OnSave(0.2f);
                item.ChangeState(PanOvenMoving.State.Done);
                ChangeState(PanOvenIdle.State.Done);
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
                case State.Butter:
                    break;
                case State.Pearl:
                    ActivePearlCollider();
                    break;
                case State.DoneSpice:
                    LevelControl.Ins.SetHint(hint); ///hint cho do an vao lo
                    panItemMoving.SetCollider(true);///luc nay moi keo tha duoc cai chao tbhg
                    //miniGame4.OnFinish(null);
                    break;

            }
        }

        private void ActivePearlCollider()
        {
            foreach (PanItemTarget item in pearlItems)
            {
                item.EnableCollider();
            }
        }

        public void OnDonePanItemTarget(PanItemTarget item, PanItemMoving.ItemType itemType)
        {
            if (itemType == PanItemMoving.ItemType.Butter)
            {
                butterItems.Remove(item);
                if (butterItems.Count == 0)
                {
                    ChangeState(PanOvenIdle.State.Pearl);
                }
            }

            if (itemType == PanItemMoving.ItemType.Pearl)
            {
                pearlItems.Remove(item);
                if (pearlItems.Count == 0)
                {
                    ChangeState(PanOvenIdle.State.Chill);
                }
            }

        }

        private void ActiveChill()
        {
            chillAlpha.DoAlpha(1f, 1f, .5f);
        }
    }

}