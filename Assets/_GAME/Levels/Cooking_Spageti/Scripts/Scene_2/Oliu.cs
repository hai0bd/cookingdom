using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Link.Cooking.Spageti
{
    public class Oliu : ItemMovingBase
    {
        [SerializeField] List<OliuFail> oliuFails;
        [SerializeField] SpriteRenderer leafSpr;
        [SerializeField] Transform raw;
        [SerializeField] Animation scaleAnim;
        [SerializeField] HintText hintText;

         public enum State { Idle, InBoard, Cut, Sliced, Pieced, Done }
        public State state = State.Idle;

        [SerializeField] Board board;

        public override bool IsCanMove => IsState(State.Idle, State.Pieced);

        public override bool IsDone => !collider.enabled;

        public override void ChangeState<T>(T t)
        {
            state = (State)(object)t;
            switch (state)
            {
                case State.Idle:
                    break;
                case State.InBoard:
                    foreach (var oliuFail in oliuFails)
                    {
                        oliuFail.OnDoneEvent.AddListener(OliuDone);
                        oliuFail.OnInit();
                    }
                    raw.DOLocalMoveY(0.75f, 0.2f);
                    break;
                case State.Cut:
                    break;
                case State.Sliced:
                    break;
                case State.Pieced:
                    leafSpr.DOFade(0, 0.2f);
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

        public void OliuDone(OliuFail oliuFail)
        {
            oliuFails.Remove(oliuFail);
            if (oliuFails.Count == 0)
            {
                ChangeState(State.Pieced);
                hintText.OnActiveHint();
            }
        }

        public override void OnClickDown()
        {
            if(IsCanMove) base.OnClickDown();
            SoundControl.Ins.PlayFX(Fx.Click);
        }

        public override void OnClickTake()
        {
            base.OnClickTake();
            SoundControl.Ins.PlayFX(Fx.Take);
        }

        public override void OnDrop()
        {
            base.OnDrop();
            OnBack();
        }

        public override void OnDone()
        {
            base.OnDone();
            LevelControl.Ins.CheckStep(0.4f);
            if(board != null) board.CheckHint();
        }

    }
}
