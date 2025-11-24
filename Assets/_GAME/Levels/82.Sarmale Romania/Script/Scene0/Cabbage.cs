using DG.Tweening;
using Link;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
namespace HuyThanh.Cooking.SarmaleRomania
{
    public class Cabbage : ItemMovingBase
    {
        public enum State
        {
            Dirty, Cleaning, DoneCleaning, Peel, DonePeel, MoveTrash, RemoveCore, MoveCoreTotrash, MovingPlate, Done
        }
        public State state = State.Dirty;

        [SerializeField] ItemAlpha dirtyAlpha, cabageAlpha, cabagePeelAlpha;

        [FoldoutGroup("Cabbage Leaf")][SerializeField] CabbageLeafStretch cabbageLeafTop, cabbageLeafLeft, cabbageLeafRight;

        [FoldoutGroup("Anim to remove core")][SerializeField] Transform squarePeel, squareCore;
        [FoldoutGroup("Anim to remove core")][SerializeField] ItemAlpha squarePeelAlpha, squareCoreAlpha;

        [FoldoutGroup("Removethecore")][SerializeField] GameObject cabbageCore, cabbageNoCore;
        [FoldoutGroup("Removethecore")][SerializeField] Core core;

        [FoldoutGroup("VFX")][SerializeField] ParticleSystem splashWaterVFX, cleanWaterVFX, blinkVFX;

        [SerializeField] Animation anim;
        [SerializeField] string animBounce;
        [SerializeField] int numberTapToClean;

        public override bool IsCanMove => IsState(State.Dirty, State.DoneCleaning, State.MovingPlate);

        private float currentDirtyAlpha = 1f;
        private int countdone = 3;

        private CuttingBoard cuttingBoard;

        public void SetCuttingBoard(CuttingBoard cB)
        {
            cuttingBoard = cB;
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
                case State.Dirty:
                    break;
                case State.DoneCleaning:
                    break;
                case State.Cleaning:
                    break;
                case State.Peel:
                    StartCoroutine(WaitForDoneMoveToCuttingBoard());
                    break;
                case State.DonePeel:

                    break;
                case State.MoveTrash:
                    break;
                case State.RemoveCore:
                    StartCoroutine(WaitForCoreMove());
                    break;
                case State.MovingPlate:
                    break;
                case State.MoveCoreTotrash:
                    StartCoroutine(WaitForShowCore());
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
                    currentDirtyAlpha = 1f;
                }
                return;
            }

            base.OnClickDown();
        }

        public void OnDoneThrowRotten()
        {
            countdone--;

            if (countdone <= 0)
            {
                ChangeState(State.RemoveCore);
            }
        }

        public void OnDoneThrowRottenCore()
        {
            collider.enabled = true;
            ChangeState(State.MovingPlate);
        }

        public void PlayAnim(string animName = "Bounce")
        {
            anim.Play(animName);
        }

        IEnumerator WaitForDoneMoveToCuttingBoard()
        {
            yield return WaitForSecondCache.Get(0.2f);
            collider.enabled = false;
            cuttingBoard.SetCollider(false);
            cabbageLeafTop.enabled = true;
            cabbageLeafLeft.enabled = true;
            cabbageLeafRight.enabled = true;
        }

        IEnumerator WaitForCoreMove()
        {
            TF.DOJump(TF.position, 2, 1, 1);
            squarePeel.DORotate(Vector3.right * 180f, 1f);
            squareCore.DORotate(Vector3.right * 180f, 1f);
            squareCoreAlpha.DoAlpha(1f, 1f);
            squarePeelAlpha.DoAlpha(0f, 1f);
            yield return WaitForSecondCache.Get(1f);
            cuttingBoard.SetCollider(true);
        }

        IEnumerator WaitForShowCore()
        {
            yield return WaitForSecondCache.Get(1f);
            cabbageCore.SetActive(false);
            cabbageNoCore.SetActive(true);
            core.gameObject.SetActive(true);
            core.OnActive();
        }
    }
}

