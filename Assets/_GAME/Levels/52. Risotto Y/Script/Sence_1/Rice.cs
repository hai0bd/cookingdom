using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Link;
using Satisgame;

namespace LawNguyen.CookingGame.LRisottoY
{
    public class Rice : ItemMovingBase
    {
        public enum State { dirty, wash, donewash }
        public State state;

        [SerializeField] GameObject Riceunwash;
        [SerializeField] List<Transform> grains;
        [SerializeField] WaterSink waterSink;

        [Header("Sprites alpha blend")]
        [SerializeField] List<SpriteRenderer> riceUnwashSprites;
        [SerializeField] List<SpriteRenderer> riceWashSprites;

        [Header("Giới hạn vùng bay (hình vuông)")]
        [SerializeField, Range(0.1f, 5f)] float maxOffsetX = 2f;
        [SerializeField, Range(0.1f, 5f)] float maxOffsetY = 2f;

        [SerializeField] ParticleSystem Spankelow;
        [SerializeField] Animation ricebound;

        [Header("Progress UI")]
        [SerializeField] private Image progressBarFill;
        [SerializeField] private GameObject progressBarRoot;
        [SerializeField] Sprite Hintrice;

        private const float minRotationToMoveGrain = 30f;

        public event Action<UnityEngine.Object> OnCompleteEvent;
        public override bool IsCanMove => !IsState(State.wash);

        private bool isTouching = false;
        private Vector3 lastDirection;
        private float totalRotation = 0f;
        private float washProgress = 0f;

        private bool hasStartedFloatingEffect = false;
        private bool hasPreparedFloating = false;

        private Dictionary<Transform, Vector3> grainStartPositions = new();
        private Dictionary<Transform, Vector3> grainTargetPositions = new();

        public override bool IsState<T>(T t) => this.state.Equals((State)(object)t);

        public override void OnBack()
        {
            base.OnBack();
            if (IsState(State.donewash))
            {
                DOVirtual.DelayedCall(0.5f, () =>
                {
                    OnDone();
                    OnCompleteEvent?.Invoke(this);
                });
            }
        }

        public override void OnClickDown()
        {
            base.OnClickDown();
            if(Hintrice!=null) {LevelControl.Ins.SetHint(Hintrice); }
            if (IsState(State.donewash))
            {
                Spankelow.Stop();
                Spankelow.transform.SetParent(this.transform);
            }
        }

        public override void ChangeState<T>(T t)
        {
            this.state = (State)(object)t;

            switch (state)
            {
                case State.dirty:
                    break;

                case State.wash:
                    totalRotation = 0f;
                    washProgress = 0f;
                    hasStartedFloatingEffect = false;
                    hasPreparedFloating = false;

                    if (progressBarRoot != null)
                    {
                        progressBarRoot.transform.localScale = new Vector3(0f, 1f, 1f);
                        progressBarRoot.SetActive(true);
                        progressBarRoot.transform.DOScaleX(1f, 1f).SetEase(Ease.OutBack);
                    }

                    if (progressBarFill != null)
                        progressBarFill.fillAmount = 0f;

                    foreach (var sr in riceUnwashSprites) SetAlpha(sr, 1f);
                    foreach (var sr in riceWashSprites) SetAlpha(sr, 0f);
                    break;


                case State.donewash:
                    waterSink.OnRiceDone();
                    Debug.Log("✅ Đã vo gạo đủ 2 vòng!");
                    DetachGrains();

                    foreach (var sr in riceUnwashSprites) SetAlpha(sr, 0f);
                    foreach (var sr in riceWashSprites) SetAlpha(sr, 1f);
                    Spankelow.transform.SetParent(null);

                    Spankelow.Play();
                    SoundControl.Ins.PlayFX(SoundFXEnum.Soundname.blingbling);

                    if (progressBarRoot != null)
                    {
                        progressBarRoot.transform.DOScaleX(0f, 0.2f)
                            .SetEase(Ease.InBack)
                            .OnComplete(() => progressBarRoot.SetActive(false));
                    }

                    if (progressBarFill != null)
                        progressBarFill.fillAmount = 1f;

                    foreach (Transform grain in grains)
                    {
                        SpriteRenderer sr = grain.GetComponent<SpriteRenderer>();
                        if (sr != null)
                            sr.sortingOrder = 25;
                    }

                    break;

            }
        }

        private void SetAlpha(SpriteRenderer sr, float alpha)
        {
            if (sr == null) return;
            var c = sr.color;
            c.a = alpha;
            sr.color = c;
        }

        private void UpdateWashBlend()
        {
            float smooth = Mathf.SmoothStep(0f, 1f, washProgress);

            foreach (var sr in riceUnwashSprites) SetAlpha(sr, 1f - smooth);
            foreach (var sr in riceWashSprites) SetAlpha(sr, smooth);
        }

        private void PrepareGrainFloating()
        {
            grainStartPositions.Clear();
            grainTargetPositions.Clear();

            foreach (Transform grain in grains)
            {
                Vector3 start = grain.position;
                grainStartPositions[grain] = start;

                float offsetX = UnityEngine.Random.Range(-maxOffsetX, maxOffsetX);
                float offsetY = UnityEngine.Random.Range(0f, maxOffsetY);
                Vector3 target = start + new Vector3(offsetX, offsetY, 0f);
                grainTargetPositions[grain] = target;

                grain.DOMove(start, 0.2f).SetEase(Ease.InOutSine);
            }
        }

        private void DetachGrains()
        {
            foreach (Transform grain in grains)
            {
                grain.SetParent(null);
                grain.tag = "Grain";

                var sr = grain.GetComponent<SpriteRenderer>();
                if (sr != null) sr.sortingOrder = 10;
            }

            grainStartPositions.Clear();
            grainTargetPositions.Clear();
        }

        private void StartFloatingEffect()
        {
            foreach (Transform grain in grains)
            {
                grain.DOMoveY(grain.position.y + 0.1f, 1f)
                     .SetLoops(-1, LoopType.Yoyo)
                     .SetEase(Ease.InOutSine);
            }
        }

        private void Update()
        {
            if (state != State.wash) return;

            if (Input.GetMouseButtonDown(0))
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                int mask = ~LayerMask.GetMask("Water"); // loại Water ra khỏi raycast
                RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, mask);

                if (hit.collider != null)
                {
                    Debug.Log("Raycast hit: " + hit.collider.name);
                    if (hit.collider.gameObject == this.gameObject)
                    {
                        isTouching = true;
                        lastDirection = (mousePos - (Vector2)transform.position).normalized;
                    }
                }
            }


            if (Input.GetMouseButton(0) && isTouching)
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3 currentDirection = (mousePos - (Vector2)transform.position).normalized;

                float angle = Vector2.SignedAngle(lastDirection, currentDirection);
                float slowFactor = 0.2f;

                ricebound.Play();
                Riceunwash.transform.Rotate(0, 0, angle * slowFactor);
                totalRotation += Mathf.Abs(angle * slowFactor);
                washProgress = Mathf.Clamp01(totalRotation / 720f);
                lastDirection = currentDirection;

                if (progressBarFill != null)
                    progressBarFill.fillAmount = washProgress;

                if (totalRotation >= minRotationToMoveGrain && !hasPreparedFloating)
                {
                    hasPreparedFloating = true;
                    PrepareGrainFloating();
                }

                UpdateGrainFloating();
                UpdateWashBlend();

                if (!hasStartedFloatingEffect && totalRotation >= 360f)
                {
                    hasStartedFloatingEffect = true;
                    StartFloatingEffect();
                }

                if (totalRotation >= 720f)
                {
                    ChangeState(State.donewash);
                    isTouching = false;
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                isTouching = false;
                ricebound.Stop();
            }
        }

        private void UpdateGrainFloating()
        {
            if (!hasPreparedFloating) return;

            float progress = Mathf.Clamp01(totalRotation / 720f);

            foreach (Transform grain in grains)
            {
                if (!grainStartPositions.ContainsKey(grain)) continue;
                if (!grainTargetPositions.ContainsKey(grain)) continue;

                Vector3 start = grainStartPositions[grain];
                Vector3 target = grainTargetPositions[grain];

                grain.position = Vector3.Lerp(start, target, progress);
            }
        }
    }
}
