using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Link.Cooking.Spageti
{
    public class Knife : ItemMovingBase
    {
        public enum State { Normal, Cut, Done }
        [SerializeField] State state;
        public override bool IsCanMove => IsState(State.Normal);
        [SerializeField] GameObject idle, hold;
        [SerializeField] Animator animator;
        Tween t;
        public override bool IsDone => idle.activeSelf;

        public override void ChangeState<T>(T t)
        {
           state = (State)(object)t;
        }
        public override bool IsState<T>(T t)
        {
           return state == (State)(object)t;
        }

        public override void OnClickDown()
        {
            base.OnClickDown();

            idle.SetActive(false);
            hold.SetActive(true);
        }
        
        public override void OnDrop()
        {
            base.OnDrop();
            OnBack();
        }

        public override void OnBack()
        {
            base.OnBack();
            t?.Kill();
            t = DOVirtual.DelayedCall(0.2f, () =>
            {
                idle.SetActive(true);
                hold.SetActive(false);

                LevelControl.Ins.CheckStep(0.4f);
            });
        }

        [Button]
        public void TabCut()
        {
            ChangeAnim("cut");
            SoundControl.Ins.PlayFX(Fx.KnifeCut);
        }

        public void ChangeAnim(string name)
        {
            animator.ResetTrigger(name);
            animator.SetTrigger(name);
        }

    }
}