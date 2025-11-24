using DG.Tweening;
using System.Collections;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

namespace LawNguyen.CookingGame.LRisottoY
{
    public class SeafoodStirEffect : MonoBehaviour
    {
        public enum SeafoodType { Shrimp, Mussel, Clam }

        [Header("Loại hải sản")]
        [SerializeField] private SeafoodType seafoodType;

        [Header("Renderer và Effect")]
        [SerializeField] private SpriteRenderer seafoodSR;
        [SerializeField] private GameObject seafoodCookDone;
        [SerializeField] CircleCollider2D circleCollider;

        public bool isDOnecook = false;
        private bool isInSpoon = false;
        private bool isOnPlate = false;
        private bool canBeTakenBySpoon = true;

        private Spoon3 currentSpoon;

        public void SetLayer(int layer)
        {
            if (seafoodSR != null)
                seafoodSR.sortingOrder = layer;

            if (seafoodCookDone != null)
            {
                var sr = seafoodCookDone.GetComponent<SpriteRenderer>();
                if (sr != null)
                    sr.sortingOrder = layer + 1; // CookDone luôn nằm trên
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!isDOnecook) return;

            // Va Spoon
            if (!isInSpoon && !isOnPlate && canBeTakenBySpoon &&
                other.TryGetComponent(out Spoon3 spoon) && !spoon.HaveSeafood)
            {
                SetLayer(55);
                spoon.TakeSeafood(transform);
                isInSpoon = true;
                currentSpoon = spoon;
                return;
            }

            // Va Plate
            if (!isOnPlate && other.TryGetComponent(out Plateend plate))
            {
                bool matchType =
                    (seafoodType == SeafoodType.Shrimp && plate.plateType == Plateend.PlateType.Shrimp) ||
                    (seafoodType == SeafoodType.Mussel && plate.plateType == Plateend.PlateType.Museel) ||
                    (seafoodType == SeafoodType.Clam && plate.plateType == Plateend.PlateType.Clam);

                if (matchType)
                {
                    Transform slot = plate.GetNextSlot(out int slotIndex);
                    if (slot != null)
                    {
                        if (currentSpoon != null)
                        {
                            currentSpoon.DropSeafood();
                            currentSpoon = null;
                        }

                        isOnPlate = true;
                        isInSpoon = false;

                        SetLayer(55 + slotIndex); // Layer theo index
                        moveinplate(slot,slotIndex);

                        StartCoroutine(DisableSpoonPickupTemporarily());
                    }
                }
            }
        }

        private void moveinplate(Transform slot ,int slotindex)
        {
            transform.SetParent(slot);
            transform.DOLocalMove(Vector3.zero, 0.5f);
            transform.DOLocalRotate(Vector3.zero, 0.5f);
            DOVirtual.DelayedCall(0.5f, () => {
                
                SetLayer(slotindex);

                circleCollider.enabled = false;
            });
           

        }

        private IEnumerator DisableSpoonPickupTemporarily()
        {
            canBeTakenBySpoon = false;
            yield return new WaitForSeconds(0.3f);
            canBeTakenBySpoon = true;
        }

        public void ResetFlags()
        {
            isInSpoon = false;
            isOnPlate = false;
            currentSpoon = null;
            canBeTakenBySpoon = true;
        }

        public void DoneCooking()
        {
            if (seafoodSR != null)
                seafoodSR.gameObject.SetActive(false);

            if (seafoodCookDone != null)
            {
                seafoodCookDone.SetActive(true);
               
            }
               
              
            isDOnecook = true;
        }
    }
}
