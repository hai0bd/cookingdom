using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

namespace HuyThanh.Cooking.Burrito
{
    public class CornSeed : MonoBehaviour
    {
        public bool OrderSortAtRoot
        {
            get => sprite.sortAtRoot;
            set
            {
                sprite.sortAtRoot = value;
            }
        }
        public int OrderLayer
        {
            get => sprite.sortingOrder;
            set
            {
                sprite.sortingOrder = value;
            }
        }

        [SerializeField] protected SortingGroup sprite;
        [SerializeField] float deltaY;

        [Button]
        protected virtual void Editor()
        {
            sprite = GetComponent<SortingGroup>();
            if (sprite == null)
            {
                sprite = gameObject.AddComponent<SortingGroup>();
            }
        }

        public void DoFinishMove()
        {
            OrderSortAtRoot = true;

            transform.DOMove(transform.position + Vector3.up * deltaY, 0.2f).SetEase(Ease.OutSine).OnComplete(() =>
            {
                OrderSortAtRoot = false;// Reset sorting order after moving
            });
        }
    }
}