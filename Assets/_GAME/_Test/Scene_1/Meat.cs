using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Link;
using Sirenix.OdinInspector;
using DG.Tweening;
using JetBrains.Annotations;

namespace HoangLinh.Cooking.Test
{
    public class Meat : ItemMovingBase
    {
        public enum State
        {
            Normal,
            CutBig,
            KatanaCut,
            Pieces,
            Grindering,
            Done,
        }

        public override bool IsCanMove => IsState(State.Normal, State.Pieces);
        [SerializeField] State state;
        [SerializeField] Knife knife;

        [SerializeField] Animation anim;
        [SerializeField] string animPickupMeat, animSlicesToPieces;
        [BoxGroup("Cutting")][SerializeField] Transform maskSlices, maskPieces, startPoint, endPoint;
        private float step = 0;
        private const int CHOP_STEP = 4;

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
                    break;
                case State.CutBig:
                    knife.OnMove(startPoint.position, Quaternion.Euler(0, 0, 90), 0.2f);
                    break;
                case State.KatanaCut:
                    //DOVirtual.DelayedCall(0.5f, () => anim.Play(anim_1));
                    DOVirtual.DelayedCall(1.5f, () => ChangeState(State.Pieces));
                    break;
                case State.Pieces:
                    break;
                case State.Grindering:
                    break;
                case State.Done:
                    break;
            }
        }

        public override void OnClickDown()
        {
            if (state == State.CutBig)
            {
                OrderLayer = 1;

                //knife.ChangeAnim(animPickupMeat);
                step += 1f / CHOP_STEP;     
                Vector2 point = Vector3.Lerp(startPoint.position, endPoint.position, step);
                maskSlices.position = point;
                maskPieces.position = point;
                knife.TF.position = point;

                if (step >= 1)
                {
                    step = 0;
                    ChangeState(State.KatanaCut);
                    knife.OnMove(TF.position, Quaternion.identity, 0.2f);
                    DOVirtual.DelayedCall(0.2f, () => knife.ChangeState(Knife.State.KatanaCutting));
                }
                return;
            }

            if (IsState(State.Pieces))
            {
                anim.Play(animSlicesToPieces);
            }
            base.OnClickDown();
        }

        public override void OnDrop()
        {
            
            base.OnDrop();
        }

    }
}