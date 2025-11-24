using DG.Tweening;
using Link;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
namespace HuyThanh.Cooking.SarmaleRomania
{
    public class Dill : ItemMovingBase
    {
        public enum State
        {
            Dirty, Cleaning, DoneCleaning, ReadyCutting, Cutting, MoveDillTrashBin, MovingPlate, Done
        }

        public State state = State.Dirty;
        [SerializeField] ItemAlpha dirtyAlpha;

        [Header("Dill Cutting")]
        [SerializeField] DillMoveTrash dillMoveTrash;
        [SerializeField] GameObject dillBase, dillOrigin, dillCutInPlate;

        [SerializeField] ItemAlpha dillUpperAlpha, dillInPlateAlpha;

        [FoldoutGroup("VFX")][SerializeField] ParticleSystem splashWaterVFX, cleanWaterVFX, blinkVFX;

        [SerializeField] Animation anim;
        [SerializeField] string animBounce;
        [SerializeField] int numberTapToClean;

        [SerializeField] Vector3 donePosition;
        public override bool IsCanMove => IsState(State.Dirty, State.DoneCleaning, State.MovingPlate);

        private float currentDirtyAlpha = 1f;

        public override bool IsState<T>(T t)
        {
            return state == (State)(object)t;
        }

        public override void ChangeState<T>(T t)
        {
            state = (State)(object)t;

            switch (state)
            {
                case State.MovingPlate:
                    anim.Play(animBounce);
                    dillCutInPlate.SetActive(true);
                    dillUpperAlpha.DoAlpha(0, 0.2f);
                    dillInPlateAlpha.DoAlpha(1, 0.2f);
                    break;
                case State.Cutting:
                    StartCoroutine(WaitForDoneCutting());
                    break;
                case State.Done:
                    anim.Play(animBounce);
                    dillCutInPlate.transform.DOLocalMove(donePosition, 0.2f);
                    break;
            }
        }
        public void OnDoneThrowRotten()
        {
            collider.enabled = true;
            ChangeState(State.MovingPlate);
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

            base.OnClickDown();
        }
        IEnumerator WaitForDoneCutting()
        {
            yield return WaitForSecondCache.Get(0.5f);
            collider.enabled = false;
            dillBase.SetActive(false);
            dillOrigin.SetActive(true);
            dillMoveTrash.gameObject.SetActive(true);
            dillMoveTrash.TF.DOMove(dillMoveTrash.TF.position + Vector3.right * 0.1f, 0.1f);
            anim.Play(animBounce);

            yield return WaitForSecondCache.Get(0.5f);
            ChangeState(State.MoveDillTrashBin);
        }
    }
}

