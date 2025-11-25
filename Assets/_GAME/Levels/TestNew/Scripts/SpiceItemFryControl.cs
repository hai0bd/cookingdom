using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Hai.Cooking.NewTest
{
    public class SpiceItemFryControl : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer actionAnim;
        [SerializeField] private SortingGroup sortingGroup;

        [SerializeField] private SpiceItemFry[] itemFries;

        public void OnInit(int orderLayer)
        {
            sortingGroup.sortingOrder = orderLayer;
            gameObject.SetActive(true);
            if (actionAnim != null)
            {
                actionAnim.sortingOrder = orderLayer;
                actionAnim.enabled = true;
            }
            SetControl(false);
        }

        public void SetControl(bool active)
        {
            foreach (var item in itemFries)
            {
                item.SetControl(active);
            }
        }

        public void SetRipeRate(float rate)
        {
            foreach (var item in itemFries)
            {
                item.SetRipeRate(rate);
            }
        }

        public void SetAlpha(float alpha)
        {
            foreach (var item in itemFries)
            {
                item.SetAlpha(alpha);
            }
        }

        public void SetRadius(float radius)
        {
            foreach (var item in itemFries)
            {
                item.SetRadius(radius);
            }
        }

        
    }
}