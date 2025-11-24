using DG.Tweening;
using Link;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HoangLinh.Cooking.Test
{
    public class Knife : ItemMovingBase
    {
        public enum State
        {
            Normal,
            Cutting,
            KatanaCutting,
            CornCutting,
            Done
        }

        [SerializeField] State state;
        [SerializeField] Animation anim;
        [SerializeField] string animCut;
        [SerializeField] string animKatanaCut;
        [SerializeField] ParticleSystem katanaFX;
        [SerializeField] Vector3 rotOnPickup;
        [BoxGroup("Knife State Sprite")][SerializeField] GameObject knifeNormal, knifeCutting;

        public override bool IsCanMove => IsState(State.Normal);

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
                    KnifeStatus(isCutting: false);
                    break;
                case State.Cutting:
                    KnifeStatus(isCutting: true);
                    TF.DORotate(Vector3.zero, 0.1f);
                    break;
                case State.KatanaCutting:
                    KnifeStatus(isCutting: true);
                    katanaFX.Play();
                    ChangeAnim(animKatanaCut);
                    DOVirtual.DelayedCall(1f, () => ChangeState(State.Done));
                    break;
                case State.CornCutting:
                    //TODO: show cutting sprite
                    break;
                case State.Done:
                    KnifeStatus(isCutting: false);
                    ChangeState(State.Normal);
                    OnBack();
                    break;
            }
        }

        private void KnifeStatus(bool isCutting)
        {
            knifeCutting.SetActive(isCutting);
            knifeNormal.SetActive(!isCutting);
        }

        public override void OnClickDown()
        {
            base.OnClickDown();
            TF.DORotate(rotOnPickup, 0.1f);
        }

        public override void OnClickTake()
        {
            base.OnClickTake();
        }

        public override void OnDrop()
        {
            base.OnDrop();
            if (IsState(State.Normal)) OnBack();
        }
        public void ChangeAnim(string name)
        {
            anim.Stop(name);
            anim.Play(name);
        }
    }
}