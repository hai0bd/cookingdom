using Link;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

namespace HuyThanh.Cooking.SarmaleRomania
{

    public class CabbageLeafCutting : ItemMovingBase
    {
        public enum State
        {
            Normal,
            ReadyCutting,
            Cutting,
            Cutting2,
            DoneCut,
            InBowl,
        }
        [SerializeField] State state;
        [BoxGroup("Cutting 1")][SerializeField] Transform maskSlide, startPoint, endPoint;
        [BoxGroup("Cutting 2")][SerializeField] Transform maskSlide2, cutting1TF, cutting2TF;
        [SerializeField] int numberCut = 5;
        [SerializeField] ParticleSystem vegetableVFX;
        [SerializeField] ItemAlpha squareAlpha, squareSpice;
        [SerializeField] Animation anim;

        float step = 0;
        bool isCanCut = true;

        private Knife knife;

        public override bool IsCanMove => IsState(State.Normal, State.DoneCut);

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
                    break;
                case State.Cutting2:
                    OnSavePoint();
                    saveRot = TF.rotation;
                    cutting1TF.gameObject.SetActive(false);
                    cutting2TF.gameObject.SetActive(true);
                    break;
            }
        }

        public void PourIntoBowl()
        {
            squareSpice.DoAlpha(0, 0.5f);
        }

        public override void OnClickDown()
        {
            if (IsState(State.Cutting))
            {
                if (isCanCut == false) return;
                StartCoroutine(WaitForDoneCut());

                step += 1f / numberCut;

                Vector3 pos = Vector3.Lerp(startPoint.position, endPoint.position, step);

                maskSlide.position = pos;
                knife.TF.position = pos + Vector3.down * 0.2f;
                vegetableVFX.transform.position = pos + Vector3.up * 0.1f;
                vegetableVFX.Play();
                knife.ChangeAnim("KnifeCut");

                if (step >= 1)
                {
                    //knife.ChangeState(Knife.State.Done);
                    knife.OnMove(TF.position, Quaternion.identity, 0.1f);
                    ChangeState(State.Cutting2);
                    step = 0;
                }
                return;
            }

            if (IsState(State.Cutting2))
            {
                if (isCanCut == false) return;
                StartCoroutine(WaitForDoneCut());

                step += 1f / numberCut;

                Vector3 pos = Vector3.Lerp(startPoint.position, endPoint.position, step);

                maskSlide2.position = pos;
                knife.TF.position = pos + Vector3.down * 0.2f;
                vegetableVFX.transform.position = pos + Vector3.up * 0.1f;
                vegetableVFX.Play();
                knife.ChangeAnim("KnifeCut");

                if (step >= 1)
                {
                    knife.ChangeState(Knife.State.Done);
                    ChangeState(State.DoneCut);
                    step = 0;
                }

                return;
            }
            base.OnClickDown();
        }

        IEnumerator WaitForDoneCut()
        {
            isCanCut = false;
            yield return WaitForSecondCache.Get(0.17f);
            isCanCut = true;
        }

        public void SetKnife(Knife knife)
        {
            this.knife = knife;
        }

        public void Show()
        {
            squareAlpha.DoAlpha(1, 0.2f);
        }

        public void PlayAnim()
        {
            anim.Play("Bounce");
        }
    }
}
