using DG.Tweening;
using Link;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

namespace HuyThanh.Cooking.Burrito
{
    public class CatHandTakeBowl : ItemIdleBase
    {
        public enum State
        {
            Normal,
            Trolling,
        }

        [SerializeField] Animation anim;
        [SerializeField] string animName;
        [SerializeField] State state;
        [SerializeField] Transform catHand;
        [SerializeField] float catHandPosXSave, catHandPosXSteal;
        [SerializeField] Spoon spoon;

        [BoxGroup("Reset Anim")][SerializeField] Transform spoonTF, bowlTF, spiceTF;
        [BoxGroup("Reset Anim Position")]
        [SerializeField] Vector3 spoonTFPos, bowlTFPos, spiceTFPos;
        [SerializeField] float maxTimeWaiting = 6f;

        private float timer = 0;

        private void OnEnable()
        {
            StartCoroutine(WaitForChangeToSteal());
        }
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
                    Reset();
                    timer = 0;
                    spoon.enabled = true;
                    anim.Stop();
                    catHand.DOLocalMoveX(catHandPosXSave, 0.2f);
                    StartCoroutine(WaitForChangeToSteal());
                    break;
                case State.Trolling:
                    spoon.enabled = false;

                    anim.Play(animName);
                    break;
            }
        }

        public override void OnClickDown()
        {
            base.OnClickDown();

            if (IsState(State.Trolling))
            {
                ChangeState(State.Normal);
            }
        }

        IEnumerator WaitForChangeToSteal()
        {
            while (timer < maxTimeWaiting)
            {
                if (spoon.IsMoving == false)
                {
                    timer += Time.deltaTime;
                }
                else
                {
                    timer = 0;
                }

                yield return null;
            }
            catHand.DOLocalMoveX(catHandPosXSteal, 0.2f);
            if (!SoundControl.Ins.IsPlaying(Fx.CatSound))
                SoundControl.Ins.PlayFX(Fx.CatSound);
            yield return WaitForSecondCache.Get(0.2f);

            ChangeState(State.Trolling);
        }

        private void Reset()
        {
            bowlTF.DOLocalMove(bowlTFPos, 0.1f);
            spoonTF.DOLocalMove(spoonTFPos, 0.1f);
            spiceTF.DOLocalMove(spiceTFPos, 0.1f);
        }

    }
}