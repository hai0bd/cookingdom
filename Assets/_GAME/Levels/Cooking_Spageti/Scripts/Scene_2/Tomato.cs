using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using DG.Tweening;
using Unity.Burst.CompilerServices;
using UnityEngine;


namespace Link.Cooking.Spageti
{
    public class Tomato : ItemMovingBase
    {
        public const float CUT_STEP = 10;

        public enum State { Idle, InBoard, Cut, Sliced, Pieced, Done }
        public State state = State.Idle;

        [SerializeField] GameObject idle, slice, piece;
        [SerializeField] Transform maskSlide, maskPiece, startPoint, finishPoint;
        [SerializeField] Animation animScale;
        [SerializeField] ParticleSystem slashVFX, fruitCutVFX;
        [SerializeField] string cutAnimName = "tomatocut";
        [SerializeField] Board board;

        [SerializeField] HintText hintText_1;

        Knife knife;
        private float step;

        public override bool IsCanMove => IsState(State.Idle, State.Pieced);

        public override bool IsDone => !collider.enabled;

        public override void ChangeState<T>(T t)
        {
           state = (State)(object)t;
        }

        public override bool IsState<T>(T t)
        {
           return state == (State)(object)t;
        }

        public override bool OnTake(IItemMoving item)
        {
            if (item is Knife knife && IsState(State.InBoard))
            {
                this.knife = knife;
                knife.OnMove(TF.position, Quaternion.identity, 0.2f);
                knife.ChangeState(Knife.State.Cut);
                StartCoroutine(IECut());
                return true;
            }

            return false;
        }

        private IEnumerator IECut()
        {
            yield return new WaitForSeconds(0.2f);
            knife.ChangeAnim(cutAnimName);
            yield return new WaitForSeconds(0.2f);
            CutSound();
            slashVFX.Play();
            animScale.Play();
            yield return new WaitForSeconds(0.7f);
            idle.SetActive(false);
            slice.SetActive(true);
            piece.SetActive(true);
            maskSlide.position = startPoint.position;
            maskPiece.position = startPoint.position;
            yield return new WaitForSeconds(0.3f);
            knife.OnMove(startPoint.position, Quaternion.identity, 0.2f);
            ChangeState(State.Sliced);
        }

        public override void OnClickDown()
        {
            if(IsCanMove) base.OnClickDown();
            if (IsState(State.Sliced))
            {
                step+= 1/CUT_STEP;
                Vector2 point = Vector3.Lerp(startPoint.position, finishPoint.position, step); 
                maskSlide.position = point;    
                maskPiece.position = point;    
                knife.TF.position = point; 
                fruitCutVFX.transform.position = point;
                fruitCutVFX.Play();
                
                knife.ChangeAnim("cut");
                SoundControl.Ins.PlayFX(Fx.KnifeCut);
                if (step >= 1f)
                {
                    animScale.Play();
                    slice.SetActive(false);
                    ChangeState(State.Pieced);
                    knife.ChangeState(Knife.State.Normal);
                    DOVirtual.DelayedCall(0.15f, ()=> 
                    knife.OnMove(TF.position + new Vector3(1.5f, 1.5f, 0), Quaternion.identity, 0.2f));
                }
            }else
            {
                SoundControl.Ins.PlayFX(Fx.Click);
            }
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
            if (board != null) board.CheckHint();
            hintText_1.OnActiveHint();
        }

        protected virtual void CutSound()
        {
            SoundControl.Ins.PlayFX(Fx.Slash);
            SoundControl.Ins.PlayFX(Fx.Slash, 0.15f);
            SoundControl.Ins.PlayFX(Fx.Slash, 0.3f);
        }
    }
}