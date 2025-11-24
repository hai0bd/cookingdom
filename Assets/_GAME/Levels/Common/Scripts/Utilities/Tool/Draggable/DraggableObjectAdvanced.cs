using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Utilities
{
    public class DraggableObjectAdvanced : MonoBehaviour
    {
        private Vector3 mOffset;
        private float mZCoord;

        public BoolModifierWithRegisteredSource BlockDragModifiers { get; private set; }
        public bool IsAllowDrag => !BlockDragModifiers.Value;
        public event System.Action<DraggableObjectAdvanced> onStartDrag;
        public event System.Action<DraggableObjectAdvanced> onDrag;
        public event System.Action<DraggableObjectAdvanced> onEndDrag;

        public bool IsDragging { get; private set; }

        [Header("Clamp Position")]
        public DraggableModuleClamp clampModule;

        [Header("Detach")]
        [FoldoutGroup("Detach")] public bool isNeedDetach = false;
        [FoldoutGroup("Detach"), ShowIf("isNeedDetach")] public float distanceDragCauseDetach = 3f;
        [FoldoutGroup("Detach"), ShowIf("isNeedDetach")] public AnimationCurve distanceReduceRateByDragDistanceRatio = AnimationCurve.Linear(0, 0, 1, 1);

        private void Awake()
        {
            IsDragging = false;
            BlockDragModifiers = new BoolModifierWithRegisteredSource(OnChangedBlockDragModifier);

            void OnChangedBlockDragModifier()
            {
                if (BlockDragModifiers.Value)
                {
                    OnMouseUp();
                }
            }
        }

        void OnMouseDown()
        {
            if (BlockDragModifiers.Value || IsDragging) return;
            mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
            mOffset = gameObject.transform.position - GetMouseWorldPos();
            IsDragging = true;
            onStartDrag?.Invoke(this);
        }

        private Vector3 GetMouseWorldPos()
        {
            Vector3 mousePoint = Input.mousePosition;
            mousePoint.z = mZCoord;
            return Camera.main.ScreenToWorldPoint(mousePoint);
        }

        void OnMouseDrag()
        {
            if (IsDragging && Time.timeScale > 0)
            {
                Vector3 position = GetMouseWorldPos() + mOffset;
                if (clampModule) position = clampModule.Clamp(position);
                transform.position = position;
                onDrag?.Invoke(this);
            }
        }

        private void OnMouseUp()
        {
            if (IsDragging)
            {
                IsDragging = false;
                onEndDrag?.Invoke(this);
            }
        }

        public void CancelDragging()
        {
            OnMouseUp();
        }
    }
}