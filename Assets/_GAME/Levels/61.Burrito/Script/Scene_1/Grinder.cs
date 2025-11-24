using DG.Tweening;
using Link;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

namespace HuyThanh.Cooking.Burrito
{
    public class Grinder : ItemIdleBase
    {
        public enum State
        {
            Normal,
            PreGrindering,
            Grindering,
            Done
        }

        [SerializeField] State state;
        [SerializeField] GameObject meatSprite;
        [SerializeField] private Transform[] meatPieces;

        [SerializeField] int sliceCount = 20;
        [SerializeField] float spawnRadius = 0.2f;
        [SerializeField] Transform spawnTransform;
        [SerializeField] Transform bowlTransform;
        [SerializeField] Transform meatBowlTransform;
        [SerializeField] GameObject meatSlicePrefab;
        [SerializeField] Transform meatParent;

        [SerializeField] ItemAlpha meat1, meat2, meat3;
        [SerializeField] Animation meatAnim;

        [SerializeField] ClockTimer clockTimer;
        [SerializeField] ParticleSystem vfxBlink;

        [SerializeField] HintText hintText;



        public override bool IsState<T>(T t)
        {
            return state == (State)(object)t;
        }

        public override void ChangeState<T>(T t)
        {
            state = (State)(object)t;

            switch (state)
            {
                case State.PreGrindering:
                    break;
                case State.Grindering:
                    SoundControl.Ins.PlayFX(Fx.Grinder);
                    TF.DOShakePosition(8f, strength: new Vector3(0.01f, 0.01f, 0), vibrato: 15, randomness: 10, fadeOut: false);
                    StartCoroutine(WaitForChangeStateDone());
                    AnimateMeatPieces();
                    SpawnMeatSlices();
                    break;
                case State.Done:
                    SoundControl.Ins.PlayFX(Fx.DoneSomething);
                    hintText.OnActiveHint();
                    LevelControl.Ins.NextHint();
                    LevelControl.Ins.CheckStep(1f);
                    break;
            }
        }
        public override bool OnTake(IItemMoving item)
        {
            if (item is BeefRaw && item.IsState(BeefRaw.State.DoneCut))
            {
                item.OnMove(TF.position + Vector3.up * 0.05f + Vector3.right * 0.4f, Quaternion.identity, 0.2f);
                item.ChangeState(BeefRaw.State.Grindering);
                DOVirtual.DelayedCall(0.2f, () =>
                {
                    item.TF.gameObject.SetActive(false);
                    meatSprite.SetActive(true);
                });

                ChangeState(State.Grindering);

                return true;
            }
            return base.OnTake(item);
        }

        [Button("Animate Meat Pieces")]
        private void AnimateMeatPieces()
        {
            Vector3 center = meatPieces[0].parent.position; // Center is the parent's position

            foreach (var meatPiece in meatPieces)
            {
                if (meatPiece != null)
                {
                    // Calculate distance from the center
                    float initialDistance = Vector3.Distance(meatPiece.position, center);

                    // Define elliptical path around the center
                    Vector3[] path = new Vector3[]
                    {
                        meatPiece.position,
                        center + new Vector3(initialDistance * 0.5f, initialDistance * 0.25f, 0),
                        center + new Vector3(-initialDistance * 0.5f, -initialDistance * 0.25f, 0),
                        center
                    };

                    // Move along the elliptical path
                    meatPiece.DOPath(path, 8f, PathType.CatmullRom)
                        .SetEase(Ease.InOutQuad);

                    DOVirtual.DelayedCall(7f, () =>
                    {
                        meatPiece.DOScale(Vector3.zero, 1f);
                    });
                }
            }
        }

        [Button("Spawn Meat Slices")]
        public void SpawnMeatSlices()
        {
            meat1.DoAlpha(1f, 1f, 1f);
            meat2.DoAlpha(1f, 3f, 2f);
            meat3.DoAlpha(1f, 3f, 5f);
            clockTimer.Show(8f);
            DOVirtual.DelayedCall(8f, () =>
            {
                meatAnim.Stop();
                meatBowlTransform.DOScale(Vector3.one, 0.1f);
                vfxBlink.Play();
            });

            for (int i = 0; i < sliceCount; i++)
            {
                StartCoroutine(WaitForSpawnMeat(Random.Range(0, 8f - 1.5f)));
            }
        }

        IEnumerator WaitForSpawnMeat(float time)
        {
            yield return WaitForSecondCache.Get(time);

            Vector2 randomPosition = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPosition = spawnTransform.position + new Vector3(randomPosition.x, randomPosition.y, 0);
            Vector3 targetPosition = bowlTransform.position;

            targetPosition.y = spawnPosition.y;

            GameObject meatSlice = Instantiate(meatSlicePrefab, spawnPosition, Quaternion.identity, meatParent);

            meatSlice.GetComponent<SpriteRenderer>().flipY = Random.Range(0, 2) == 0;

            meatSlice.transform.DOMove(targetPosition, 1f).SetEase(Ease.InOutQuad);

            DOVirtual.DelayedCall(1f, () =>
            {
                meatSlice.transform.DOScale(new Vector3(0f, 0f, 0f), 1f).SetEase(Ease.InOutQuad);
            });
        }

        IEnumerator WaitForChangeStateDone()
        {
            yield return WaitForSecondCache.Get(8f);
            ChangeState(State.Done);
        }
    }

}
