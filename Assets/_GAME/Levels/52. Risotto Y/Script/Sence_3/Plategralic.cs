using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Link;
using DG.Tweening;

namespace LawNguyen.CookingGame.LRisottoY
{
    public class Plategralic : ItemMovingBase
    {
       

        [Header("Gralic Items")]
        [SerializeField] List<Transform> itemgralic;
        [SerializeField] StoveTapOn stoveTapOn;
        bool graliciPot = false;
        private void OnEnable()
        {
            stoveTapOn.PotisTurnedOn += ShankGralic;
        }
        private void OnDisable()
        {
            stoveTapOn.PotisTurnedOn -= ShankGralic;
        }


      
        public void ScatterGralicToPot(Transform potTransform , Transform gralicparenttransform)
        {
            if (itemgralic == null || itemgralic.Count == 0 || potTransform == null)
                return;

            Vector3 center = potTransform.position + new Vector3(0f, -0.1f, 0);
            int[] itemsPerCircle = { 5, 8 };
            float[] radii = { 0.25f, 0.45f };

            int itemIndex = 0;
            for (int circle = 0; circle < itemsPerCircle.Length; circle++)
            {
                int count = itemsPerCircle[circle];
                float radius = radii[circle];

                for (int i = 0; i < count && itemIndex < itemgralic.Count; i++, itemIndex++)
                {
                    Transform item = itemgralic[itemIndex];
                    if (item == null) continue;

                    DOTween.Kill(item);
                    item.SetParent(null);

                    Vector3 originalScale = item.localScale;
                    float angleDeg = (360f / count) * i;
                    float angleRad = angleDeg * Mathf.Deg2Rad;
                    Vector3 offset = new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad)) * radius;
                    Vector3 targetPos = center + offset;

                    float jumpPower = 0.45f;
                    float jumpDuration = 1.0f;

                    item.DOJump(targetPos, jumpPower, 1, jumpDuration)
                        .SetEase(Ease.OutQuad)
                        .SetLink(item.gameObject);

                    item.DOScale(originalScale * 0.9f, 0.5f)
                        .SetEase(Ease.OutExpo)
                        .SetLink(item.gameObject);

                    DOVirtual.DelayedCall(jumpDuration + 0.1f, () =>
                    {
                        if (item != null)
                        {
                            item.SetParent(gralicparenttransform);
                            // Set layer garlic = -20
                            var sr = item.GetComponent<SpriteRenderer>();
                            if (sr != null) sr.sortingOrder = -20;
                        }
                    }).SetLink(item.gameObject).SetUpdate(true);
                }
            }

           graliciPot=true;
            DOVirtual.DelayedCall(2f, () => {
                ShankGralic();
            });
        }


        private void DeactiveGralic()
        {

        }

        private void ShankGralic()
        {
            if (!graliciPot) return;   

            for (int i = 0; i < itemgralic.Count; i++)
            {
                Transform obj = itemgralic[i];

                // Dừng mọi tween đang hoạt động trên obj (nếu có)
                obj.DOKill();

                // Nếu bếp đang bật thì bắt đầu rung
                if (stoveTapOn.Ison)
                {
                    obj.DOShakePosition(
                        duration: 0.1f,
                        strength: new Vector3(0.01f, 0.01f, 0),
                        vibrato: 10,
                        randomness: 90f,
                        fadeOut: true
                    )
                    .SetLoops(-1)
                    .SetTarget(obj)            // ✅ Quan trọng để DOKill hoạt động chính xác
                    .SetLink(obj.gameObject);  // Auto-kill nếu object bị Destroy
                }
            }
        }





        public override void OnBack()
        {
            base.OnBack();
            SetOrder(-25);
        }

        public override void OnDrop()
        {
            base.OnDrop();
        }

       
    }
}

