using System.Collections;
using DG.Tweening;
using Link;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HoangLinh.Cooking.Test
{
    public class BasketCutItems : ItemMovingBase
    {
        public enum State
        {
            Normal,
            CutBig,
            CutSmall,
            Spice,
            Done
        }

        [SerializeField] State state;
        [SerializeField] Knife knife;

        [SerializeField] private int CUT_BIG = 3;
        [SerializeField] private int CUT_SMALL = 6;

        [SerializeField] GameObject normal, cutBig, cutSmall, spice;
        [BoxGroup("Cut Ref")][SerializeField] Transform maskSlide, maskCutBig, maskCutSmall, maskSpice, startPoint, endPoint;

        private float step = 0;

        public override bool IsCanMove => IsState(State.Normal, State.Spice);

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
                    this.OnSavePoint();
                    knife.OnMove(startPoint.position, Quaternion.identity,0.2f);
                    break;
                case State.CutSmall:

                    break;
                case State.Spice:
                    break;
                case State.Done:
                    break;
            }
        }

        public override bool OnTake(IItemMoving item)
        {   
            return base.OnTake(item);
        }

        public override void OnClickDown()
        {
            if (IsState(State.CutBig))
            {
                OrderLayer = 1;
                knife.ChangeAnim("KnifeCutting");

                step += 1f / CUT_BIG;
                Vector2 point = Vector3.Lerp(startPoint.position, endPoint.position, step);
                
                Debug.Log($"point: {point}");
                knife.TF.position = maskSlide.position = maskCutBig.position = point;
                
                knife.OrderLayer = 49;

                if (step >= 1)
                {
                    step = 0;
                    ChangeState(State.CutSmall);
                    cutBig.SetActive(false);
                    cutSmall.SetActive(true);
                    step = 0;
                }

                return;
            }

            if (IsState(State.CutSmall))
            {
                OrderLayer = 1;
                knife.ChangeAnim("KnifeCutting");

                step += 1f / CUT_SMALL;
                Vector2 point2 = Vector2.Lerp(startPoint.position, endPoint.position, step);
                Debug.Log("point: " + point2);


                knife.TF.position = maskCutSmall.position = maskSpice.position = point2;
                //maskCutSmall.position = point;
                //maskSpice.position = point;
                
                knife.OrderLayer = 49;

                if (step >= 1)
                {
                    step = 0;
                    ChangeState(State.Spice);
                    knife.ChangeState(Knife.State.Done);
                }

                return;
            }
            base.OnClickDown();
            TF.DORotate(Vector3.zero, 0.1f);
        }

        public override void OnDrop()
        {
            base.OnDrop();
        }

    }
}
