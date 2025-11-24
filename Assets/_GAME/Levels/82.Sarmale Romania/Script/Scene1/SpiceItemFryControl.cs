using Link;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

namespace HuyThanh.Cooking.SarmaleRomania
{
    public class SpiceItemFryControl : MonoBehaviour
    {
        [SerializeField] AnimaBase2D actionAnim;
        [SerializeField] SortingGroup sortingGroup;

        [field: SerializeField] public SpiceItemFry[] ItemFries { get; private set; }
        public void OnInit(int orderLayer)
        {
            sortingGroup.sortingOrder = orderLayer;
            gameObject.SetActive(true);
            if (actionAnim != null) actionAnim.OnActive();
            SetControl(false);
        }

        public void SetControl(bool active)
        {
            foreach (var item in ItemFries)
            {
                item.SetControl(active);
            }
        }

        public void SetRipeRate(float rate)
        {
            foreach (var item in ItemFries)
            {
                item.SetRipeRate(rate);
            }
        }

        public void SetAlpha(float alpha)
        {
            foreach (var item in ItemFries)
            {
                item.SetAlpha(alpha);
            }
        }

        public void SetRadius(float radius)
        {
            foreach (var item in ItemFries)
            {
                item.SetRadius(radius);
            }
        }

        [Button]
        protected void Editor()
        {
            sortingGroup = GetComponent<SortingGroup>();
            actionAnim = GetComponentInChildren<AnimaBase2D>();
            ItemFries = GetComponentsInChildren<SpiceItemFry>();
            foreach (var item in ItemFries)
            {
                item.Setup();
            }
        }
    }
}
