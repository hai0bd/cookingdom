using DG.Tweening;
using Link;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LawNguyen.CookingGame.LRisottoY
{
    public class CHese : ItemMovingBase
    {
        [Header("Shave Effect")]
        [SerializeField] GameObject cheseStripPrefab;
        [SerializeField] Transform dropPoint;
        [SerializeField] GameObject cheseOnGlass;
        [SerializeField] Transform BlowglassTransform;

        [Header("Spawn Random Range")]
        [SerializeField] float dropRangeX = 0.2f;
        [SerializeField] float dropRangeY = 0.1f;

        [Header("Target Random Range")]
        [SerializeField] float targetRangeX = 0.25f;
        [SerializeField] float targetRangeY = 0.1f;

        [Header("Related")]
        [SerializeField] GameObject crapObject;

        [Header("Progress UI")]
        [SerializeField] private Image progressBarFill;
        [SerializeField] private GameObject progressBarRoot;
        [SerializeField] Sprite Hintchesecrap;

        public event Action<UnityEngine.Object> OnCompleteEvent;

        private bool cancrap = false;
        private bool isShaving = false;
        private bool hasShaved = false;
        private bool hasFadedGlass = false;
        private Coroutine shavingRoutine = null;

        private Vector3 lastWorldPos;
        private float moveDistance = 0f;
        private float currentScale = 1f;
        private int savedShaveCount = 0;

        private List<GameObject> strips = new List<GameObject>();
        private Transform cheseSpriteTF;

        private void Start()
        {
            DeActiveboxcolider();
            cheseSpriteTF = GetComponentInChildren<SpriteRenderer>().transform;
            currentScale = cheseSpriteTF.localScale.x;

            if (progressBarRoot != null)
                progressBarRoot.SetActive(false);
        }

        public void Activeboxcolider()
        {
            collider.enabled = true;

            // ✅ Hiện progress bar ngay khi bắt đầu
            if (progressBarRoot != null)
            {
                progressBarRoot.transform.localScale = new Vector3(0f, 1f, 1f);
                progressBarRoot.SetActive(true);
                progressBarRoot.transform.DOScaleX(1f, 0.3f).SetEase(Ease.OutBack);
            }

            if (progressBarFill != null)
                progressBarFill.fillAmount = savedShaveCount / 35f;
        }

        public void DeActiveboxcolider()
        {
            collider.enabled = false;
        }

        public void Setparentnull() => transform.SetParent(null);

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Crap") && !hasShaved)
            {
                cancrap = true;
                lastWorldPos = transform.position;
                cheseSpriteTF.localScale = Vector3.one * currentScale;
                moveDistance = 0f;
                SoundControl.Ins.PlayFX(SoundFXEnum.Soundname.grater1,true);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Crap"))
            {
                cancrap = false;
                isShaving = false;

                if (shavingRoutine != null)
                {
                    StopCoroutine(shavingRoutine);
                    shavingRoutine = null;
                }
                SoundControl.Ins.StopFX(SoundFXEnum.Soundname.grater1);
            }
        }

        private void Update()
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (shavingRoutine != null)
                {
                    StopCoroutine(shavingRoutine);
                    SoundControl.Ins.StopFX(SoundFXEnum.Soundname.grater1);
                    shavingRoutine = null;
                    isShaving = false;
                }
            }

            if (!cancrap || isShaving || hasShaved || !Input.GetMouseButton(0)) return;

            float delta = Vector2.Distance(transform.position, lastWorldPos);
            if (delta < 0.001f) return;

            moveDistance += delta;
            lastWorldPos = transform.position;

            if (moveDistance >= 0.1f)
            {
                if (shavingRoutine == null)
                {
                    shavingRoutine = StartCoroutine(ShaveChese());
                }
                moveDistance = 0f;
            }
        }

        private IEnumerator ShaveChese()
        {
            isShaving = true;
            int count = 35;

            for (int i = savedShaveCount; i < count; i++)
            {
                if (!cancrap)
                {
                    isShaving = false;
                    savedShaveCount = i;
                    shavingRoutine = null;
                    yield break;
                }

                GameObject strip = SpawnStrip();
                strips.Add(strip);

                if (i < 25)
                {
                    float t = Mathf.InverseLerp(0, 25, i);
                    currentScale = Mathf.Lerp(0.5f, 0.3f, t);
                    cheseSpriteTF.DOScale(currentScale, 0.02f).SetEase(Ease.OutSine);
                }

                savedShaveCount = i + 1;

                if (progressBarFill != null)
                    progressBarFill.fillAmount = (i + 1) / (float)count;

                yield return new WaitForSeconds(0.15f);
            }

            yield return new WaitForSeconds(0.3f);

            if (!hasFadedGlass)
            {
                hasFadedGlass = true;

                var srGlass = cheseOnGlass.GetComponentInChildren<SpriteRenderer>();
                srGlass.DOKill();
                srGlass.color = new Color(srGlass.color.r, srGlass.color.g, srGlass.color.b, 0f);
                srGlass.DOFade(1f, 0.5f);
            }

            foreach (var strip in strips)
            {
                if (strip != null)
                {
                    var sr = strip.GetComponentInChildren<SpriteRenderer>();
                    if (sr != null)
                        sr.DOFade(0f, 0.5f);
                }
            }

            yield return new WaitForSeconds(0.6f);
            strips.ForEach(Destroy);
            strips.Clear();
            shavingRoutine = null;

            // ✅ Tắt progress bar mượt
            if (progressBarRoot != null)
            {
                progressBarRoot.transform.DOScaleX(0f, 0.2f)
                    .SetEase(Ease.InBack)
                    .OnComplete(() => progressBarRoot.SetActive(false));
            }

            OnDone1();
        }

        private GameObject SpawnStrip()
        {
            Vector3 spawnPos = dropPoint.position + new Vector3(
                UnityEngine.Random.Range(-dropRangeX, dropRangeX),
                UnityEngine.Random.Range(-dropRangeY, dropRangeY),
                0);

            GameObject strip = Instantiate(cheseStripPrefab, spawnPos, Quaternion.identity);

            Vector3 target = BlowglassTransform.position + new Vector3(
                UnityEngine.Random.Range(-targetRangeX, targetRangeX),
                UnityEngine.Random.Range(-targetRangeY, targetRangeY),
                0) + new Vector3(0, -0.2f, 0);

            strip.transform.DOMove(target, 0.5f).SetEase(Ease.InQuad);
            strip.transform.DORotate(new Vector3(0, 0, UnityEngine.Random.Range(-180f, 180f)), 0.5f, RotateMode.FastBeyond360);
            strip.transform.DOScale(new Vector3(1, 1.2f, 1), 0.25f).SetLoops(2, LoopType.Yoyo);

            var sr = strip.GetComponentInChildren<SpriteRenderer>();
            if (sr != null)
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f);

            return strip;
        }

        public override void OnBack()
        {
            base.OnBack();
            hasFadedGlass = false;

            if (progressBarRoot != null)
                progressBarRoot.SetActive(false);

            DOVirtual.DelayedCall(0.4f, () => OrderLayer = 11);
        }
        public override void OnClickDown()
        {
            base.OnClickDown();
            if (Hintchesecrap != null) LevelControl.Ins.SetHint(Hintchesecrap);
        }
        public void OnDone1()
        {
            hasShaved = true;
            isShaving = false;

            if (crapObject != null && crapObject.TryGetComponent<ItemMovingBase>(out var crapDone))
            {
                crapDone.OnBack();
                var col = crapDone.GetComponent<Collider2D>();
                if (col != null) col.enabled = false;
            }

            DeActiveboxcolider();
            cheseSpriteTF.gameObject.SetActive(false);
            OnCompleteEvent?.Invoke(this);
        }

        public override void OnDrop()
        {
            base.OnDrop();
            moveDistance = 0f;
        }
    }
}
