using DG.Tweening;
using Link;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
namespace HuyThanh.Cooking.SarmaleRomania
{
    public class DillPice : ItemMovingBase
    {
        public enum State
        {
            Dirty, Cleaning, DoneCleaning, ReadyCutting, Cutting, MoveDillTrashBin, ReadyPice1, Pice1, ReadyPice2, Pice2, MovingPlate, Done
        }

        [SerializeField] State state;
        [Header("Dill Cutting")]
        [SerializeField] DillMoveTrash dillMoveTrash;
        [SerializeField] GameObject dillBase;
        [SerializeField] GameObject dillOrigin;


        [FoldoutGroup("Pice")]
        [SerializeField] GameObject pice1, pice2, piceDone;
        [SerializeField] Transform maskSlide, maskSpice, maskSlide2, maskSpice2, startPoint, finishPoint;
        [Header("Item Dirty")]
        [SerializeField] ItemAlpha dirtyAlpha;
        [FoldoutGroup("VFX")]
        [SerializeField] ParticleSystem splashWaterVFX, cleanWaterVFX, blinkVFX;
        [Header("ANimation")]
        [SerializeField] Animation anim;
        [SerializeField] string animBounce;

        [SerializeField] int numberTapToClean;

        [SerializeField] Knife knife;

        private float step = 0;
        private float currentDirtyAlpha = 1f;

        private bool canCut = true;
        private const int CHOP_STEP_CUT = 8;
        public override bool IsCanMove => IsState(State.Dirty, State.DoneCleaning, State.MovingPlate);



        public override bool IsState<T>(T t)
        {
            return state == (State)(object)t;
        }

        public override void ChangeState<T>(T t)
        {
            state = (State)(object)t;

            switch (state)
            {
                case State.Dirty:
                    break;
                case State.DoneCleaning:
                    break;
                case State.ReadyCutting:
                    break;
                case State.ReadyPice2:
                    dillOrigin.SetActive(false);
                    pice1.SetActive(false);
                    pice2.SetActive(true);
                    StartCoroutine(WaitForChangeToCut2());
                    break;
                case State.MoveDillTrashBin:
                    collider.enabled = false;
                    dillBase.SetActive(false);
                    dillOrigin.SetActive(true);
                    dillMoveTrash.gameObject.SetActive(true);
                    break;
                case State.Cutting:
                    StartCoroutine(WaitForDoneCutting());
                    break;
                case State.Pice1:
                    break;
                case State.Pice2:
                    piceDone.SetActive(true);
                    break;
                case State.MovingPlate:
                    break;
                case State.Done:
                    anim.Play(animBounce);
                    dillOrigin.SetActive(false);
                    //dillcutInplate.SetActive(true);
                    break;
                default:
                    break;
            }
        }
        public void OnDoneThrowRotten()
        {
            collider.enabled = true;
            ChangeState(State.ReadyPice1);
        }

        public override void OnDrop()
        {
            base.OnDrop();
            if (IsState(State.DoneCleaning))
            {
                DOVirtual.DelayedCall(0.31f, () =>
                {
                    splashWaterVFX.transform.position = TF.position;
                    splashWaterVFX.Play();
                    OrderLayer = -40;
                }); ///dang trong nuoc nen se de layer thap hon
            }
        }

        public override void OnClickTake()
        {
            base.OnClickTake();
        }

        public override void OnClickDown()
        {
            if (IsState(State.Cleaning))
            {
                cleanWaterVFX.transform.position = TF.position;
                cleanWaterVFX.Play();

                anim.Play(animBounce);

                currentDirtyAlpha -= 1f / numberTapToClean;
                dirtyAlpha.DoAlpha(currentDirtyAlpha, 0.1f);

                if (currentDirtyAlpha < 0f)
                {
                    cleanWaterVFX.Stop();
                    blinkVFX.transform.position = TF.position;
                    blinkVFX.Play();
                    ChangeState(State.DoneCleaning);
                }
                return;
            }

            if (!canCut)
            {
                return;
            }

            if (IsState(State.Pice1))
            {
                OrderLayer = 0;

                knife.ChangeAnim("KnifeCut");

                anim.Play(animBounce);
                //SoundControl.Ins.PlayFX(Fx.KnifeCutEgg);

                step += 1f / CHOP_STEP_CUT;
                Vector2 point = Vector3.Lerp(startPoint.position, finishPoint.position, step);
                //splashVFX.transform.position = point;
                //  splashVFX.Play();
                maskSlide.position = point;
                maskSpice.position = point;
                knife.TF.position = point;
                knife.PlayAllKatanaVFX(point + Vector2.up * 0.2f);

                StartCoroutine(WaitForCanCut());

                if (step >= 1f)
                {
                    step = 0; // reset counter step
                    ChangeState(State.ReadyPice2);
                }
                return;
            }

            if (IsState(State.Pice2))
            {
                OrderLayer = 0;

                knife.ChangeAnim("KnifeCut");
                anim.Play(animBounce);
                //SoundControl.Ins.PlayFX(Fx.KnifeCutEgg);

                step += 1f / CHOP_STEP_CUT;
                Vector2 point = Vector3.Lerp(startPoint.position, finishPoint.position, step);

                maskSlide2.position = point;
                maskSpice2.position = point;
                knife.TF.position = point;
                knife.PlayAllKatanaVFX(point + Vector2.up * 0.2f);

                StartCoroutine(WaitForCanCut());

                if (step >= 1f)
                {
                    knife.ChangeState(Knife.State.Done);
                    blinkVFX.transform.position = TF.position;
                    blinkVFX.Play();
                    step = 0; // reset counter step
                    ChangeState(State.MovingPlate);
                }
                return;
            }

            base.OnClickDown();
        }

        IEnumerator WaitForDoneCutting()
        {
            yield return WaitForSecondCache.Get(0.3f);
            ChangeState(State.MoveDillTrashBin);
        }

        IEnumerator WaitForCanCut()
        {
            canCut = false;
            yield return WaitForSecondCache.Get(0.16f);
            canCut = true;
        }

        IEnumerator WaitForChangeToCut2()
        {
            knife.OnMove(startPoint.position, Quaternion.identity, 0.2f);
            yield return WaitForSecondCache.Get(0.2f);
            ChangeState(State.Pice2);
        }
    }
}

