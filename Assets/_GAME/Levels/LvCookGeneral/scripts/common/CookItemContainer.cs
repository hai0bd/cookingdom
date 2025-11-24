using System;
using System.Collections;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace TidyCooking.Levels
{
    public class CookItemContainer : CookItemAbstract
    {
        private const float ItemInitLocalPosZ = -0.001f;
        private const float IncItemLocalPosZ = -0.0001f;
        private const int ItemInitSortOrder = 1;

        public Transform ContainerTransform => this.transform;

        [System.Serializable]
        public class ItemSnapInsideAnimConfig
        {
            public float snapSpeed = 3f;
            public Vector2 clampDurationSnap = new Vector2(0.1f, 0.35f);
            public AnimationCurve curveSnap = AnimationCurve.EaseInOut(0, 0, 1, 1);
        }

        [System.Serializable]
        public class ShakeConfig
        {
            [Header("Shake")]
            public float durationShake = 0.25f;
            public Vector2 shakeAmplitude = new Vector2(0.02f, 0.03f);
            public AnimationCurve curveShakeStrengthOverTime = AnimationCurve.Linear(0, 1, 1, 0);
            public float lerpShakePosition = 0.7f;
        }

        [System.Serializable]
        public class SnapByTag
        {
            public string tag;
            public Transform matchingTransform;
            public Vector3 offset;
        }

        [System.Serializable]
        public class SpecificItemSnap
        {
            public CookItemAbstract item;
            public Transform matchingTransform;
            public Vector3 offset;
        }

        [Space, Header("Layout")]
        [SerializeField] private Transform _itemContainer;
        [SerializeField] private Rect _localAreaClamp = new Rect(-1, -1, 2, 2);
        [SerializeField] private bool _isClampWithItemGroundBounding = false;
        [EnableIf("_isClampWithItemGroundBounding")]
        [SerializeField] private Rect _localAreaBounding = new Rect(-1, -1, 2, 2);

        [Space, Header("Item Config")]
        [SerializeField] private Collider2D[] _colliderAreaCanThrowItemInside;
        [SerializeField] private Rect[] _areasCanThrowItemInside;
        [SerializeField] private float _itemScale = 1f;
        [SerializeField] private Vector2 _itemAngleRange = Vector2.zero;
        [SerializeField] private bool _isRestrictItemCanDragInside = true;
        [SerializeField] private List<CookItemAbstract> _itemAllowDragInside;
        [SerializeField] private List<string> _itemTagsAllowDragInside;
        [SerializeField] private List<SpecificItemSnap> _specificItemSnaps;
        [SerializeField] private List<SnapByTag> _snapByTags;

        private List<CookItemAbstract> _itemInside = new();
        public List<CookItemAbstract> ItemsInside => _itemInside;

        [Header("Item Fly Inside Anim")]
        [SerializeField] private bool _isPlayAnimSnapItem = true;
        [EnableIf("_isPlayAnimSnapItem")]
        [SerializeField] private ItemSnapInsideAnimConfig _itemFlyInsideAnimConfig;


        [Header("Shake Fx")]
        [SerializeField] private Transform _shakeAnchor;
        [EnableIf("_shakeAnchor")]
        [SerializeField] private bool _isPlayShakeAnimOnAddItem = true;
        [EnableIf("_shakeAnchor")]
        [SerializeField] private bool _isPlayShakeAnimOnRemoveItem = true;
        [EnableIf("_shakeAnchor")]
        [SerializeField] private ShakeConfig _shakeConfig;
        private float _lastTimeAddOrRemoveItem = -999f;
        private Vector2 _shakeTimeOffset;

        public BoolModifierWithRegisteredSource BlockThrowInOutModifiers { get; private set; } = new BoolModifierWithRegisteredSource();
        private int _lastItemSortOrder;
        private float _lastItemLocalPosZ;

        protected override void Awake()
        {
            base.Awake();

            List<CookItemAbstract> itemsInsideFromStart = new List<CookItemAbstract>();
            for (int i = _itemContainer.childCount - 1; i >= 0; i--)
            {
                if (_itemContainer.GetChild(i).TryGetComponent(out CookItemAbstract item))
                {
                    itemsInsideFromStart.Add(item);
                }
            }
            foreach (var item in itemsInsideFromStart)
            {
                AddItem(item, false);
            }

            _lastItemSortOrder = ItemInitSortOrder;
            _lastItemLocalPosZ = ItemInitLocalPosZ;
        }

        #region Container
        public bool IsContainsItem(CookItemAbstract item)
        {
            return _itemInside.Contains(item);
        }

        public bool IsAllowAddItem(CookItemAbstract ingredient)
        {
            if (BlockThrowInOutModifiers.Value) return false;

            if (_isRestrictItemCanDragInside)
            {
                bool isItemAllow = false;

                if (_itemAllowDragInside != null && _itemAllowDragInside.Contains(ingredient))
                {
                    isItemAllow = true;
                }

                if (_itemTagsAllowDragInside != null)
                {
                    foreach (var tag in _itemTagsAllowDragInside)
                    {
                        if (ingredient.IsHaveTag(tag))
                        {
                            isItemAllow = true;
                            break;
                        }
                    }
                }

                if (!isItemAllow)
                {
                    return false;
                }
            }

            return true;
        }

        public bool IsCanAddItemAtPosition(CookItemAbstract ingredient)
        {
            if (BlockThrowInOutModifiers.Value) return false;

            if (_isRestrictItemCanDragInside)
            {
                bool isItemAllow = false;

                if (_itemAllowDragInside != null && _itemAllowDragInside.Contains(ingredient))
                {
                    isItemAllow = true;
                }

                if (_itemTagsAllowDragInside != null)
                {
                    foreach (var tag in _itemTagsAllowDragInside)
                    {
                        if (ingredient.IsHaveTag(tag))
                        {
                            isItemAllow = true;
                            break;
                        }
                    }
                }

                if (!isItemAllow)
                {
                    return false;
                }
            }

            if (_colliderAreaCanThrowItemInside != null)
            {
                foreach (var collider in _colliderAreaCanThrowItemInside)
                {
                    if (collider.OverlapPoint(ingredient.transform.position))
                    {
                        return true;
                    }
                }
            }

            if (_areasCanThrowItemInside != null)
            {
                foreach (var area in _areasCanThrowItemInside)
                {
                    if (area.Contains(ingredient.transform.position - transform.position))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private SpecificItemSnap GetItemSpecificSnap(CookItemAbstract item)
        {
            if (_specificItemSnaps == null) return null;
            foreach (var specificItemSnap in _specificItemSnaps)
            {
                if (specificItemSnap.item == item)
                {
                    return specificItemSnap;
                }
            }
            return null;
        }

        private SnapByTag GetItemSnapByTag(CookItemAbstract item)
        {
            foreach (var snapByTag in _snapByTags)
            {
                if (item.IsHaveTag(snapByTag.tag))
                {
                    return snapByTag;
                }
            }
            return null;
        }

        public bool AddItem(CookItemAbstract item, bool isPlayAnimSnapItem = true)
        {
            if (IsAllowAddItem(item) && !_itemInside.Contains(item))
            {
                _itemInside.Add(item);
                item.onStartDragItem -= OnItemInsideStartDrag;
                item.onStartDragItem += OnItemInsideStartDrag;

                // clamp
                item.transform.SetParent(_itemContainer.transform);

                _lastItemLocalPosZ += IncItemLocalPosZ;
                _lastItemSortOrder += 1;
                item.LayerBehaviour?.SetSortOrder(_lastItemSortOrder);

                Vector3 destinationPos;
                Quaternion destinationRot;
                Vector3 destinationScale;

                SpecificItemSnap specificItemSnap = GetItemSpecificSnap(item);
                if (specificItemSnap != null)
                {
                    if (specificItemSnap.matchingTransform == null)
                    {
                        destinationPos = specificItemSnap.offset;
                        destinationRot = Quaternion.Euler(0, 0, _itemAngleRange.GetRandomWithin());
                        destinationScale = _itemScale * Vector3.one;
                    }
                    else
                    {
                        destinationPos = specificItemSnap.matchingTransform.localPosition + specificItemSnap.offset;
                        destinationRot = specificItemSnap.matchingTransform.localRotation;
                        destinationScale = specificItemSnap.matchingTransform.localScale;
                    }
                }
                else
                {
                    SnapByTag snapByTag = GetItemSnapByTag(item);
                    if (snapByTag != null)
                    {
                        if (snapByTag.matchingTransform == null)
                        {
                            destinationPos = snapByTag.offset;
                            destinationRot = Quaternion.Euler(0, 0, _itemAngleRange.GetRandomWithin());
                            destinationScale = _itemScale * Vector3.one;
                        }
                        else
                        {
                            destinationPos = snapByTag.matchingTransform.localPosition + snapByTag.offset;
                            destinationRot = snapByTag.matchingTransform.localRotation;
                            destinationScale = snapByTag.matchingTransform.localScale;
                        }
                    }
                    else
                    {
                        Vector3 localPos = item.transform.localPosition;
                        destinationPos = _localAreaClamp.Clamp(localPos);
                        destinationRot = Quaternion.Euler(0, 0, _itemAngleRange.GetRandomWithin());
                        destinationScale = _itemScale * Vector3.one;

                        if (_isClampWithItemGroundBounding)
                        {
                            Rect rectAfterRotate = item.GroundBoundingBox.CalculateNewBoundingRectByRotation(destinationRot.eulerAngles.z);

                            float minX = _localAreaBounding.xMin - rectAfterRotate.xMin;
                            float maxX = _localAreaBounding.xMax - rectAfterRotate.xMax;
                            float minY = _localAreaBounding.yMin - rectAfterRotate.yMin;
                            float maxY = _localAreaBounding.yMax - rectAfterRotate.yMax;
                            if (minX > maxX) minX = maxX = (minX + maxX) / 2f;
                            if (minY > maxY) minY = maxY = (minY + maxY) / 2f;
                            destinationPos.x = Mathf.Clamp(destinationPos.x, minX, maxX);
                            destinationPos.y = Mathf.Clamp(destinationPos.y, minY, maxY);
                        }
                    }
                }
                destinationPos.z = _lastItemLocalPosZ;

                item.SetContainer(this);

                if (isPlayAnimSnapItem)
                {
                    item.SetTransformation(IESnapItemInside(item, destinationPos, destinationRot, destinationScale));
                }
                else
                {
                    item.transform.localPosition = destinationPos;
                    item.transform.localRotation = destinationRot;
                    item.transform.localScale = destinationScale;
                }

                return true;
            }
            return false;
        }

        private void OnItemInsideStartDrag(CookItemAbstract item)
        {
            if (_itemInside.Remove(item))
            {
                if (_isPlayShakeAnimOnRemoveItem)
                {
                    _lastTimeAddOrRemoveItem = Time.time;
                    _shakeTimeOffset = UnityEngine.Random.insideUnitCircle * _shakeConfig.durationShake;
                }
                item.transform.SetParent(null);
                //item.transform.localRotation = Quaternion.identity;
                //item.transform.localScale = Vector3.one;
                item.SetRotatation(IEResetItemRotation(item));
                item.SetScaling(IEResetItemScale(item));
                item.onStartDragItem -= OnItemInsideStartDrag;
            }

            IEnumerator IEResetItemScale(CookItemAbstract item)
            {
                Transform itemTransform = item.transform;
                Vector3 startLocalScale = itemTransform.localScale;
                float durationSnap = 1f / _itemFlyInsideAnimConfig.snapSpeed;
                float timeStart = Time.time;
                float t;
                while (Time.time - timeStart < durationSnap)
                {
                    t = (Time.time - timeStart) / durationSnap;
                    itemTransform.localScale = Vector3.LerpUnclamped(startLocalScale, Vector3.one, _itemFlyInsideAnimConfig.curveSnap.Evaluate(t));
                    yield return null;
                }
                itemTransform.localScale = Vector3.one;
            }

            IEnumerator IEResetItemRotation(CookItemAbstract item)
            {
                Transform itemTransform = item.transform;
                Quaternion startLocalRot = itemTransform.localRotation;
                float durationSnap = 1f / _itemFlyInsideAnimConfig.snapSpeed;
                float timeStart = Time.time;
                float t;
                while (Time.time - timeStart < durationSnap)
                {
                    t = (Time.time - timeStart) / durationSnap;
                    itemTransform.localRotation = Quaternion.SlerpUnclamped(startLocalRot, Quaternion.identity, _itemFlyInsideAnimConfig.curveSnap.Evaluate(t));
                    yield return null;
                }
                itemTransform.localRotation = Quaternion.identity;
            }
        }

        private IEnumerator IESnapItemInside(CookItemAbstract item, Vector3 endLocalPos, Quaternion endLocalRot, Vector3 endLocalScale)
        {
            Transform itemTransform = item.transform;
            itemTransform.SetParent(_itemContainer);
            item.BlockDragModifiers.AddModifier(this);

            Vector3 startLocalPos = itemTransform.localPosition;
            Vector3 startLocalScale = itemTransform.localScale;
            Quaternion startLocalRot = itemTransform.localRotation;

            float durationSnap = Vector2.Distance(startLocalPos, endLocalPos) / _itemFlyInsideAnimConfig.snapSpeed;
            durationSnap = Mathf.Clamp(durationSnap, _itemFlyInsideAnimConfig.clampDurationSnap.x, _itemFlyInsideAnimConfig.clampDurationSnap.y);

            float timeStart = Time.time;
            float t;
            while (Time.time - timeStart < durationSnap)
            {
                t = (Time.time - timeStart) / durationSnap;
                itemTransform.SetLocalPositionAndRotation(
                    Vector3.LerpUnclamped(startLocalPos, endLocalPos, _itemFlyInsideAnimConfig.curveSnap.Evaluate(t)),
                    Quaternion.SlerpUnclamped(startLocalRot, endLocalRot, _itemFlyInsideAnimConfig.curveSnap.Evaluate(t)));
                itemTransform.localScale = Vector3.LerpUnclamped(startLocalScale, endLocalScale, _itemFlyInsideAnimConfig.curveSnap.Evaluate(t));
                yield return null;
            }

            itemTransform.SetLocalPositionAndRotation(endLocalPos, endLocalRot);
            itemTransform.localScale = endLocalScale;

            item.BlockDragModifiers.RemoveModifier(this);

            if (_isPlayShakeAnimOnAddItem)
            {
                _lastTimeAddOrRemoveItem = Time.time;
                _shakeTimeOffset = UnityEngine.Random.insideUnitCircle * _shakeConfig.durationShake;
            }
        }
        #endregion

        private void Update()
        {
            if (_shakeAnchor && Time.time - _lastTimeAddOrRemoveItem < _shakeConfig.durationShake + 0.5f)
            {
                float t = (Time.time - _lastTimeAddOrRemoveItem) / _shakeConfig.durationShake;
                Vector3 deltaShake = new Vector3(
                    Mathf.Sin(Time.time + _shakeTimeOffset.x) * _shakeConfig.shakeAmplitude.x,
                    Mathf.Sin(Time.time + _shakeTimeOffset.y) * _shakeConfig.shakeAmplitude.y,
                    0f) * _shakeConfig.curveShakeStrengthOverTime.Evaluate(t);

                Vector3 shakePos = _shakeAnchor.localPosition;
                shakePos.LerpTo(deltaShake, _shakeConfig.lerpShakePosition);
                _shakeAnchor.localPosition = shakePos;
            }
        }

        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();

            Matrix4x4 backupMatrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(_itemContainer.position, Quaternion.identity, _itemContainer.lossyScale);
            GizmoUtility.DrawRect(_localAreaClamp, Color.blue);
            Gizmos.matrix = backupMatrix;

            if (_isClampWithItemGroundBounding)
            {
                Gizmos.matrix = Matrix4x4.TRS(_itemContainer.position, Quaternion.identity, _itemContainer.lossyScale);
                GizmoUtility.DrawRect(_localAreaBounding, Color.magenta);
                Gizmos.matrix = backupMatrix;
            }

            if (_areasCanThrowItemInside != null)
            {
                foreach (var area in _areasCanThrowItemInside)
                {
                    Gizmos.color = Color.cyan;
                    Gizmos.DrawWireCube(transform.position + (Vector3)area.center, new Vector3(area.width, area.height, 0.1f));
                }
            }
        }
    }
}