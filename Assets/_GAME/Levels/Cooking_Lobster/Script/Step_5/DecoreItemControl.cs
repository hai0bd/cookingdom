using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;


namespace Link.Cooking.Lobster
{
    public class DecoreItemControl : MonoBehaviour
    {
        public DecoreItem.NameType ItemType;
        [SerializeField] List<DecoreItem> itemDecors = new List<DecoreItem>();

        [SerializeField] UnityEvent OnDoneEvent;

        [SerializeField] HintText hintText;

        void Awake()
        {
            foreach (var item in itemDecors)
            {
                item.OnDoneEvent.AddListener(ItemDone);
            }
        }

        private void ItemDone(DecoreItem item)
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
            DecoreItem[] items = FindObjectsOfType<DecoreItem>(true);
            foreach (var item in items)
            {
                if (item.Name == ItemType)
                {
                    itemDecors.Add(item);
                }
            }
        }
    }
}