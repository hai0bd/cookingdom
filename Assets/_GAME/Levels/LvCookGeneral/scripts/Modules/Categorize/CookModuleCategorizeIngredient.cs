using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities;

namespace TidyCooking.Levels
{
    public class CookModuleCategorizeIngredient : MonoBehaviour, ICookModule
    {
        [Header("Containers")]
        [SerializeField] private GameObject _objContainerItemsPicked;
        [EnableIf("_objContainerItemsPicked")]
        [SerializeField] private bool _isPlayAnimShowContainerItemsPickedOnStartModule = true;
        private ICookIngredientContainer _containerItemsPicked;
        private Vector3 _originScaleContainerItemsPicked;

        [SerializeField] private GameObject _objContainerItemsDiscarded;
        [EnableIf("_objContainerItemsDiscarded")]
        [SerializeField] private bool _isPlayAnimShowContainerItemsDiscardedOnStartModule = true;
        private ICookIngredientContainer _containerItemsDiscarded;
        private Vector3 _originScaleContainerItemsDiscarded;

        [SerializeField] private bool _isBlockDragContainerOnEndModule = true;

        [Header("Items")]
        [SerializeField] private GameObject[] _allObjItems;
        [SerializeField] private GameObject[] _objsMustPick;
        [SerializeField] private GameObject[] _objsOptionalPick;

        private List<ICookPickableIngredient> _allItems;
        private List<ICookPickableIngredient> _itemsMustPick;
        private List<ICookPickableIngredient> _itemsOptionalPick;

        [Header("Behavior")]
        [SerializeField] private bool _isPlayAnimShowItemsOnStartModule = true;
        [EnableIf("_isPlayAnimShowItemsOnStartModule")]
        [DrawWithUnity][SerializeField] private AnimShowItemConfig _animShowItemConfig;

        [System.Serializable]
        public class AnimShowItemConfig
        {
            public float delayShowContainer = 0f;
            public float delayShowItem = 1f;
            public float durationShow = 0.5f;
            public float delayBetweenItems = 0.1f;

            public Vector3 offsetStarPosShow = new Vector3(0, 0.5f, 0);
            public AnimationCurve curveFly = AnimationCurve.EaseInOut(0, 0, 1, 1);
            public AnimationCurve curveScale = AnimationCurve.EaseInOut(0, 0, 1, 1);
        }

        private Action<CookModuleCategorizeIngredient> _onEnd;
        public bool IsEnded { get; private set; }

        #region Internal Setup
        private void Awake()
        {
            _containerItemsPicked = _objContainerItemsPicked.GetComponent<ICookIngredientContainer>();
            if (_containerItemsPicked == null) Debug.LogError("Container Items Picked is missing ICookIngredientToPickOrDiscard component");

            if (_objContainerItemsDiscarded)
            {
                _containerItemsDiscarded = _objContainerItemsDiscarded.GetComponent<ICookIngredientContainer>();
                if (_containerItemsDiscarded == null) Debug.LogError("Container Items Discarded is missing ICookIngredientToPickOrDiscard component");
            }

            _allItems = new List<ICookPickableIngredient>(_allObjItems.Length);
            for (int i = 0; i < _allObjItems.Length; i++)
            {
                if (_allObjItems[i].TryGetComponent(out ICookPickableIngredient item))
                {
                    if (_allItems.Contains(item))
                    {
                        Debug.LogError("Item is duplicated: " + item.ItemTransform.name);
                        continue;
                    }

                    _allItems.Add(item);
                    item.onEndDrag += OnEndDragItem;
                }
                else
                {
                    Debug.LogError("Item " + _allObjItems[i].name + " is missing ICookIngredientToPickOrDiscard component");
                }
            }

            _itemsMustPick = new List<ICookPickableIngredient>(_objsMustPick.Length);
            for (int i = 0; i < _objsMustPick.Length; i++)
            {
                if (_objsMustPick[i].TryGetComponent(out ICookPickableIngredient item))
                {
                    if (_itemsMustPick.Contains(item))
                    {
                        Debug.LogError("Must Pick Item is duplicated: " + item.ItemTransform.name);
                        continue;
                    }
                    if (!_allItems.Contains(item))
                    {
                        Debug.LogError("Must Pick Item is not in all items: " + item.ItemTransform.name);
                        continue;
                    }

                    _itemsMustPick.Add(item);
                }
                else
                {
                    Debug.LogError("Must Pick Item " + _objsMustPick[i].name + " is missing ICookIngredientToPickOrDiscard component");
                }
            }

            _itemsOptionalPick = new List<ICookPickableIngredient>(_objsOptionalPick.Length);
            for (int i = 0; i < _objsOptionalPick.Length; i++)
            {
                if (_objsOptionalPick[i].TryGetComponent(out ICookPickableIngredient item))
                {
                    if (_itemsOptionalPick.Contains(item))
                    {
                        Debug.LogError("Optional Pick Item is duplicated: " + item.ItemTransform.name);
                        continue;
                    }
                    if (!_allItems.Contains(item))
                    {
                        Debug.LogError("Optional Pick Item is not in all items: " + item.ItemTransform.name);
                        continue;
                    }

                    _itemsOptionalPick.Add(item);
                }
                else
                {
                    Debug.LogError("Optional Pick Item " + _objsOptionalPick[i].name + " is missing ICookIngredientToPickOrDiscard component");
                }
            }

            _originScaleContainerItemsPicked = _containerItemsPicked.ContainerTransform.localScale;
            if (_isPlayAnimShowContainerItemsPickedOnStartModule)
            {
                _containerItemsPicked.ContainerTransform.localScale = Vector3.zero;
                _containerItemsPicked.ContainerTransform.gameObject.SetActive(false);
            }
            else
            {
                _containerItemsPicked.ContainerTransform.localScale = Vector3.one;
                _containerItemsPicked.ContainerTransform.gameObject.SetActive(true);
            }

            if (_containerItemsDiscarded != null)
            {
                _originScaleContainerItemsDiscarded = _containerItemsDiscarded.ContainerTransform.localScale;
                if (_isPlayAnimShowContainerItemsDiscardedOnStartModule)
                {
                    _containerItemsDiscarded.ContainerTransform.localScale = Vector3.zero;
                    _containerItemsDiscarded.ContainerTransform.gameObject.SetActive(false);
                }
                else
                {
                    _containerItemsDiscarded.ContainerTransform.localScale = Vector3.one;
                    _containerItemsDiscarded.ContainerTransform.gameObject.SetActive(true);
                }
            }

            if (_isPlayAnimShowItemsOnStartModule)
            {
                foreach (var item in _allItems)
                {
                    item.ItemTransform.localScale = Vector3.zero;
                    item.ItemTransform.gameObject.SetActive(false);
                }
            }
            else
            {
                foreach (var item in _allItems)
                {
                    item.ItemTransform.localScale = Vector3.one;
                    item.ItemTransform.gameObject.SetActive(true);
                }
            }
        }

        private void OnDestroy()
        {
            foreach (var item in _allItems)
            {
                item.onEndDrag -= OnEndDragItem;
            }
        }
        #endregion

        public async Task SetUp()
        {
            if (_containerItemsDiscarded != null && _containerItemsDiscarded.ContainerTransform.TryGetComponent(out INeedSetUp containerNeedSetUp))
            {
                containerNeedSetUp.SetUp();
            }

            if (_containerItemsPicked.ContainerTransform.TryGetComponent(out INeedSetUp containerNeedSetUp2))
            {
                containerNeedSetUp2.SetUp();
            }

            foreach (var item in _allItems)
            {
                if (item.ItemTransform.TryGetComponent(out INeedSetUp itemNeedSetUp))
                {
                    itemNeedSetUp.SetUp();
                }

                item.BlockDragModifiers.AddModifier(this);
            }
            await Task.CompletedTask;
        }

        public async Task StartModule(Action<ICookModule> onEnd)
        {
            _onEnd = onEnd;

            if (_isPlayAnimShowContainerItemsDiscardedOnStartModule || _isPlayAnimShowContainerItemsPickedOnStartModule || _isPlayAnimShowItemsOnStartModule)
            {
                StartCoroutine(IEShowContainerAndItemIterately());

                float estimateTime = 0f;
                if (_isPlayAnimShowContainerItemsDiscardedOnStartModule || _isPlayAnimShowContainerItemsPickedOnStartModule)
                {
                    estimateTime += _animShowItemConfig.delayShowContainer;
                    if (_isPlayAnimShowContainerItemsPickedOnStartModule) estimateTime += _animShowItemConfig.delayBetweenItems;
                    if (_isPlayAnimShowContainerItemsDiscardedOnStartModule) estimateTime += _animShowItemConfig.delayBetweenItems;
                    estimateTime += _animShowItemConfig.durationShow;
                }
                if (_isPlayAnimShowItemsOnStartModule)
                {
                    estimateTime += _animShowItemConfig.delayShowItem;
                    estimateTime += _allItems.Count * _animShowItemConfig.delayBetweenItems;
                    estimateTime += _animShowItemConfig.durationShow;
                }
                await Task.Delay(TimeSpan.FromSeconds(estimateTime));
            }
            else
            {
                await Task.CompletedTask;
            }

            IEnumerator IEShowContainerAndItemIterately()
            {
                var wait = new WaitForSeconds(_animShowItemConfig.delayBetweenItems);

                if (_isPlayAnimShowContainerItemsDiscardedOnStartModule || _isPlayAnimShowContainerItemsPickedOnStartModule)
                {
                    yield return new WaitForSeconds(_animShowItemConfig.delayShowContainer);

                    if (_isPlayAnimShowContainerItemsPickedOnStartModule)
                    {
                        StartCoroutine(IEShowContainer(_containerItemsPicked, _originScaleContainerItemsPicked));
                        yield return wait;
                    }

                    if (_containerItemsDiscarded != null && _isPlayAnimShowContainerItemsDiscardedOnStartModule)
                    {
                        StartCoroutine(IEShowContainer(_containerItemsDiscarded, _originScaleContainerItemsDiscarded));
                        yield return wait;
                    }
                }

                if (_isPlayAnimShowItemsOnStartModule)
                {
                    yield return new WaitForSeconds(_animShowItemConfig.delayShowItem);
                    int[] shuffledIndex = RandomUtility.GetShuffledIndexArray(_allItems.Count);
                    for (int i = 0; i < shuffledIndex.Length; i++)
                    {
                        StartCoroutine(IEShowSingleItem(_allItems[shuffledIndex[i]]));
                        yield return wait;
                    }
                }
                else
                {
                    foreach (var item in _allItems)
                    {
                        item.BlockDragModifiers.RemoveModifier(this);
                    }
                }
            }

            IEnumerator IEShowSingleItem(ICookPickableIngredient item)
            {
                item.ItemTransform.gameObject.SetActive(true);
                Transform itemTransform = item.ItemTransform;
                Vector3 originPos = itemTransform.position;
                Vector3 startPos = itemTransform.position + _animShowItemConfig.offsetStarPosShow;
                float timeStart = Time.time;
                while (Time.time - timeStart < _animShowItemConfig.durationShow)
                {
                    float t = (Time.time - timeStart) / _animShowItemConfig.durationShow;
                    itemTransform.position = Vector3.LerpUnclamped(startPos, originPos, _animShowItemConfig.curveFly.Evaluate(t));
                    itemTransform.localScale = Vector3.one * _animShowItemConfig.curveScale.Evaluate(t);
                    yield return null;
                }
                item.BlockDragModifiers.RemoveModifier(this);
            }

            IEnumerator IEShowContainer(ICookIngredientContainer container, Vector3 originScale)
            {
                container.ContainerTransform.gameObject.SetActive(true);
                Transform itemTransform = container.ContainerTransform;
                Vector3 originPos = itemTransform.position;
                Vector3 startPos = itemTransform.position + _animShowItemConfig.offsetStarPosShow;
                float timeStart = Time.time;
                while (Time.time - timeStart < _animShowItemConfig.durationShow)
                {
                    float t = (Time.time - timeStart) / _animShowItemConfig.durationShow;
                    itemTransform.position = Vector3.LerpUnclamped(startPos, originPos, _animShowItemConfig.curveFly.Evaluate(t));
                    itemTransform.localScale = originScale * _animShowItemConfig.curveScale.Evaluate(t);
                    yield return null;
                }
            }
        }

        private void OnEndDragItem(ICookPickableIngredient item)
        {
            if (_containerItemsPicked.IsCanAddIngredientAtPosition(item))
            {
                _containerItemsPicked.AddIngredient(item);
            }
            else if (_containerItemsDiscarded != null && _containerItemsDiscarded.IsCanAddIngredientAtPosition(item))
            {
                _containerItemsDiscarded.AddIngredient(item);
            }

            if (IsCanEndModule())
            {
                IsEnded = true;
                _containerItemsPicked.BlockThrowInOutModifiers.AddModifier(this);
                if (_isBlockDragContainerOnEndModule) _containerItemsPicked.BlockDragModifiers.AddModifier(this);

                if (_containerItemsDiscarded != null)
                {
                    _containerItemsDiscarded.BlockThrowInOutModifiers.AddModifier(this);
                    if (_isBlockDragContainerOnEndModule) _containerItemsDiscarded.BlockDragModifiers.AddModifier(this);
                }

                _onEnd?.Invoke(this);
            }
        }

        private bool IsCanEndModule()
        {
            if (_containerItemsPicked.IngredientsInside.Count < _itemsMustPick.Count) return false;

            bool isAllMustPickItemsPicked = true;
            foreach (var item in _containerItemsPicked.IngredientsInside)
            {
                if (!_itemsMustPick.Contains(item))
                {
                    isAllMustPickItemsPicked = false;
                    break;
                }
            }
            if (!isAllMustPickItemsPicked) return false;

            if (_containerItemsDiscarded != null)
            {
                if (_containerItemsDiscarded.IngredientsInside.Count + _containerItemsPicked.IngredientsInside.Count < _allItems.Count) return false;
                bool isAllItemsPickedOrDiscard = true;
                foreach (var item in _allItems)
                {
                    if (!_containerItemsPicked.IngredientsInside.Contains(item) && !_containerItemsDiscarded.IngredientsInside.Contains(item))
                    {
                        isAllItemsPickedOrDiscard = false;
                        break;
                    }
                }
                if (!isAllItemsPickedOrDiscard) return false;
            }

            return true;
        }
    }
}