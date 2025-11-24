using DG.Tweening;
using Link;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HuyThanh.Cooking.TunaSaladFrench
{
    public class ItemSpice : ItemMovingBase
    {
        public enum State { Origin, Cutting, Cut, Spice, Done }
        public enum NameType { None, Knife, Mortar, Pestle, Spoon, Garlic, Herb, Capers }

        [SerializeField] State state;
        [SerializeField] GameObject itemActive;
        public override bool IsCanMove => IsState(State.Origin, State.Cut, State.Spice);

        [field: SerializeField] public NameType ItemName;

        [BoxGroup("SpriteControl")][SerializeField] GameObject origin, cut, spice;
        [BoxGroup("ItemAlphaControl")][SerializeField] ItemAlpha itemAlphaOrigin, itemAlphaCut, itemAlphSpice;

        public override void ChangeState<T>(T t)
        {
            state = (State)(object)t;

            switch (state)
            {
                case State.Origin:
                    break;
                case State.Cutting:

                    //anim chuyen hinh anh
                    DOVirtual.DelayedCall(0.4f, () =>
                    {
                        itemAlphaOrigin.DoAlpha(0, 0.4f);
                        cut.SetActive(true);
                        itemAlphaCut.DoAlpha(1, 0.4f);
                    });

                    DOVirtual.DelayedCall(0.8f, () =>
                    {
                        itemAlphaCut.DoAlpha(0, 0.4f);
                        spice.SetActive(true);
                        itemAlphSpice.DoAlpha(1, 0.4f);
                    });

                    //chuyen state
                    DOVirtual.DelayedCall(1.2f, () =>
                    {
                        ChangeState(ItemSpice.State.Spice);
                    });

                    break;
                case State.Spice:


                    break;
                case State.Done:

                    DOVirtual.DelayedCall(0.2f, () =>
                    {
                        spice.SetActive(false);
                        itemActive.SetActive(true);
                    });
                    break;
                default:
                    break;
            }
        }

        public override bool OnTake(IItemMoving item)
        {
            if (item is Knife && IsState(State.Origin) && LevelControl.Ins.IsHaveObject<CuttingBoard>(TF.position) != null)
            {
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                item.ChangeState(Knife.State.Cutting);
                ChangeState(ItemSpice.State.Cutting);
                return true;
            }
            return base.OnTake(item);
        }

        public override void OnDone()
        {
            gameObject.SetActive(false);
            base.OnDone();
        }

        public override void OnDrop()
        {
            base.OnDrop();
            OnBack();
        }

        public override bool IsState<T>(T t)
        {
            return state == (State)(object)t;
        }
        public override void OnClickTake()
        {
            base.OnClickTake();
            SoundControl.Ins.PlayFX(Fx.Take);
        }

        public override void OnClickDown()
        {
            base.OnClickDown();
            SoundControl.Ins.PlayFX(Fx.Click);
        }

    }

}
