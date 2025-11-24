using System;
using System.Collections;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;
using Utilities;

namespace TidyCooking.Levels
{
    public class CookItemAbstract : MonoBehaviour, INeedSetUp
    {
        [Header("Base Item Config")]
        [SerializeField] private bool _isAutoSetUpOnStart = false;
        [SerializeField] protected DraggableObject _draggable;
        [SerializeField] private DraggableObjectLayerBehaviour _layerBehaviour;

        [EnableIf("_draggable")]
        [SerializeField] protected Collider2D[] _colliders;
        private CookItemContainer _container;
        public CookItemContainer Container => _container;
        public void SetContainer(CookItemContainer container)
        {
            _container = container;
        }

        [SerializeField] protected Rect _groundBoundingBox = new Rect(-1, -1, 2, 2);
        public Rect GroundBoundingBox => _groundBoundingBox;
        private bool _isDoneSetup = false;

        [SerializeField] private string[] tags;
        public bool IsHaveTag(string tag)
        {
            if (string.IsNullOrEmpty(tag)) return false;
            if (tags == null || tags.Length == 0) return false;
            foreach (var t in tags)
            {
                if (string.Equals(t, tag)) return true;
            }
            return false;
        }

        public DraggableObjectLayerBehaviour LayerBehaviour => _layerBehaviour;
        public static int Compare(CookItemAbstract a, CookItemAbstract b)
        {
            // if have layer behaviour, compare by sorting order, then z position. If not, compare by z position only
            int layerA = a.LayerBehaviour != null ? a.LayerBehaviour.CurrentSortOrder : 0;
            int layerB = b.LayerBehaviour != null ? b.LayerBehaviour.CurrentSortOrder : 0;
            float zA = a.LayerBehaviour != null ? a.LayerBehaviour.CurrentZPosition : a.transform.position.z;
            float zB = b.LayerBehaviour != null ? b.LayerBehaviour.CurrentZPosition : b.transform.position.z;

            if (layerA == layerB)
            {
                return -zA.CompareTo(zB);
            }
            else
            {
                return layerA.CompareTo(layerB);
            }
        }
        public static int CompareZPos(CookItemAbstract a, CookItemAbstract b)
        {
            float zA = a.LayerBehaviour != null ? a.LayerBehaviour.CurrentZPosition : a.transform.position.z;
            float zB = b.LayerBehaviour != null ? b.LayerBehaviour.CurrentZPosition : b.transform.position.z;
            return -zA.CompareTo(zB);
        }

        public BoolModifierWithRegisteredSource BlockDragModifiers { get; private set; }

        public event System.Action<CookItemAbstract> onStartDragItem;
        public event System.Action<CookItemAbstract> onEndDragItem;

        #region Shared Transformation
        private Coroutine _corTransformation;
        private Coroutine _corMove;
        private Coroutine _corRotate;
        private Coroutine _corScale;
        private Tween _tweenTransformation;
        private Tween _tweenMove;
        private Tween _tweenRotate;
        private Tween _tweenScale;
        private void StopAllTransformation()
        {
            if (_corTransformation != null)
            {
                StopCoroutine(_corTransformation);
                _corTransformation = null;
            }
            _tweenTransformation.Kill();
            _tweenTransformation = null;

            StopAllMove();
            StopAllRotate();
            StopAllScale();
        }

        private void StopAllMove()
        {
            if (_corMove != null)
            {
                StopCoroutine(_corMove);
                _corMove = null;
            }
            _tweenMove.Kill();
            _tweenMove = null;
        }

        private void StopAllRotate()
        {
            if (_corRotate != null)
            {
                StopCoroutine(_corRotate);
                _corRotate = null;
            }
            _tweenRotate.Kill();
            _tweenRotate = null;
        }

        private void StopAllScale()
        {
            if (_corScale != null)
            {
                StopCoroutine(_corScale);
                _corScale = null;
            }
            _tweenScale.Kill();
            _tweenScale = null;
        }

        public void SetTransformation(IEnumerator ienumerator)
        {
            StopAllTransformation();
            _corTransformation = StartCoroutine(ienumerator);
        }
        public void SetTransformation(Tween tween)
        {
            StopAllTransformation();
            _tweenTransformation = tween;
        }

        public void SetMovement(IEnumerator ienumerator)
        {
            StopAllMove();
            _corMove = StartCoroutine(ienumerator);
        }

        public void SetMovement(Tween tween)
        {
            StopAllMove();
            _tweenMove = tween;
        }

        public void SetRotatation(IEnumerator ienumerator)
        {
            StopAllRotate();
            _corRotate = StartCoroutine(ienumerator);
        }

        public void SetRotatation(Tween tween)
        {
            StopAllRotate();
            _tweenRotate = tween;
        }

        public void SetScaling(IEnumerator ienumerator)
        {
            StopAllScale();
            _corScale = StartCoroutine(ienumerator);
        }

        public void SetScaling(Tween tween)
        {
            StopAllScale();
            _tweenScale = tween;
        }
        #endregion

        protected virtual void Awake()
        {
            if (BlockDragModifiers == null) BlockDragModifiers = new BoolModifierWithRegisteredSource(OnBlockDragModifiersChanged);
        }

        protected virtual void Start()
        {
            if (_isAutoSetUpOnStart && !_isDoneSetup) SetUp();
        }

        public virtual void SetUp()
        {
            if (_isDoneSetup) return;
            _isDoneSetup = true;

            if (_draggable)
            {
                _draggable.OnStartDrag.AddListener(OnStartDrag);
                _draggable.OnEndDrag.AddListener(OnEndDrag);
                if (BlockDragModifiers == null) BlockDragModifiers = new BoolModifierWithRegisteredSource(OnBlockDragModifiersChanged);
                // LevelBase.instance.onBlockPlayerInteractChanged += OnBlockPlayerInteractChanged;
            }
        }

        protected virtual void OnDestroy()
        {
            // if (LevelBase.instance) LevelBase.instance.onBlockPlayerInteractChanged -= OnBlockPlayerInteractChanged;
        }

        protected virtual void OnBlockPlayerInteractChanged()
        {
            const int BLOCK_DRAG_MODIFIER_ID = -1;
            // if (LevelBase.instance.IsAllowInteract)
            // {
            //     BlockDragModifiers.RemoveModifier(BLOCK_DRAG_MODIFIER_ID);
            // }
            // else
            // {
            //     BlockDragModifiers.AddModifier(BLOCK_DRAG_MODIFIER_ID);
            // }
        }

        private void OnBlockDragModifiersChanged()
        {
            if (_draggable)
            {
                if (BlockDragModifiers.Value)
                {
                    _draggable.isPreventDrag.AddModifier(this);
                }
                else
                {
                    _draggable.isPreventDrag.RemoveModifier(this);
                }
            }

            if (_colliders != null)
            {
                if (BlockDragModifiers.Value)
                {
                    if (_colliders != null) foreach (var collider in _colliders) collider.enabled = false;
                }
                else
                {
                    if (_colliders != null) foreach (var collider in _colliders) collider.enabled = true;
                }
            }
        }

        protected virtual void OnStartDrag()
        {
            StopAllMove();
            _draggable.transform.SetParent(null);

            try { onStartDragItem?.Invoke(this); }
            catch (Exception e) { Debug.LogException(e); }
        }

        protected virtual void OnEndDrag()
        {
            try { onEndDragItem?.Invoke(this); }
            catch (Exception e) { Debug.LogException(e); }
        }

        protected virtual void OnDrawGizmosSelected()
        {
            Color color = Color.white;
            color.a = 0.5f;
            Gizmos.color = color;
            Matrix4x4 backupMatrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(transform.position, Quaternion.identity, transform.lossyScale);
            GizmoUtility.DrawFilledRect(_groundBoundingBox, color);
            Gizmos.matrix = backupMatrix;
        }
    }
}