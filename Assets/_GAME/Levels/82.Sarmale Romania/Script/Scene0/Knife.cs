using DG.Tweening;
using Link;
using System.Collections;
using UnityEngine;
namespace HuyThanh.Cooking.SarmaleRomania
{
    public class Knife : ItemMovingBase
    {
        public enum State
        {
            Normal,
            Cutting,
            CuttingDill,
            KatanaCut,
            PeelOnion,
            RemoveCoreCabbage,
            Done
        }
        public override bool IsCanMove => IsState(State.Normal);

        [SerializeField] State state;
        [SerializeField] Vector3 rotOnClick;
        [SerializeField] Animation anim;

        [SerializeField] string animCut;
        [SerializeField] string animPeelOnion;
        [SerializeField] string animKatana;
        [SerializeField] string animRemoveCore;

        [SerializeField] GameObject knifeFront, knifeBack;

        [SerializeField] ParticleSystem[] katanaVFX;

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
                case State.CuttingDill:
                    ShowFront(false);
                    StartCoroutine(WaitForDoneCutting());
                    break;
                case State.Cutting:
                    ShowFront(false);
                    break;
                case State.PeelOnion:
                    ShowFront(false);
                    ChangeAnim(animPeelOnion);
                    StartCoroutine(WaitForChangeToDone());
                    break;
                case State.KatanaCut:
                    anim.Play(animKatana);
                    StartCoroutine(WaitForChangeToDone());
                    break;
                case State.RemoveCoreCabbage:
                    anim.Play(animRemoveCore);
                    StartCoroutine(WaitForDoneCoreCut());
                    break;
                case State.Done:
                    ShowFront(true);
                    OnBack();
                    ChangeState(State.Normal);
                    break;
            }
        }

        private void ShowFront(bool value)
        {
            knifeFront.SetActive(value);
            knifeBack.SetActive(!value);
        }

        public void PlayAllKatanaVFX(Vector3 position)
        {
            for (int index = 0; index < katanaVFX.Length; index++)
            {
                katanaVFX[index].transform.position = position;
                katanaVFX[index].Play();
            }
        }

        public override void OnClickDown()
        {
            base.OnClickDown();
            TF.DORotate(rotOnClick, 0.1f);
        }

        public override void OnClickTake()
        {
            base.OnClickTake();
            OrderLayer = 10;
        }

        public void ChangeAnim(string animName)
        {
            anim.Play(animName);
        }

        IEnumerator WaitForDoneCoreCut()
        {
            yield return WaitForSecondCache.Get(1.1f);
            ChangeState(State.Done);
        }

        IEnumerator WaitForDoneCutting()
        {
            yield return WaitForSecondCache.Get(0.2f);
            anim.Play(animCut);
            yield return WaitForSecondCache.Get(0.5f);
            ChangeState(State.Done);
        }

        IEnumerator WaitForChangeToDone()
        {
            yield return WaitForSecondCache.Get(1f);
            ChangeState(State.Done);
        }
    }
}

