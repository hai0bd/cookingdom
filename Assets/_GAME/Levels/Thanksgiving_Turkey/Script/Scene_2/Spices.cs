using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link.Turkey
{
    public class Spices : ItemMovingBase
    {
        public enum State { Ready, Used }
        private State state = State.Ready;
        public ItemName spicesName;
        [SerializeField] AnimationClip select, ussing, used;
        [SerializeField] Animation animation;
        //[SerializeField] Collider2D collider;

        public override bool IsCanMove => state == State.Ready;

        public override void OnClickDown()
        {
            base.OnClickDown();
            ChangeAnim(select.name);
            SoundControl.Ins.PlayFX(LevelStep_1.FX.Hold);
        }

        public override void OnClickTake()
        {
            base.OnClickTake();
            SoundControl.Ins.PlayFX(LevelStep_1.FX.Put);
        }


        public override void ChangeState<T>(T t)
        {
            this.state = (State)(object)(t);
            switch (state)
            {
                case State.Ready:
                    break;
                case State.Used:
                    collider.enabled = false;
                    ChangeAnim(ussing.name);
                    DOVirtual.DelayedCall(1.5f, OnDrop);
                    break;
                default:
                    break;
            }
        }

        public override bool OnTake(IItemMoving item)
        {
            return false;
        }

        public override void OnDrop()
        {
            base.OnDrop();

            //if (LevelControl.Ins.IsHaveObject<PanSpice>(TF.position) is PanSpice pan)
            //{
            //    pan.OnTake(this);
            //    OnClickUp();
            //}
            //else
            //{
            OnClickTake();
            OnBack();
            //}

        }

        private void ChangeAnim(string anim)
        {
            //Debug.Log(anim);
            animation.Play(anim);
        }
    }
}