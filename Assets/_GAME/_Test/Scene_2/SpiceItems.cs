using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Link;
using Unity.VisualScripting;
using DG.Tweening;

namespace HoangLinh.Cooking.Test
{
    public enum SpiceType
    {
        Onion,
        Garlic,
        Meat,
        Pepper,
        Chili,
        Salt,
        Tumeric,
        Tomato,
        Oregano,
    }

    public class SpiceItems : ItemMovingBase
    {
        public enum State
        {
            Normal,
            Pouring,
            Done
        }

        [SerializeField] State state;
        [SerializeField] SpiceType spiceType;
        [SerializeField] Animation anim;
        [SerializeField] string animPouring;
        [SerializeField] Transform moveTF;

        public override bool IsCanMove => IsState(State.Normal);

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
                case State.Pouring:
                    OrderLayer = 49;
                    StartCoroutine(MoveSpices());
                    break;
                case State.Done:
                    OnBack();
                    break;
            }
        }

        public override void OnClickTake()
        {
            base.OnClickTake();
            if (IsState(State.Pouring))
            {
                OrderLayer = 49;
            }
        }

        public override void OnClickDown()
        {
            base.OnClickDown();
            SoundControl.Ins.PlayFX(Fx.PickUp);
        }

        public override void OnDrop()
        {

            base.OnDrop();
            SoundControl.Ins.PlayFX(Fx.Drop);
        }

        IEnumerator MoveSpices()
        {
            this.TF.DOMove(moveTF.position, .3f);
            yield return new WaitForSeconds(.35f);
            anim.Play(animPouring);
            yield return new WaitForSeconds(1.15f);
            ChangeState(State.Done);
        }

        public bool IsSpiceType(SpiceType type)
        {
            return spiceType == type;
        }
    }
}
