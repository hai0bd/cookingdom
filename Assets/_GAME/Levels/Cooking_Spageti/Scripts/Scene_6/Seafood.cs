using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace Link.Cooking
{
    public class Seafood : ItemMovingBase
    {
        public enum SeafoodType { Shirmp, Scallop }
        [field: SerializeField] public SeafoodType Type { get; private set; }
        public enum State { Raw, InPan, Ripe, Riped, InSpoon, InPlate, Done }
        [SerializeField] State state;
        [SerializeField] ItemFryFlip fryFlip;
        [SerializeField] Transform square, raw, ripe;
        [SerializeField] ParticleSystem ripeVFX;
        [SerializeField] PlayVFX sparkVFX;
        public UnityEvent<Seafood> OnDoneEvent;
        public override bool IsCanMove => IsState(State.Raw) && base.IsCanMove;

        [SerializeField] AudioClip clickClip, takeClip;

        public UnityEvent OnOverripeEvent;

        public override void ChangeState<T>(T t)
        {
            state = (State)(object)t;

            switch (state)
            {
                case State.Raw:
                    break;
                case State.InPan:
                    ripeVFX.Play();
                    fryFlip.OnActive();
                    break;
                case State.Ripe:
                    ripeVFX.Play();
                    fryFlip.OnActive();
                    break;
                case State.Riped:
                    fryFlip.OnDespawn();
                    break;
                case State.InSpoon:
                    fryFlip.OnDespawn();
                    ripeVFX.Stop();
                    ripeVFX.Clear();
                    break;
                case State.InPlate:
                    OnDoneEvent?.Invoke(this);
                    break;
                case State.Done:
                default:
                    break;
            }
        }

        public override bool IsState<T>(T t)
        {
            return state == (State)(object)t;
        }

        protected override void Editor()
        {
            base.Editor();
            ripeVFX = GetComponentInChildren<ParticleSystem>();
            fryFlip = GetComponent<ItemFryFlip>();
            square = transform.GetChild(0);
        }

        public void Fliped()
        {
            if (IsState(State.Ripe))
            {
                if (fryFlip.IsState(ItemFryFlip.State.Ripe))
                {
                    ChangeState(State.Riped);
                    sparkVFX.Play();
                }
                else
                if (fryFlip.IsState(ItemFryFlip.State.Overripe))
                {
                    ChangeState(State.Done);
                    //Lose
                    LevelControl.Ins.LoseGame(3.5f);
                    OnOverripeEvent?.Invoke();
                }
            }
        }

        public override void OnDone()
        {
            square.DOLocalRotate(Vector3.zero, 0.2f);
        }

        public void Flip()
        {
            fryFlip.Flip();
        }

        public override void OnClickDown()
        {
            SoundControl.Ins.PlayFX(clickClip);
            base.OnClickDown();
        }

        public override void OnClickTake()
        {
            SoundControl.Ins.PlayFX(takeClip);
            base.OnClickTake();
        }
    }
}