using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Link.Cooking.Spageti
{
    public class Bear : ItemIdleBase
    {
        [SerializeField] ItemMovingBase[] items;

        public enum State { Hide, Start, Scream, Attack, Idle, Happy, MoveOn }

        public enum AnimType { idle, walk, attack, scream, takeflower }

        [SerializeField] State state;
        [SerializeField] Collider2D col;
        [SerializeField] Animator anim;

        [SerializeField] AudioClip screamClip, attackClip, happyClip, happyClip2;
        [SerializeField] Sprite hint;
        [SerializeField] HintText hintText;

        Tween tween;

        public override void ChangeState<T>(T t)
        {
           state = (State)(object)t;
           switch (state)
           {
                case State.Hide:
                    break;
                case State.Start:
                    foreach (var i in items)
                    {
                        i.SetControl(false);
                    }
                    ChangeAnim(AnimType.walk);
                    TF.DOMoveX(-1, 1.5f).SetEase(Ease.Linear).OnComplete(() => ChangeState(State.Scream));
                    break;
                case State.Scream:
                    ChangeAnim(AnimType.scream);
                    SoundControl.Ins.PlayFX(screamClip);
                    DOVirtual.DelayedCall(2f, () => ChangeState(State.Idle));
                    CameraControl.Instance.OnShake(CameraControl.ShakeType.Light_1, 1f, 0.5f);
                    break;
                case State.Attack:
                    tween?.Kill();
                    ChangeAnim(AnimType.attack);
                    //Attack
                    SoundControl.Ins.PlayFX(attackClip);
                    CameraControl.Instance.OnShake(CameraControl.ShakeType.Light_2, 1f, 1.1f);
                    tween = DOVirtual.DelayedCall(2, () => 
                        {
                            ChangeState(State.Idle);
                            LevelControl.Ins.LoseFullHeart(TF.position);
                        });

                    break;
                case State.Idle:
                    col.enabled = true;
                    ChangeAnim(AnimType.idle);
                    tween?.Kill();
                    tween = DOVirtual.DelayedCall(Random.Range(2.5f, 5f), () => ChangeState(State.Attack));
                    break;
                case State.Happy:
                    tween?.Kill();
                    ChangeAnim(AnimType.takeflower);
                    tween = DOVirtual.DelayedCall(3.5f, () => ChangeState(State.MoveOn));
                    SoundControl.Ins.PlayFX(happyClip, 1f);
                    SoundControl.Ins.PlayFX(happyClip2, 1.35f);
                    hintText.OnActiveHint();
                    break;
                case State.MoveOn:
                    TF.DOMoveX(4.5f, 3.5f).SetEase(Ease.Linear).OnComplete(OnDone);
                    ChangeAnim(AnimType.walk);
                    LevelControl.Ins.SetHint(hint);
                    break;
           }
        }
        public override bool IsState<T>(T t)
        {
           return state == (State)(object)t;
        }

        public override bool OnTake(IItemMoving item)
        {

            if (item is FlowerPack flower && item.IsDone)
            {
                item.OnDone();
                ChangeState(State.Happy);
            }
            return false;
        }

        public override void OnDone()
        {
            base.OnDone();
            foreach (var i in items)
            {
                i.SetControl(true);
            }
            gameObject.SetActive(false);
            SoundControl.Ins.PlayFX(Fx.Blink);
        }

        private void ChangeAnim(AnimType name)
        {
            anim.ResetTrigger(name.ToString());
            anim.SetTrigger(name.ToString());
        }

        protected override void Editor()
        {
            base.Editor();
            if (items == null || items.Length == 0)
            {
                items = GetComponentsInChildren<ItemMovingBase>();
            }
        }
    }
}