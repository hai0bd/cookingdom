using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Link;
using DG.Tweening;
using Unity.VisualScripting;

namespace HoangLinh.Cooking.Test
{
    public class BeanPlate : ItemMovingBase
    {
        public enum State
        {
            HaveBean,
            Pouring,
            Done
        }
        [SerializeField] State state;
        [SerializeField] Animation anim;
        [SerializeField] string animPouring;


        public override bool IsCanMove => IsState(State.HaveBean);

        public override bool IsState<T>(T t)
        {
            return state == (State)(object)t;
        }

        public override void ChangeState<T>(T t)
        {
            state = (State)(object)t;

            switch (t)
            {
                case State.HaveBean:
                    break;
                case State.Pouring:
                    OrderLayer = 49;
                    anim.Play(animPouring);
                    DOVirtual.DelayedCall(1.1f, () =>
                    {
                        ChangeState(State.Done);
                    });
                    break;
                case State.Done:
                    OnBack();
                    break;
            }
        }

        public override void OnDrop()
        {
            base.OnDrop();
        }

        public override void OnClickDown()
        {
            base.OnClickDown();
            
        }

        public override void OnBack()
        {
            TF.DOMove(savePoint, 0.3f).OnComplete(() => OrderLayer = -50);
            TF.DORotateQuaternion(saveRot, 0.3f);

        }

    }
}