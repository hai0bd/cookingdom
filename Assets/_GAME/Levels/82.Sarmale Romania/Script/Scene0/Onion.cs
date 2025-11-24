using DG.Tweening;
using Link;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using VinhLB;
namespace HuyThanh.Cooking.SarmaleRomania
{
    public class Onion : ItemMovingBase
    {
        public enum State { Dirty, Cleaning, DoneCleaning, ReadyPeel, Peel, MoveTrash, ReadyCutting, Cutting, ReadyPice, Pice, MovingPlate, Done }
        public State state = State.Dirty;

        [SerializeField] ItemAlpha dirtyAlpha;
        [FoldoutGroup("VFX")][SerializeField] ParticleSystem splashWaterVFX, cleanWaterVFX, blinkVFX, katanaSuperVFX;

        [SerializeField] Animation anim;
        [SerializeField] string animBounce;

        [SerializeField] int numberTapToClean;
        [BoxGroup("Onionpeel")]
        [SerializeField] GameObject Onionorigin;
        [SerializeField] GameObject Onionpeel;
        [SerializeField] GameObject OnionCut;

        [SerializeField] GameObject onionCutPiece;
        [SerializeField] GameObject onionPice;

        [SerializeField] ItemAlpha onionCutAlpha, onionDoneAlpha;

        [SerializeField] ModifiedMeshFoldDraggable meshFoldLeafLeft, meshFoldLeafRight;

        int onionRemove = 0;
        public override bool IsCanMove => IsState(State.Dirty, State.DoneCleaning, State.MovingPlate);

        [BoxGroup("Cutting")][SerializeField] Transform maskSlide, maskSpice, startPoint, finishPoint;

        [SerializeField] Knife knife;
        private float step = 0;
        private float currentDirtyAlpha = 1f;
        private bool canCut = true;

        private const int CHOP_STEP_CUT = 4;

        private void OnEnable()
        {
            meshFoldLeafLeft.onDetached += OnDoneThrowRotten;
            meshFoldLeafRight.onDetached += OnDoneThrowRotten;
        }

        private void OnDisable()
        {
            meshFoldLeafLeft.onDetached -= OnDoneThrowRotten;
            meshFoldLeafRight.onDetached -= OnDoneThrowRotten;
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
                case State.Peel:
                    StartCoroutine(WaitForDoneKatanaCut());
                    break;
                case State.MoveTrash:
                    collider.enabled = false;
                    break;
                case State.ReadyCutting:
                    collider.enabled = true;
                    break;
                case State.Cutting:
                    knife.OnMove(startPoint.position + new Vector3(0, -0.3f, 0), Quaternion.identity, 0.2f);
                    break;
                case State.Pice:
                    knife.OnMove(TF.position, Quaternion.identity, 0.2f);
                    knife.ChangeState(Knife.State.KatanaCut);
                    katanaSuperVFX.Play();
                    katanaSuperVFX.transform.position = TF.position;

                    StartCoroutine(WaitForDonePice());
                    break;
                case State.MovingPlate:
                    blinkVFX.transform.position = TF.position;
                    blinkVFX.Play();
                    break;
                case State.Done:
                    anim.Play(animBounce);
                    break;
            }
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

            if (IsState(State.Cutting))
            {
                OrderLayer = 0;

                knife.ChangeAnim("KnifeCut");
                StartCoroutine(WaitForCanCut());
                step += 1f / CHOP_STEP_CUT;
                Vector2 point = Vector3.Lerp(startPoint.position, finishPoint.position, step);

                maskSlide.position = point;
                maskSpice.position = point;
                knife.TF.position = point;
                knife.PlayAllKatanaVFX(point + Vector2.up * 0.2f);

                anim.Play(animBounce);

                if (step >= 1f)
                {
                    knife.ChangeState(Knife.State.Done);
                    step = 0; // reset counter step
                    ChangeState(State.ReadyPice);
                }
                return;
            }

            base.OnClickDown();
        }

        public void OnDoneThrowRotten(ModifiedMeshFoldDraggable draggable)
        {
            onionRemove++;
            if (onionRemove == 1)
            {
                meshFoldLeafRight.enabled = true;
            }
            if (onionRemove >= 2)
            {
                ChangeState(State.ReadyCutting);
            }
            anim.Play(animBounce);
        }

        IEnumerator WaitForDoneKatanaCut()
        {
            yield return WaitForSecondCache.Get(1f);
            Onionorigin.SetActive(false);
            Onionpeel.SetActive(true);
            OnionCut.SetActive(true);
        }

        IEnumerator WaitForDonePice()
        {
            yield return WaitForSecondCache.Get(0.5f);
            onionCutAlpha.DoAlpha(0, 0.5f);
            onionDoneAlpha.DoAlpha(1, 0.5f);
            yield return WaitForSecondCache.Get(0.5f);
            ChangeState(Onion.State.MovingPlate);
        }

        IEnumerator WaitForCanCut()
        {
            canCut = false;
            yield return WaitForSecondCache.Get(0.17f);
            canCut = true;
        }
    }
}

