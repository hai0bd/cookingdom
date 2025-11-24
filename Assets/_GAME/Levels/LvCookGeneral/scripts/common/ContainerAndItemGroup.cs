using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TidyCooking.Levels.ContainerAndItemGroup;

namespace TidyCooking.Levels
{
    public class ContainerAndItemGroup : MonoBehaviour
    {
        public enum ActionWhenNoContainer
        {
            None,
            AutoPlaceNearestContainer,
            ReturnLastContainer,
        }

        [SerializeField] private CookItemAbstract[] _allItems;
        [SerializeField] private CookItemContainer[] _containers;
        [SerializeField] private ActionWhenNoContainer _actionWhenNoContainer = ActionWhenNoContainer.ReturnLastContainer;

        private void Awake()
        {
            foreach (var item in _allItems)
            {
                item.onStartDragItem += OnStartDragItem;
                item.onEndDragItem += OnEndDragItem;
            }
        }

        private void OnStartDragItem(CookItemAbstract item)
        {

        }

        private void OnEndDragItem(CookItemAbstract item)
        {
            CookItemContainer thisContainer = item as CookItemContainer;

            List<CookItemContainer> availableContainers = new List<CookItemContainer>();
            foreach (var container in _containers)
            {
                if (thisContainer)
                {
                    if (container != thisContainer && !thisContainer.IsContainsItem(container) && container.IsCanAddItemAtPosition(thisContainer))
                    {
                        availableContainers.Add(container);
                    }
                }
                else
                {
                    if (container.IsCanAddItemAtPosition(item))
                    {
                        availableContainers.Add(container);
                    }
                }
            }

            if (availableContainers.Count > 0)
            {
                // find container that have highest sorting order, then "hightest" z position (the more negative the better)
                CookItemContainer destinationContainer = availableContainers[0];
                foreach (var container in availableContainers)
                {
                    if (CookItemAbstract.CompareZPos(container, destinationContainer) > 0)
                    {
                        destinationContainer = container;
                    }
                }

                destinationContainer.AddItem(item);
            }
            else
            {
                if (_actionWhenNoContainer == ActionWhenNoContainer.AutoPlaceNearestContainer)
                {
                    // go to nearest available container
                    float minDistance = float.MaxValue;
                    CookItemContainer nearestContainer = null;

                    foreach (var container in _containers)
                    {
                        if (thisContainer)
                        {
                            if (container != thisContainer && !thisContainer.IsContainsItem(container) && container.IsAllowAddItem(thisContainer))
                            {
                                float distance = Vector3.Distance(item.transform.position, container.transform.position);
                                if (distance < minDistance)
                                {
                                    minDistance = distance;
                                    nearestContainer = container;
                                }
                            }
                        }
                        else
                        {
                            if (container.IsAllowAddItem(item))
                            {
                                float distance = Vector3.Distance(item.transform.position, container.transform.position);
                                if (distance < minDistance)
                                {
                                    minDistance = distance;
                                    nearestContainer = container;
                                }
                            }
                        }
                    }

                    if (nearestContainer != null)
                    {
                        nearestContainer.AddItem(item);
                    }
                }
                else if (_actionWhenNoContainer == ActionWhenNoContainer.ReturnLastContainer && item.Container)
                {
                    item.Container.AddItem(item);
                }
                else
                {
                    // do nothing
                }
            }
        }

#if UNITY_EDITOR
        [Sirenix.OdinInspector.Button]
        private void LoadAllContainerAndItemsEditor()
        {
            // find all inside recursively
            List<CookItemContainer> containers = new List<CookItemContainer>();
            List<CookItemAbstract> items = new List<CookItemAbstract>();

            GetChildRecursively(transform);

            void GetChildRecursively(Transform parent)
            {
                foreach (Transform child in parent)
                {
                    if (child.TryGetComponent<CookItemAbstract>(out var item))
                    {
                        items.Add(item);

                        if (child.TryGetComponent<CookItemContainer>(out var container))
                        {
                            containers.Add(container);
                        }
                    }
                    GetChildRecursively(child);
                }
            }

            _containers = containers.ToArray();
            _allItems = items.ToArray();

            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }
}