using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Link.Cooking.Spageti
{
    public class ItemDecorsControl : MonoBehaviour
    {
        public ItemType ItemType;
        [SerializeField] List<ItemDecors> itemDecors = new List<ItemDecors>();

        [SerializeField] UnityEvent OnDoneEvent;

        [SerializeField] HintText hintText;

        void Awake()
        {
            foreach (var item in itemDecors)
            {
                item.unityEvent.AddListener(ItemDone);
            }
        }

        private void ItemDone(ItemDecors item)
        {
            itemDecors.Remove(item);
            if (itemDecors.Count == 0)
            {
                OnDoneEvent?.Invoke();
                hintText.OnActiveHint();
            }
        }


        [Button]
        private void SearchItem()
        {
            itemDecors.Clear();
            ItemDecors[] items = FindObjectsOfType<ItemDecors>(true);
            foreach (var item in items)
            {
                if (item.ItemType == ItemType)
                {
                    itemDecors.Add(item);
                }
            }
        }
    }
}