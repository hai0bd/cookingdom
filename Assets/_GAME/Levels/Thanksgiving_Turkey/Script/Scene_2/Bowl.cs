using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Link.Turkey
{
    public class Bowl : ItemIdleBase
    {
        [SerializeField] SpriteRenderer core, halfCore;
        public enum State { Empty, Full, AHalf, Clear }
        [ShowInInspector] State state = State.Empty;

        public override bool OnTake(IItemMoving item)
        {
            if (item is Pan)
            {
                item.ChangeState(Pan.State.Clear);
                item.OnMove(TF.position + new Vector3(-.8f, .8f, 0), Quaternion.identity, 0.15f);
                ChangeState(State.Full);
                return true;    
            }
            else if (item is Spoon && item.IsState(Spoon.State.Empty) && (item.IsState(State.Full) || item.IsState(State.AHalf)))
            {
                item.ChangeState(Spoon.State.TakeMixed);
            }
            return false;
        }

        public override void ChangeState<T>(T t)
        {
            this.state = (State)(object)(t);
            switch (state)
            {
                case State.Empty:
                    break;
                case State.Full:
                    core.transform.localPosition = Vector3.up * 0.45f + Vector3.left * 0.45f;
                    core.transform.DOLocalMove(Vector2.zero, 1.5f);
                    core.DOFade(1, 1.5f);
                    break;
                case State.AHalf:
                    core.DOFade(0, .5f);
                    halfCore.DOFade(1, .5f);
                    break;
                case State.Clear:
                    halfCore.DOFade(0, .5f);
                    break;
                default:
                    break;
            }
            //Debug.Log("Bowl : " + state);
        }

        public override bool IsState<T>(T t)
        {
            return this.state == (State)(object)(t);
        }

    }
}
