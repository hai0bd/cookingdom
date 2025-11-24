using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link.Cooking.Spageti
{
    public class Bacon : ItemMovingBase
    {
        public enum State { Idle, InBoard, Cut, Sliced, Pieced, Done }
        public State state = State.Idle;
        [SerializeField] ParticleSystem slashVFX;
        [SerializeField] string cutAnimName = "tomatocut";
        Knife knife;
        [SerializeField] Animation animScale;
        [SerializeField] AnimaBase2D burst;
        [SerializeField] GameObject raw;
        [SerializeField] Board board;
        [SerializeField] HintText hintText_1;
        public override bool IsDone => !collider.enabled;

        public override bool IsCanMove => base.IsCanMove && IsState(State.Idle, State.Pieced);

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
                knife.SetOrder(2);
                StartCoroutine(IECut());
                return true;
            }

            return false;
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

        public override void OnDrop()
        {
            base.OnDrop();
            OnBack();
        }

        public override void OnDone()
        {
            base.OnDone();
            LevelControl.Ins.CheckStep(0.4f);
            if (board != null)
            {
                board.CheckHint();
            }
            hintText_1.OnActiveHint();
        }
        private IEnumerator IECut()
        {
            yield return new WaitForSeconds(0.2f);
            knife.ChangeAnim(cutAnimName);
            yield return new WaitForSeconds(0.2f);
            CutSound();
            slashVFX.Play();
            animScale.Play();
            yield return new WaitForSeconds(1);
            knife.OnMove(TF.position + new Vector3(1.5f, 1.5f, 0), Quaternion.identity, 0.2f);
            knife.ChangeState(Knife.State.Normal);
            ChangeState(State.Pieced);
            burst.gameObject.SetActive(true);
            raw.SetActive(false);
            burst.OnActive();
        }
        protected virtual void CutSound()
        {
            SoundControl.Ins.PlayFX(Fx.Slash);
            SoundControl.Ins.PlayFX(Fx.Slash, 0.15f);
            SoundControl.Ins.PlayFX(Fx.Slash, 0.3f);
            SoundControl.Ins.PlayFX(Fx.Slash, 0.45f);
        }

    }
}