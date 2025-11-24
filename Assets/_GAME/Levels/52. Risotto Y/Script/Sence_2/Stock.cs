using DG.Tweening;
using Link;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LawNguyen.CookingGame.LRisottoY
{
    public class Stock : ItemMovingBase
    {

        public enum State { idle, during };
        State state = State.idle;
        [SerializeField] Animation animation;


        private void Start()
        {
            DOVirtual.DelayedCall(2f, () => OnSavePoint());
        }
        public override void OnBack()
        {
            base.OnBack();
        }
        public override void OnDrop()
        {
            base.OnDrop();
            OnBack();

        }

        public override void ChangeState<T>(T t)
        {
            this.state = (State)(object)t;

            switch (state)
            {
                case State.idle:

                    break;
                case State.during:

                    animation.Play();
                    break;


            }


        }

    }
}

