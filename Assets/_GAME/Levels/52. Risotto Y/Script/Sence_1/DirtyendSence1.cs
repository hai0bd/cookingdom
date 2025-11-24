using System.Collections.Generic;
using UnityEngine;
using Link;
using System;
using DG.Tweening;

namespace LawNguyen.CookingGame.LRisottoY
{
    [RequireComponent(typeof(Collider2D))]
    public class DirtyendSence1 : ItemIdleBase
    {
        public event Action<UnityEngine.Object> OnCompleteDirtyEvent;

        [Header("Nhiều vết bẩn riêng")]
        [SerializeField] private List<SpriteRenderer> dirtySpots = new List<SpriteRenderer>();
        [SerializeField] private int cleanRequired = 5;

        private Dictionary<SpriteRenderer, int> cleanCounts = new Dictionary<SpriteRenderer, int>();
        private bool isCleaned = false;

        private void Awake()
        {
            foreach (var spot in dirtySpots)
            {
                if (spot != null && !cleanCounts.ContainsKey(spot))
                {
                    cleanCounts[spot] = 0;
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (isCleaned) return;

            var towel = collision.GetComponent<Towel>();
            if (towel == null) return;

            Vector3 towelPos = collision.transform.position;
            SpriteRenderer closestSpot = GetClosestDirtySpot(towelPos);

            if (closestSpot != null && closestSpot.color.a > 0.05f)
            {
                cleanCounts[closestSpot]++;
                FadeDirty(closestSpot);
            }
        }

        private SpriteRenderer GetClosestDirtySpot(Vector3 position)
        {
            float minDist = float.MaxValue;
            SpriteRenderer closest = null;

            foreach (var spot in dirtySpots)
            {
                if (spot == null || spot.color.a <= 0.05f) continue;

                float dist = Vector2.Distance(spot.transform.position, position);
                if (dist < minDist)
                {
                    minDist = dist;
                    closest = spot;
                }
            }

            return closest;
        }

        private void FadeDirty(SpriteRenderer spot)
        {
            int count = cleanCounts[spot];
            float targetAlpha = 1f - (count / (float)cleanRequired);
            targetAlpha = Mathf.Clamp01(targetAlpha);

            spot.DOFade(targetAlpha, 0.3f).OnComplete(() =>
            {
                Debug.Log($"🧽 {spot.name} alpha = {targetAlpha:F2} (clean count: {count}/{cleanRequired})");

                if (AllSpotsClean() && !isCleaned)
                {
                    Debug.Log("✅ All spots cleaned, trigger complete.");
                    isCleaned = true;
                    OnCompleteDirtyEvent?.Invoke(this);
                    gameObject.SetActive(false);
                }
            });
        }

        private bool AllSpotsClean()
        {
            foreach (var spot in dirtySpots)
            {
                if (spot != null)
                {
                    Debug.Log($"🔍 Check {spot.name}: alpha = {spot.color.a:F2}");
                    if (spot.color.a > 0.05f)
                        return false;
                }
            }
            return true;
        }
    }
}
