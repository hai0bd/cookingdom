using System;
using UnityEngine;

namespace Utilities
{
    public class DraggableObject : MonoBehaviour
    {
        private Vector3 mOffset;
        private float mZCoord;

        public BoolModifierWithRegisteredSource isPreventDrag;
        public UnityEngine.Events.UnityEvent OnStartDrag;
        public UnityEngine.Events.UnityEvent OnDrag;
        public UnityEngine.Events.UnityEvent OnEndDrag;

        public bool IsDragging { get; private set; }

        public bool isClampPosition = false;
        public bool isClampRelativeToViewport = false;
        public Rect areaClampPosition = new Rect(0.05f, 0.1f, 0.9f, 0.85f);

        private void Awake()
        {
            IsDragging = false;
            isPreventDrag = new BoolModifierWithRegisteredSource(OnChangedCanDrag);
        }

        private void OnChangedCanDrag()
        {
            if (isPreventDrag.Value)
            {
                OnMouseUp();
            }
        }

        void OnMouseDown()
        {
            if (isPreventDrag.Value || IsDragging) return;
            mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
            mOffset = gameObject.transform.position - GetMouseWorldPos();
            IsDragging = true;
            OnStartDrag.Invoke();
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
                if (isClampPosition)
                {
                    if (isClampRelativeToViewport)
                    {
                        Vector2 min = Camera.main.ViewportToWorldPoint(new Vector3(areaClampPosition.xMin, areaClampPosition.yMin, 0));
                        Vector2 max = Camera.main.ViewportToWorldPoint(new Vector3(areaClampPosition.xMax, areaClampPosition.yMax, 0));
                        position.x = Mathf.Clamp(position.x, min.x, max.x);
                        position.y = Mathf.Clamp(position.y, min.y, max.y);
                    }
                    else
                    {
                        position.x = Mathf.Clamp(position.x, areaClampPosition.xMin, areaClampPosition.xMax);
                        position.y = Mathf.Clamp(position.y, areaClampPosition.yMin, areaClampPosition.yMax);
                    }
                }
                transform.position = position;
                OnDrag.Invoke();
            }
        }

        private void OnMouseUp()
        {
            if (IsDragging)
            {
                IsDragging = false;
                OnEndDrag.Invoke();
            }
        }

        public void CancelDragging()
        {
            OnMouseUp();
        }

        private void OnDrawGizmosSelected()
        {
            if (isClampPosition)
            {
                if (isClampRelativeToViewport)
                {
                    Vector2 min = Camera.main.ViewportToWorldPoint(new Vector3(areaClampPosition.xMin, areaClampPosition.yMin, 0));
                    Vector2 max = Camera.main.ViewportToWorldPoint(new Vector3(areaClampPosition.xMax, areaClampPosition.yMax, 0));
                    GizmoUtility.DrawRect(new Rect(min, max - min), Color.green);
                }
                else
                {
                    GizmoUtility.DrawRect(areaClampPosition, Color.green);
                }
            }
        }
    }
}