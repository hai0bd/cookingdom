using System;
using System.Collections;
using DG.Tweening;
using Sirenix.OdinInspector;
using Spine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Utilities
{
    // a draggable UI button that can be dragged around the screen and can click if not dragged (use a threshold to determine if it was dragged or not), the button is auto smooth snap to edge of screen with a configurable offset value
    public class UIDraggableFloatingButton : MonoBehaviour
    {
        public enum HorizontalSnapDirection
        {
            None,
            Left,
            Right,
            Both
        }

        public enum VerticalSnapDirection
        {
            None,
            Top,
            Bottom,
            Both
        }

        [System.Serializable]
        public class GeneralConfig
        {
            [Header("Drag")]
            [Range(0f, 1f)] public float lerpPositionSpeed = 0.5f;

            [Header("Snap")]
            public float edgeSnapOffsetLeft = 0f;
            public float edgeSnapOffsetRight = 0f;
            public float edgeSnapOffsetTop = 0f;
            public float edgeSnapOffsetBottom = 0f;
            public float snapSpeed = 10f;
            public float maxSnapDuration = 0.5f;

            [Header("Click")]
            public float thresholdMoveDistanceAllowClick = 1f;
            public float thresholdHoldDurationAllowClick = 0.25f;
        }

        [SerializeField] private ForwardUIPointerEvent _forwardMouseEvent;
        [SerializeField] private RectTransform _controlledTransform;
        [SerializeField] private RectTransform _draggableRectArea;
        [SerializeField] private GeneralConfig _config;
        [SerializeField] private HorizontalSnapDirection _horizontalSnapDirection = HorizontalSnapDirection.None;
        [SerializeField] private VerticalSnapDirection _verticalSnapDirection = VerticalSnapDirection.None;

        public RectTransform ControlledTransform => _controlledTransform;
        public BoolModifierWithRegisteredSource BlockDragModifier { get; private set; }

        public event System.Action onClick;
        public event System.Action onDragEnd;

        private bool _isDragging;
        private Vector2 _dragStartMousePos;
        private Vector2 _dragStartAnchoredPos;
        private float _dragStartTime;

        private Canvas _canvas;
        private Tween _snapTween;

        private void Awake()
        {
            _forwardMouseEvent.onMouseDown += OnPlayerPointerDown;
            _forwardMouseEvent.onMouseUp += OnPlayerPointerUp;

            if (_draggableRectArea == null && this.transform.parent)
            {
                _draggableRectArea = this.transform.parent.GetComponent<RectTransform>();
            }

            BlockDragModifier = new BoolModifierWithRegisteredSource(OnChangedBlockDragModifier);

            void OnChangedBlockDragModifier()
            {
                if (BlockDragModifier.Value)
                {
                    _isDragging = false;
                    Snap();
                }
            }
        }

        private void OnEnable()
        {
            Snap();
        }

        private void OnPlayerPointerDown(ForwardUIPointerEvent @event, PointerEventData data)
        {
            if (BlockDragModifier.Value) return;
            if (_isDragging) return;
            _isDragging = true;
            _snapTween.Kill();
            _dragStartMousePos = data.position;
            _dragStartAnchoredPos = _controlledTransform.anchoredPosition;
            _dragStartTime = Time.time;

            Canvas[] canvasParents = _draggableRectArea.GetComponentsInParent<Canvas>();
            _canvas = (canvasParents.Length > 0) ? canvasParents[canvasParents.Length - 1] : null;
        }

        private void Update()
        {
            if (_isDragging)
            {
                Vector2 desiredPos = _dragStartAnchoredPos + ((Vector2)Input.mousePosition - _dragStartMousePos) / (_canvas ? _canvas.scaleFactor : 1f);
                desiredPos.x = Mathf.Clamp(desiredPos.x, _draggableRectArea.rect.xMin, _draggableRectArea.rect.xMax);
                desiredPos.y = Mathf.Clamp(desiredPos.y, _draggableRectArea.rect.yMin, _draggableRectArea.rect.yMax);
                Vector2 anchoredPos = _controlledTransform.anchoredPosition;
                anchoredPos.LerpToWithUnscaledTime(desiredPos, _config.lerpPositionSpeed);
                _controlledTransform.anchoredPosition = anchoredPos;
            }
        }

        public void OnPlayerPointerUp(ForwardUIPointerEvent @event, PointerEventData data)
        {
            if (BlockDragModifier.Value) return;
            if (!_isDragging) return;
            _isDragging = false;

            float dragDuration = Time.time - _dragStartTime;
            float dragDistance = Vector2.Distance(_dragStartMousePos, data.position);

            if (dragDistance < _config.thresholdMoveDistanceAllowClick && dragDuration < _config.thresholdHoldDurationAllowClick)
            {
                onClick?.Invoke();
            }
            else
            {
                onDragEnd?.Invoke();
            }

            Snap();
        }

        private void Snap()
        {
            Vector2 desiredPos = _dragStartAnchoredPos + ((Vector2)Input.mousePosition - _dragStartMousePos);
            Snap(desiredPos);
        }

        public void Snap(Vector2 desiredPos)
        {
            desiredPos.x = Mathf.Clamp(desiredPos.x, _draggableRectArea.rect.xMin, _draggableRectArea.rect.xMax);
            desiredPos.y = Mathf.Clamp(desiredPos.y, _draggableRectArea.rect.yMin, _draggableRectArea.rect.yMax);

            switch (_horizontalSnapDirection)
            {
                case HorizontalSnapDirection.Left:
                    desiredPos.x = _draggableRectArea.rect.xMin + _config.edgeSnapOffsetLeft;
                    break;
                case HorizontalSnapDirection.Right:
                    desiredPos.x = _draggableRectArea.rect.xMax - _config.edgeSnapOffsetRight;
                    break;
                case HorizontalSnapDirection.Both:
                    if (Mathf.Abs(desiredPos.x - (_draggableRectArea.rect.xMin + _config.edgeSnapOffsetLeft)) < Mathf.Abs(desiredPos.x - (_draggableRectArea.rect.xMax - _config.edgeSnapOffsetRight)))
                    {
                        desiredPos.x = _draggableRectArea.rect.xMin + _config.edgeSnapOffsetLeft;
                    }
                    else
                    {
                        desiredPos.x = _draggableRectArea.rect.xMax - _config.edgeSnapOffsetRight;
                    }
                    break;
            }

            switch (_verticalSnapDirection)
            {
                case VerticalSnapDirection.Top:
                    desiredPos.y = _draggableRectArea.rect.yMax - _config.edgeSnapOffsetTop;
                    break;
                case VerticalSnapDirection.Bottom:
                    desiredPos.y = _draggableRectArea.rect.yMin + _config.edgeSnapOffsetBottom;
                    break;
                case VerticalSnapDirection.Both:
                    if (Mathf.Abs(desiredPos.y - (_draggableRectArea.rect.yMin + _config.edgeSnapOffsetBottom)) < Mathf.Abs(desiredPos.y - (_draggableRectArea.rect.yMax - _config.edgeSnapOffsetTop)))
                    {
                        desiredPos.y = _draggableRectArea.rect.yMin + _config.edgeSnapOffsetBottom;
                    }
                    else
                    {
                        desiredPos.y = _draggableRectArea.rect.yMax - _config.edgeSnapOffsetTop;
                    }
                    break;
            }

            float durationSnap = Vector2.Distance(_controlledTransform.anchoredPosition, desiredPos) / _config.snapSpeed;
            if (durationSnap > _config.maxSnapDuration) durationSnap = _config.maxSnapDuration;

            _snapTween.Kill();
            // _snapTween = _controlledTransform.DOAnchorPos(desiredPos, durationSnap)
            //     .SetEase(Ease.OutQuad)
            //     .Play();
        }

        private void OnDisable()
        {
            _snapTween.Kill();
        }
    }
}
