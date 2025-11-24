using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Link.Turkey
{
    public class Tissue : ItemMovingBase
    {
        Chicken chicken;
        public enum State { Clean, Dirty, Remove }
        State state = State.Clean;
        State avt = State.Clean;
        float cleanRate = 0;
        [SerializeField] SpriteRenderer cleanTissue, dirtyTissue;
        Vector3 point;

        public override bool IsCanMove => state != State.Remove;

        public override void ChangeState<T>(T t)
        {
            this.state = (State)(object)(t);
            switch (state)
            {
                case State.Clean:
                    break;
                case State.Dirty:
                    LevelControl.Ins.AddTrash(this);
                    SoundControl.Ins.StopFX(LevelStep_1.FX.Tissue);
                    break;
                case State.Remove:
                    LevelControl.Ins.RemoveTrash(this);
                    break;
                default:
                    break;
            }
        }

        public override void OnDrop()
        {
            base.OnDrop();
            if (IsState(State.Clean))
            {
                ChangeState(State.Dirty);
                avt = State.Dirty;
                cleanTissue.DOFade(0, 0.3f);
                dirtyTissue.DOFade(1, 0.3f);
            }
        }

        public override bool IsState<T>(T t)
        {
            return this.state == (State)(object)(t);
        }

        private void LateUpdate()
        {
            if ( chicken != null && IsState(State.Clean) && chicken.IsState(Chicken.State.NeedClean))
            {
                if (Vector3.Distance(TF.position, point) > 0.1f)
                {
                    if (!SoundControl.Ins.IsPlaying(LevelStep_1.FX.Tissue))
                    {
                        SoundControl.Ins.PlayFX(LevelStep_1.FX.Tissue, true);
                    }

                    point = TF.position;
                    cleanRate += Time.deltaTime;
                    chicken.AddClean();
                    if (cleanRate >= 0.5f)
                    {
                        ChangeState(State.Dirty);
                    }
                    if (cleanRate >= 0.49f && avt == State.Clean)
                    {
                        avt = State.Dirty;
                        cleanTissue.DOFade(0, 0.3f);
                        dirtyTissue.DOFade(1, 0.3f);
                    }
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (chicken == null)
            {
                chicken = collision.GetComponent<Chicken>();
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (chicken != null && collision.GetComponent<Chicken>() != null)
            {
                chicken = null;
                SoundControl.Ins.StopFX(LevelStep_1.FX.Tissue);
            }
        }

    }
}