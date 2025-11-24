using DG.Tweening;
using Link;
using Satisgame;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HuyThanh.Cooking.TunaSaladFrench
{
    public class MustardIdle : ItemIdleBase
    {
        public enum State { Normal, OpenCap, TookMustard, Done }
        [SerializeField] State state;
        [SerializeField] Collider2D collider;

        private float countingClickDown = 0; ///dem so lan da cham vao hop de mo nap

        [BoxGroup("Anim Name")][SerializeField] Animation anim;
        [BoxGroup("Anim Name")][SerializeField] string animOpenCap, animTaking, animClosing;

        [SerializeField] BoxCollider2D boxCollider2D;
        [SerializeField] EmojiControl _emojiControl;

        public override void OnClickDown()
        {
            if (IsState(State.TookMustard))
            {
                ChangeState(State.Done);
                return;
            }
            countingClickDown++;

            if (countingClickDown > 1)
            {
                return;
            }
            SoundControl.Ins.PlayFX(Fx.Click);

            OnClickIdle();


            if (countingClickDown == 1)
            {
                ChangeState(MustardIdle.State.OpenCap);

                ///tat di bat lai collider
                collider.enabled = false;

                DOVirtual.DelayedCall(1.2f, () =>
                {
                    collider.enabled = true;
                });
                return;
            }

            base.OnClickDown();
        }

        public void OnClickIdle()
        {
            TF.DOKill();
            TF.DOScale(1.2f, 0.1f).OnComplete(() =>
            {
                TF.DOScale(1f, 0.1f);
            });
        }

        public override void ChangeState<T>(T t)
        {
            state = (State)(object)t;

            switch (state)
            {
                case State.OpenCap:
                    SoundControl.Ins.PlayFX(Fx.OpenMustard);
                    anim.Play(animOpenCap);
                    ChangeOpenCapCollider(isOn: true);

                    break;
                case State.TookMustard:
                    anim.Play(animTaking);
                    ChangeOpenCapCollider(isOn: false);
                    break;
                case State.Done:
                    _emojiControl.ShowPositive();
                    SoundControl.Ins.PlayFX(Fx.CloseMustard);
                    anim.Play(animClosing);
                    LevelControl.Ins.CheckStep(1f);
                    break;
            }
        }

        public override bool IsState<T>(T t)
        {
            return state == (State)(object)t;
        }

        public void ChangeOpenCapCollider(bool isOn)
        {
            if (isOn)
            {
                boxCollider2D.offset = Vector2.up * 0.4f;
                boxCollider2D.size = new Vector2(1.1f, 0.8f);
            }
            else
            {
                boxCollider2D.offset = Vector2.zero;
                boxCollider2D.size = new Vector2(1.1f, 1.75f);
            }
        }
    }
}

