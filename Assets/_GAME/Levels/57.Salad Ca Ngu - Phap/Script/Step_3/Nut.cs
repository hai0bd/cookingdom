using DG.Tweening;
using Link;
using UnityEngine;

namespace HuyThanh.Cooking.TunaSaladFrench
{
    public class Nut : ItemMovingBase
    {
        public enum State
        {
            Normal,
            Cooking,
            Cooked, //state nay dat vao dia hoac cai thot cung duoc
            Done // state xay ra khi rac len cai thot
        }

        [SerializeField] private State state;
        [SerializeField] private SpriteRenderer nutSprite;
        [SerializeField] private Color cookedColor;
        [SerializeField] private ItemAlpha nutAlpha;
        [SerializeField] private ParticleSystem vfxBlink;

        [SerializeField] Animation anim;

        private Pan _pan;
        public override bool IsCanMove => IsState(Nut.State.Normal);

        public override bool IsState<T>(T t)
        {
            return state == (State)(object)t;
        }

        public override void ChangeState<T>(T t)
        {
            state = (State)(object)t;

            switch (state)
            {
                case Nut.State.Cooking:
                    nutSprite.DOColor(cookedColor, 4.2f);
                    DOVirtual.DelayedCall(4.2f, () =>
                    {
                        _pan.ChangeState(Pan.State.Cooked);
                        ChangeState(Nut.State.Cooked);
                    });
                    break;
                case State.Cooked:
                    vfxBlink.Play();
                    break;
                case State.Done:
                    nutAlpha.DoAlpha(0f, 0.4f);
                    break;
            }
        }

        public void ChangeAnim(string animName)
        {
            if (anim == null)
                return;
            anim.Play(animName);
        }

        public void GetPan(Pan pan)
        {
            this._pan = pan;
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