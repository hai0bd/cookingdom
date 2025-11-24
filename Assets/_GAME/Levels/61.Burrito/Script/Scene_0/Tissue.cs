using DG.Tweening;
using Link;
using UnityEngine;

namespace HuyThanh.Cooking.Burrito
{


    public class Tissue : ItemMovingBase
    {
        public enum State { Clean, Dirty, Done }

        [SerializeField] SpriteRenderer cleanTissue, dirtyTissue;
        [SerializeField] TrashBin trashBin;

        MeatClean meatClean;

        State state = State.Clean;
        State avt = State.Clean;
        float cleanRate = 0;

        Vector3 point;

        public override bool IsCanMove => state != State.Done;

        public override bool IsState<T>(T t)
        {
            return state == (State)(object)(t);
        }

        public override void ChangeState<T>(T t)
        {
            state = (State)(object)(t);
            switch (state)
            {
                case State.Clean:
                    break;
                case State.Dirty:
                    LevelControl.Ins.AddTrash(this);
                    SoundControl.Ins.StopFX(Fx.Tissue);
                    break;
                case State.Done:
                    LevelControl.Ins.RemoveTrash(this);
                    break;
                default:
                    break;
            }
        }
        public override void OnClickDown()
        {
            SoundControl.Ins.PlayFX(Fx.Click);
            base.OnClickDown();
            if (IsState(State.Dirty))
            {
                trashBin.OnNeedTrashBin();
            }

        }

        public override void OnDrop()
        {
            base.OnDrop();

            if (IsState(State.Dirty))
            {
                SoundControl.Ins.PlayFX(Fx.PutDown);
                trashBin.OnNoNeedTrashBin();
            }

            if (IsState(State.Clean))
            {
                SoundControl.Ins.PlayFX(Fx.Tissue);
                ChangeState(State.Dirty);
                avt = State.Dirty;
                cleanTissue.DOFade(0, 0.3f);
                dirtyTissue.DOFade(1, 0.3f);
            }
        }

        private void LateUpdate()
        {
            if (meatClean != null && IsState(State.Clean) && meatClean.IsState(MeatClean.State.NeedClean))
            {
                if (Vector3.Distance(TF.position, point) > 0.1f)
                {
                    if (!SoundControl.Ins.IsPlaying(Fx.Tissue))
                    {
                        SoundControl.Ins.PlayFX(Fx.Tissue, true);
                    }
                    point = TF.position;
                    cleanRate += Time.deltaTime;
                    meatClean.AddClean();
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
            if (meatClean == null)
            {
                meatClean = collision.GetComponent<MeatClean>();
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (meatClean != null && collision.GetComponent<MeatClean>() != null)
            {
                meatClean = null;
                SoundControl.Ins.StopFX(Fx.Tissue);
            }
        }
    }
}