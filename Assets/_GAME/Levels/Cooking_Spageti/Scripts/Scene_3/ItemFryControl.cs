using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

namespace Link.Cooking.Spageti
{
    public class ItemFryControl : MonoBehaviour
    {
        [SerializeField] AnimaBase2D actionAnim;
        [SerializeField] SortingGroup sortingGroup;

        [field: SerializeField] public ItemFry[] ItemFries { get; private set; } 
        public void OnInit(int orderLayer)
        {
            sortingGroup.sortingOrder = orderLayer;
            gameObject.SetActive(true);
            if(actionAnim != null) actionAnim.OnActive();
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
            ItemFries = GetComponentsInChildren<ItemFry>(); 
            foreach (var item in ItemFries)
            {
                item.Setup();
            }
        }
    }
}