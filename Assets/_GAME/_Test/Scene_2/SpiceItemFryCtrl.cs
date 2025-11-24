using Link.Cooking.Spageti;
using Link;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace HoangLinh.Cooking.Test
{
    public class SpiceItemFryCtrl : MonoBehaviour
    {
        [SerializeField] AnimaBase2D actionAnim;
        [SerializeField] SortingGroup sortingGroup;

        [field: SerializeField] public SpiceItemFry[] ItemFry { get; private set; }
        public void OnInit(int orderLayer)
        {
            sortingGroup.sortingOrder = orderLayer;
            gameObject.SetActive(true);
            if (actionAnim != null) actionAnim.OnActive();
            SetControl(false);
        }

        public void SetControl(bool active)
        {
            foreach (var item in ItemFry)
            {
                    item.SetControl(active);
            }
        }

        public void SetRipeRate(float rate)
        {
            foreach (var item in ItemFry)
            {
                item.SetRipeRate(rate);
            }
        }


        public void SetAlpha(float alpha)
        {
            foreach (var item in ItemFry)
            {
                item.SetAlpha(alpha);
            }
        }

        public void SetRadius(float radius)
        {
            foreach (var item in ItemFry)
            {
                item.SetRadius(radius);
            }
        }

        [Button]
        protected void Editor()
        {
            sortingGroup = GetComponent<SortingGroup>();
            actionAnim = GetComponentInChildren<AnimaBase2D>();
            ItemFry = GetComponentsInChildren<SpiceItemFry>();
            foreach (var item in ItemFry)
            {
                item.Setup();
            }
        }
    }
}
