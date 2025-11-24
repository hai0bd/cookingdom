using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities;

namespace TidyCooking.Levels
{
    public class CookItemBucket : CookItemAbstract, ICookIngredientContainer
    {
        private const float ItemLocalPosZ = -0.001f;

        [System.Serializable]
        public class ItemFlyInsideAnimConfig
        {
            public float durationFlyUp = 0.5f;
            public float offsetFlyPosY = 1f;
            public float minLocalFlyPosY = 0f;
            public float durationFlyDown = 0.5f;

            public AnimationCurve curveFlyUp = AnimationCurve.EaseInOut(0, 0, 1, 1);
            public AnimationCurve curveFlyDown = AnimationCurve.EaseInOut(0, 0, 1, 1);

            [Header("Shake")]
            public float durationShake = 0.5f;
            public Vector2 shakeStrength = Vector2.one;
            public AnimationCurve curveShakeStrengthOverTime = AnimationCurve.EaseInOut(0, 1, 1, 0);
            public float lerpShakePos = 0.5f;
        }

        [Space, Header("Bucket Config")]
        [SerializeField] private ObjectScatterGroup _itemContainer;
        [SerializeField] private Collider2D[] _colliderAreaCanThrowItemInside;
        [SerializeField] private bool _isBlockDragItemInside = true;
        [SerializeField] private float _itemScaleMultiplier = 1f;

        public Transform ContainerTransform => this.transform;
        private List<ICookPickableIngredient> _ingredientsInside = new();
        public List<ICookPickableIngredient> IngredientsInside => _ingredientsInside;

        [Header("Item Fly Inside Anim")]
        [SerializeField] private bool _isPlayFlyAnimOnAddItem = true;
        [EnableIf("_isPlayFlyAnimOnAddItem")]
        [SerializeField] private ItemFlyInsideAnimConfig _itemFlyInsideAnimConfig;

        [Header("Shake Fx")]
        [SerializeField] private Transform _shakeAnchor;
        [EnableIf("_shakeAnchor")]
        [SerializeField] private bool _isPlayShakeAnimOnAddItem = true;
        [EnableIf("_shakeAnchor")]
        [SerializeField] private bool _isPlayShakeAnimOnRemoveItem = true;
        private float _lastTimeAddOrRemoveItem = -999f;
        private Vector2 _shakeTimeOffset;

        [Space, Header("Pop Item")]
        [SerializeField] private ForwardMouseEvent _zoneTapToPopItem;
        [EnableIf("_zoneTapToPopItem")]
        [SerializeField] private ItemPopAnimConfig _popItemAnimConfig;
        private bool IsPopItemOnTap => _zoneTapToPopItem;

        public BoolModifierWithRegisteredSource BlockThrowInOutModifiers { get; private set; } = new BoolModifierWithRegisteredSource();

        [System.Serializable]
        public class ItemPopAnimConfig
        {
            public float clickDurationThreshold = 0.2f;
            public float clickDistanceThreshold = 0.5f;
            public Transform popItemAnchor;
            public Vector2 popItemArea = Vector2.zero;
            public float popItemRadius = 1f;
            public AnimationCurve curvePopFlyPos = AnimationCurve.EaseInOut(0, 0, 1, 1);
            public float flyDistanceToHeightAddY = 0.1f;
            public float minHeightAddY = 0.5f;
            public AnimationCurve curvePopFlyAddY = AnimationCurve.Constant(0, 1, 0);
            public float durationPopFly = 0.5f;
            public bool isClampPopEndPos = true;
            public bool isClampRelativeToScreen = true;
            public Rect areaClampPosPop = new Rect(0.1f, 0.05f, 0.8f, 0.9f);
        }
        private float _timeStartTap;
        private Vector2 _posStartTap;

        protected override void Awake()
        {
            base.Awake();

            if (_zoneTapToPopItem)
            {
                _zoneTapToPopItem.onMouseDown += OnMouseDownPop;
                _zoneTapToPopItem.onMouseUp += OnMouseUpPop;
            }

            for (int i = _itemContainer.Container.childCount - 1; i >= 0; i--)
            {
                if (_itemContainer.Container.GetChild(i).TryGetComponent(out ICookPickableIngredient ingredient))
                {
                    _ingredientsInside.Add(ingredient);
                }
            }
        }

        protected override void Start()
        {
            base.Start();
            if (_isBlockDragItemInside)
            {
                foreach (var ingredient in _ingredientsInside)
                {
                    ingredient.BlockDragModifiers.AddModifier(this);
                }
            }
        }

        public bool IsCanAddIngredientAtPosition(ICookPickableIngredient ingredient)
        {
            if (BlockThrowInOutModifiers.Value) return false;

            foreach (var collider in _colliderAreaCanThrowItemInside)
            {
                if (collider.OverlapPoint(ingredient.ItemTransform.position))
                {
                    return true;
                }
            }
            return false;
        }

        public bool AddIngredient(ICookPickableIngredient ingredient)
        {
            if (IsCanAddIngredientAtPosition(ingredient) && !_ingredientsInside.Contains(ingredient))
            {
                _ingredientsInside.Add(ingredient);
                ingredient.onStartDrag -= OnItemInsideStartDrag;
                ingredient.onStartDrag += OnItemInsideStartDrag;

                int indexPos = _ingredientsInside.Count - 1;
                if (_isPlayFlyAnimOnAddItem)
                {
                    ingredient.SetTransformation(IEThrowItemInside(ingredient, indexPos));
                }
                else
                {
                    ingredient.ItemTransform.SetParent(_itemContainer.Container);
                    ingredient.ItemTransform.SetLocalPositionAndRotation(
                        _itemContainer.GetRandomPosition(indexPos),
                        Quaternion.Euler(0f, 0f, _itemContainer.GetRandomAngle(indexPos)));
                    ingredient.ItemTransform.localScale = _itemContainer.GetRandomScale(indexPos) * _itemScaleMultiplier * Vector3.one;
                    if (_isBlockDragItemInside) ingredient.BlockDragModifiers.AddModifier(this);
                }

                return true;
            }
            return false;
        }

        private void OnItemInsideStartDrag(ICookPickableIngredient ingredient)
        {
            if (_ingredientsInside.Remove(ingredient))
            {
                if (_isPlayShakeAnimOnRemoveItem)
                {
                    _lastTimeAddOrRemoveItem = Time.time;
                    _shakeTimeOffset = Random.insideUnitCircle * _itemFlyInsideAnimConfig.durationShake;
                }
                ingredient.ItemTransform.SetParent(null);
                ingredient.ItemTransform.localRotation = Quaternion.identity;
                ingredient.ItemTransform.localScale = Vector3.one;
            }
        }

        private IEnumerator IEThrowItemInside(ICookPickableIngredient ingredient, int indexPos)
        {
            Transform itemTransform = ingredient.ItemTransform;
            itemTransform.SetParent(_itemContainer.Container);
            ingredient.BlockDragModifiers.AddModifier(this);

            Vector3 endPos = _itemContainer.GetRandomPosition(indexPos);
            Quaternion endRot = Quaternion.Euler(0f, 0f, _itemContainer.GetRandomAngle(indexPos));
            float endScale = _itemContainer.GetRandomScale(indexPos) * _itemScaleMultiplier;

            Vector3 localStartPos = itemTransform.localPosition;
            Vector3 localDestination = localStartPos;
            Quaternion localStartRot = itemTransform.localRotation;
            Quaternion localDestinationRot = Quaternion.Lerp(endRot, localStartRot, 0.5f);
            Vector3 startScale = itemTransform.localScale;
            Vector3 destinationScale = startScale * (1f + endScale) / 2f;
            Quaternion startRot = itemTransform.localRotation;
            localDestination.x = (localStartPos.x + endPos.x) / 2f;
            localDestination.y = Mathf.Max(localStartPos.y + _itemFlyInsideAnimConfig.offsetFlyPosY, endPos.y + _itemFlyInsideAnimConfig.offsetFlyPosY, _itemFlyInsideAnimConfig.minLocalFlyPosY);

            float timeStart = Time.time;
            float t;
            while (Time.time - timeStart < _itemFlyInsideAnimConfig.durationFlyUp)
            {
                t = (Time.time - timeStart) / _itemFlyInsideAnimConfig.durationFlyUp;
                itemTransform.SetLocalPositionAndRotation(
                    Vector3.LerpUnclamped(localStartPos, localDestination, _itemFlyInsideAnimConfig.curveFlyUp.Evaluate(t)),
                    Quaternion.SlerpUnclamped(localStartRot, localDestinationRot, _itemFlyInsideAnimConfig.curveFlyUp.Evaluate(t))
                    );
                itemTransform.localScale = Vector3.LerpUnclamped(startScale, destinationScale, _itemFlyInsideAnimConfig.curveFlyUp.Evaluate(t));
                yield return null;
            }

            yield return null;

            localStartPos = itemTransform.localPosition;
            localDestination = endPos;
            startScale = itemTransform.localScale;
            destinationScale = Vector3.one * endScale;

            timeStart = Time.time;
            while (Time.time - timeStart < _itemFlyInsideAnimConfig.durationFlyDown)
            {
                t = (Time.time - timeStart) / _itemFlyInsideAnimConfig.durationFlyDown;
                itemTransform.SetLocalPositionAndRotation(
                    Vector3.LerpUnclamped(localStartPos, localDestination, _itemFlyInsideAnimConfig.curveFlyDown.Evaluate(t)),
                    Quaternion.SlerpUnclamped(localDestinationRot, endRot, _itemFlyInsideAnimConfig.curveFlyDown.Evaluate(t))
                    );
                itemTransform.localScale = Vector3.LerpUnclamped(startScale, destinationScale, _itemFlyInsideAnimConfig.curveFlyDown.Evaluate(t));
                yield return null;
            }
            itemTransform.localPosition = localDestination;

            if (!_isBlockDragItemInside)
            {
                ingredient.BlockDragModifiers.RemoveModifier(this);
            }

            if (_isPlayShakeAnimOnAddItem)
            {
                _lastTimeAddOrRemoveItem = Time.time;
                _shakeTimeOffset = Random.insideUnitCircle * _itemFlyInsideAnimConfig.durationShake;
            }
        }

        public void RemoveIngredient(ICookPickableIngredient ingredient)
        {
            if (BlockThrowInOutModifiers.Value) return;

            if (_ingredientsInside.Remove(ingredient))
            {
                ingredient.BlockDragModifiers.RemoveModifier(this);
                if (_isPlayShakeAnimOnRemoveItem)
                {
                    _lastTimeAddOrRemoveItem = Time.time;
                    _shakeTimeOffset = Random.insideUnitCircle * _itemFlyInsideAnimConfig.durationShake;
                }

                // random pos
                Vector3 pos = _popItemAnimConfig.popItemAnchor ? _popItemAnimConfig.popItemAnchor.position : this.transform.position;
                pos += new Vector3(
                        UnityEngine.Random.Range(-_popItemAnimConfig.popItemArea.x / 2f, _popItemAnimConfig.popItemArea.x / 2f),
                        UnityEngine.Random.Range(-_popItemAnimConfig.popItemArea.y / 2f, _popItemAnimConfig.popItemArea.y / 2f),
                        0f)
                    + (Vector3)Random.insideUnitCircle * _popItemAnimConfig.popItemRadius;

                if (_popItemAnimConfig.isClampPopEndPos)
                {
                    if (_popItemAnimConfig.isClampRelativeToScreen)
                    {
                        Vector2 min = Camera.main.ViewportToWorldPoint(new Vector3(_popItemAnimConfig.areaClampPosPop.xMin, _popItemAnimConfig.areaClampPosPop.yMin, 0));
                        Vector2 max = Camera.main.ViewportToWorldPoint(new Vector3(_popItemAnimConfig.areaClampPosPop.xMax, _popItemAnimConfig.areaClampPosPop.yMax, 0));
                        pos.x = Mathf.Clamp(pos.x, min.x, max.x);
                        pos.y = Mathf.Clamp(pos.y, min.y, max.y);
                    }
                    else
                    {
                        pos.x = Mathf.Clamp(pos.x, _popItemAnimConfig.areaClampPosPop.xMin, _popItemAnimConfig.areaClampPosPop.xMax);
                        pos.y = Mathf.Clamp(pos.y, _popItemAnimConfig.areaClampPosPop.yMin, _popItemAnimConfig.areaClampPosPop.yMax);
                    }
                }
                pos.z = ItemLocalPosZ;

                ingredient.SetTransformation(IEPopIngredient(ingredient, pos));
            }
        }

        private IEnumerator IEPopIngredient(ICookPickableIngredient ingredient, Vector3 position)
        {
            Transform itemTransform = ingredient.ItemTransform;
            itemTransform.SetParent(null);

            Vector3 startScale = itemTransform.localScale;
            Quaternion startRot = itemTransform.rotation;
            Vector3 startPos = itemTransform.position;
            Vector3 destination = position;
            destination.z = startPos.z;

            float distance = Vector3.Distance(startPos, destination);
            float heightAddY = Mathf.Max(_popItemAnimConfig.minHeightAddY, distance * _popItemAnimConfig.flyDistanceToHeightAddY);

            float timeStart = Time.time;
            float t;
            while (Time.time - timeStart < _popItemAnimConfig.durationPopFly)
            {
                t = (Time.time - timeStart) / _popItemAnimConfig.durationPopFly;
                itemTransform.SetLocalPositionAndRotation(
                    Vector3.LerpUnclamped(startPos, destination, _popItemAnimConfig.curvePopFlyPos.Evaluate(t)) + Vector3.up * _popItemAnimConfig.curvePopFlyAddY.Evaluate(t) * heightAddY,
                    Quaternion.SlerpUnclamped(startRot, Quaternion.identity, _popItemAnimConfig.curvePopFlyPos.Evaluate(t))
                    );
                itemTransform.localScale = Vector3.LerpUnclamped(startScale, Vector3.one, _popItemAnimConfig.curvePopFlyPos.Evaluate(t));
                yield return null;
            }

            itemTransform.position = destination;
        }

        private void Update()
        {
            if (_shakeAnchor && Time.time - _lastTimeAddOrRemoveItem < _itemFlyInsideAnimConfig.durationShake + 0.5f)
            {
                float t = (Time.time - _lastTimeAddOrRemoveItem) / _itemFlyInsideAnimConfig.durationShake;
                Vector3 deltaShake = new Vector3(
                    Mathf.Sin(Time.time + _shakeTimeOffset.x) * _itemFlyInsideAnimConfig.shakeStrength.x,
                    Mathf.Sin(Time.time + _shakeTimeOffset.y) * _itemFlyInsideAnimConfig.shakeStrength.y,
                    0f) * _itemFlyInsideAnimConfig.curveShakeStrengthOverTime.Evaluate(t);

                Vector3 shakePos = _shakeAnchor.localPosition;
                shakePos.LerpTo(deltaShake, _itemFlyInsideAnimConfig.lerpShakePos);
                _shakeAnchor.localPosition = shakePos;
            }
        }

        private void OnMouseDownPop(ForwardMouseEvent @event)
        {
            // if (!LevelBase.instance .IsAllowInteract) return;
            _timeStartTap = Time.unscaledTime;
            _posStartTap = @event.transform.position;
        }
        private void OnMouseUpPop(ForwardMouseEvent @event)
        {
            // if (!LevelBase.instance.IsAllowInteract) return;
            if (IsPopItemOnTap && Time.unscaledTime < _timeStartTap + _popItemAnimConfig.clickDurationThreshold && Vector2.Distance(@event.transform.position, _posStartTap) < _popItemAnimConfig.clickDistanceThreshold)
            {
                if (_ingredientsInside.Count > 0 && !BlockThrowInOutModifiers.Value)
                {
                    var lastIngredient = _ingredientsInside[_ingredientsInside.Count - 1];
                    RemoveIngredient(lastIngredient);
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (_itemContainer && _itemContainer.Container)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(
                    new Vector3(_itemContainer.Container.position.x - 3f, _itemContainer.Container.position.y + _itemFlyInsideAnimConfig.minLocalFlyPosY, _itemContainer.Container.position.z),
                    new Vector3(_itemContainer.Container.position.x + 3f, _itemContainer.Container.position.y + _itemFlyInsideAnimConfig.minLocalFlyPosY, _itemContainer.Container.position.z)
                    );
            }

            if (IsPopItemOnTap)
            {
                Gizmos.color = Color.magenta;
                Vector3 pos = _popItemAnimConfig.popItemAnchor ? _popItemAnimConfig.popItemAnchor.position : this.transform.position;

                Matrix4x4 backupGizmo = Gizmos.matrix;
                Gizmos.matrix = Matrix4x4.TRS(pos, Quaternion.identity, Vector3.one);

                float _popItemRadius = _popItemAnimConfig.popItemRadius;
                Vector2 _popItemArea = _popItemAnimConfig.popItemArea;

                if (_popItemRadius < 0.01f)
                {
                    Gizmos.DrawWireCube(Vector3.zero, new Vector3(_popItemArea.x, _popItemArea.y, 0.1f));
                }
                else if (_popItemArea.x + _popItemArea.y < 0.01f)
                {
                    Gizmos.DrawWireSphere(Vector3.zero, _popItemRadius);
                }
                else
                {
                    Vector3 cornerLT = new Vector3(-_popItemArea.x / 2f, _popItemArea.y / 2f);
                    Vector3 cornerRT = new Vector3(_popItemArea.x / 2f, _popItemArea.y / 2f);
                    Vector3 cornerLB = new Vector3(-_popItemArea.x / 2f, -_popItemArea.y / 2f);
                    Vector3 cornerRB = new Vector3(_popItemArea.x / 2f, -_popItemArea.y / 2f);

                    Gizmos.DrawWireSphere(cornerLT, _popItemRadius);
                    Gizmos.DrawWireSphere(cornerRT, _popItemRadius);
                    Gizmos.DrawWireSphere(cornerLB, _popItemRadius);
                    Gizmos.DrawWireSphere(cornerRB, _popItemRadius);

                    Gizmos.DrawLine(cornerLT + Vector3.up * _popItemRadius, cornerRT + Vector3.up * _popItemRadius);
                    Gizmos.DrawLine(cornerRT + Vector3.right * _popItemRadius, cornerRB + Vector3.right * _popItemRadius);
                    Gizmos.DrawLine(cornerRB + Vector3.down * _popItemRadius, cornerLB + Vector3.down * _popItemRadius);
                    Gizmos.DrawLine(cornerLB + Vector3.left * _popItemRadius, cornerLT + Vector3.left * _popItemRadius);
                }

                Gizmos.matrix = backupGizmo;

                if (_popItemAnimConfig.isClampPopEndPos)
                {
                    if (_popItemAnimConfig.isClampRelativeToScreen)
                    {
                        Vector2 min = Camera.main.ViewportToWorldPoint(new Vector3(_popItemAnimConfig.areaClampPosPop.xMin, _popItemAnimConfig.areaClampPosPop.yMin, 0));
                        Vector2 max = Camera.main.ViewportToWorldPoint(new Vector3(_popItemAnimConfig.areaClampPosPop.xMax, _popItemAnimConfig.areaClampPosPop.yMax, 0));
                        GizmoUtility.DrawRect(new Rect(min, max - min), Color.red);
                    }
                    else
                    {
                        GizmoUtility.DrawRect(_popItemAnimConfig.areaClampPosPop, Color.red);
                    }
                }
            }
        }
    }
}