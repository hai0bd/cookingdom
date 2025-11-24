using UnityEngine;
using Link;
using DG.Tweening;

namespace LawNguyen.CookingGame.LRisottoY
{
    public class Seafoodend : ItemMovingBase
    {
        public enum SeefoodType { Shrimp, Museel, Clam }
        [SerializeField] public SeefoodType seefoodType;
     
        public void MoveInPlate(Transform point)
        {
            transform.DOMove(point.position, 0.5f).SetEase(Ease.OutBack);
            transform.DORotate(point.rotation.eulerAngles, 0.5f).SetEase(Ease.OutBack);
        }

     

        public void SetCollider(bool isActive)
        {
            Collider2D col = GetComponent<Collider2D>();
            if (col != null) col.enabled = isActive;
        }

        public void SetLayer(int sortingOrder)
        {
            SetOrder(sortingOrder);
        }
    }
}
