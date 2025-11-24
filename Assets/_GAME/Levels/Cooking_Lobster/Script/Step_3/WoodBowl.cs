using DG.Tweening;
using Satisgame;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link.Cooking.Lobster
{
    public class WoodBowl : ItemMovingBase
    {
        public enum State { Empty, Fill, Moving, Decore }
        [SerializeField, ReadOnly] State state;
        [SerializeField] GameObject outlineGO, mixGO;
        [SerializeField] EmojiControl emoji;
        [SerializeField] Animation anima;
        [SerializeField] ParticleSystem smokeVFX;

        [SerializeField] HintText hintText;

        public override bool IsCanMove => IsState(State.Moving);

        public override bool IsDone => IsState(State.Decore);

        public override void ChangeState<T>(T t)
        {
            state = (State)(object)t;
            switch (state)
            {
                case State.Empty:
                    break;
                case State.Fill:
                    mixGO.SetActive(true);
                    anima.Play("WoodBowl_Fill");
                    DOVirtual.DelayedCall(1.2f, () =>
                    {
                        LevelControl.Ins.CheckStep();
                        emoji.ShowPositive();
                        smokeVFX.Play();
                    });
                    break;
                case State.Moving:
                    break;
                case State.Decore:
                    outlineGO.SetActive(true);
                    hintText.OnActiveHint();
                    break;
                default:
                    break;
            }
        }
        public override bool IsState<T>(T t)
        {
            return state == (State)(object)t;
        }

        public override bool OnTake(IItemMoving item)
        {
            if (item is PanSpice)
            {
                item.OnMove(TF.position + new Vector3(0.98f, 0.56f, 0), Quaternion.identity, 0.2f);
                item.ChangeState(PanSpice.State.Pouring);
                ChangeState(State.Fill);
                return true;
            }
            return base.OnTake(item);
        }

        public override void OnDone()
        {
            base.OnDone();
            TF.DOScale(1.3f, 0.2f);
            // anima.Play("IdleItemAppear");
            DOVirtual.DelayedCall(0.5f, () => ChangeState(State.Decore));
        }

        public override void OnClickDown()
        {
            base.OnClickDown();
            SoundControl.Ins.PlayFX(LevelStep_1.Fx.Click);
        }
        public override void OnClickTake()
        {
            base.OnClickTake();
            SoundControl.Ins.PlayFX(LevelStep_1.Fx.Take);
        }

    }
}