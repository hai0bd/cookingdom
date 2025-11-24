using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Link.Cooking.Spageti
{
    public class Leaf : ItemMovingBase
    {
        public enum State { Idle, InBoard, Cut, Done }
        public State state = State.Idle;
        public override bool IsCanMove => IsState(State.Idle, State.Cut);
        public override bool IsDone => !collider.enabled;
        [SerializeField] List<LeafFail> leaves;
        [SerializeField] Animation animaScale;
        [SerializeField] Board board;
        [SerializeField] HintText hintText_1;

        public override void ChangeState<T>(T t)
        {
            state = (State)(object)t;

            switch (state)
            {
                case State.Idle:
                    break;
                case State.InBoard:
                    foreach (var leaf in leaves)
                    {
                        leaf.OnActive();
                        leaf.OnDoneAction += CutLeaf;
                    }
                    break;
                case State.Cut:
                    break;
                case State.Done:
                    break;
            }
        }
        public override bool IsState<T>(T t)
        {
            return state == (State)(object)t;
        }

        public override bool OnTake(IItemMoving item)
        {
            if(item is Knife) LevelControl.Ins.LoseHalfHeart(TF.position);
            return base.OnTake(item);
        }


        public override void OnDrop()
        {
            base.OnDrop();
            OnBack();
        }

        private void CutLeaf(LeafFail leaf)
        {
            leaf.enabled = false;
            leaves.Remove(leaf);
            if (leaves.Count == 0)
            {
                ChangeState(State.Cut);
                hintText_1.OnActiveHint();
            }
            animaScale.Stop();
            animaScale.Play();
            SoundControl.Ins.PlayFX(Fx.LeafCut);
        }

        public override void OnDone()
        {
            base.OnDone();
            LevelControl.Ins.CheckStep(0.4f);
            board.CheckHint();
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