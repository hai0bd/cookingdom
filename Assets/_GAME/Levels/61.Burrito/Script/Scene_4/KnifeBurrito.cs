using DG.Tweening;
using Link;
using System.Collections;
using UnityEngine;

namespace HuyThanh.Cooking.Burrito
{
    public class KnifeBurrito : ItemMovingBase
    {
        public enum State
        {
            Normal,
            Cutting,
            Done,
        }

        [SerializeField] State state;

        [SerializeField] Animation anim;
        [SerializeField] string animCuttingBurrito;

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
                case State.Cutting:
                    anim.Play(animCuttingBurrito);
                    SoundControl.Ins.PlayFX(Fx.KnifeCut);
                    StartCoroutine(WaitForBack());
                    break;
            }
        }

        public override void OnClickDown()
        {
            base.OnClickDown();
            SoundControl.Ins.PlayFX(Fx.Click);
            TF.DORotate(new Vector3(0, 0, -30), 0.1f);
        }

        public override void OnDrop()
        {
            base.OnDrop();
            SoundControl.Ins.PlayFX(Fx.PutDown);
        }

        IEnumerator WaitForBack()
        {
            yield return WaitForSecondCache.Get(1.2f);
            OnBack();
        }
    }
}
