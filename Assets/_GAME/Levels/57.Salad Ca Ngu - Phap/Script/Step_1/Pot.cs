using DG.Tweening;
using Link;
using Satisgame;
using UnityEngine;
using UnityEngine.Rendering;

namespace HuyThanh.Cooking.TunaSaladFrench
{
    public class Pot : ItemMovingBase
    {
        public enum State { Normal, HaveWater, NotBoil, Boil, Cooking, PourWater, PouringWater, DonePour, Done }
        public override bool IsCanMove => IsState(State.Normal, State.HaveWater, State.PourWater, State.DonePour);

        [SerializeField] private State state;
        [SerializeField] private SpriteRenderer waterSprite, waterBoilSprite;
        [SerializeField] private ParticleSystem smokeVFX;
        [SerializeField] private ParticleSystem steamRisingVFX;
        [SerializeField] private Color colorSmoke;
        [SerializeField] private Color colorClearSmoke;
        [SerializeField] private SortingGroup waterInPot;

        [SerializeField] private EmojiControl _emojiControl;

        [SerializeField] NapNoi itemNapNoi;

        [SerializeField] Animation anim;
        [SerializeField] private string animPour;

        [SerializeField] Sprite hint;
        public bool IsHaveCover => Vector2.Distance(itemNapNoi.TF.position, TF.position) < 0.5f;

        public override void ChangeState<T>(T t)
        {
            state = (State)(object)t;

            switch (state)
            {
                case State.HaveWater:
                    //_emojiControl.ShowPositive();
                    waterSprite.DOFade(1f, 0.4f);
                    waterBoilSprite.DOFade(0f, 0.1f);
                    smokeVFX.Stop();
                    break;
                case State.NotBoil:
                    LevelControl.Ins.CheckStep(1f);
                    SoundControl.Ins.StopFX(Fx.Boil);
                    waterBoilSprite.DOFade(0f, 0.1f);
                    smokeVFX.Stop();
                    break;
                case State.Boil:
                    SoundControl.Ins.PlayFX(Fx.Boil, loop: true);
                    waterInPot.sortingOrder = 10; /// giam layer xuong de tranh loi
                    waterBoilSprite.DOFade(0.5f, 1f);
                    DOVirtual.DelayedCall(0.8f, () => smokeVFX.Play());
                    break;

                case State.PourWater:
                    LevelControl.Ins.SetHint(hint);
                    SoundControl.Ins.StopFX(Fx.Boil);
                    waterBoilSprite.DOFade(0f, 0.1f);
                    smokeVFX.Stop();
                    break;
                case State.PouringWater:
                    anim.Play(animPour);
                    DOVirtual.DelayedCall(1f, () =>
                    {
                        OnBack();
                        ChangeState(State.DonePour);
                    });
                    break;
                case State.Done:
                    _emojiControl.ShowPositive();
                    break;
            }
        }

        public override bool IsState<T>(T t)
        {
            return state == (State)(object)t;
        }

        public override void OnDrop()
        {
            base.OnDrop();
            OnBack();
            DOVirtual.DelayedCall(0.3f, () => waterInPot.sortingOrder = 2);
            DOVirtual.DelayedCall(0.3f, () =>
            {
                OrderLayer = 0;
            });
        }

        public override void OnClickDown()
        {
            base.OnClickDown();
            waterInPot.sortingOrder = 51;
            SoundControl.Ins.PlayFX(Fx.Click);
        }

        public override void OnMove(Vector3 pos, Quaternion rot, float time)
        {
            base.OnMove(pos, rot, time);
            DOVirtual.DelayedCall(time, () =>
            {
                waterInPot.sortingOrder = 2;
                OrderLayer = 0;
            });

        }

        public void ChangeSteam(bool isOk = false)
        {
            var main = steamRisingVFX.main;

            if (isOk)
            {
                main.startColor = colorSmoke;
                return;
            }

            if (IsHaveCover)
            {
                main.startColor = colorClearSmoke;
            }
            else
            {
                main.startColor = colorSmoke;
            }

        }
    }
}