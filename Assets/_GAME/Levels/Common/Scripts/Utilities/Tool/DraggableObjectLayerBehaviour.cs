using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace Utilities
{
    [RequireComponent(typeof(DraggableObject))]
    public class DraggableObjectLayerBehaviour : MonoBehaviour, IDraggableLayerBehaviour
    {
        [Header("Main")]
        public SortingGroup sortingGroup;
        public SpriteRenderer spriteRenderer;
        public Transform objTransform;
        public event System.Action<IDraggableLayerBehaviour> onDragStart;
        public event System.Action<IDraggableLayerBehaviour> onEndStart;
        public int CurrentSortOrder => sortingGroup ? sortingGroup.sortingOrder : spriteRenderer ? spriteRenderer.sortingOrder : 0;
        public float CurrentZPosition => objTransform ? objTransform.position.z : 0f;

        public void SetSortOrder(int value)
        {
            if (sortingGroup) sortingGroup.sortingOrder = value;
            else if (spriteRenderer) spriteRenderer.sortingOrder = value;
        }

        public void SetZPosition(float value)
        {
            if (!objTransform) objTransform = transform;
            Vector3 pos = objTransform.localPosition;
            pos.z = value;
            objTransform.localPosition = pos;
        }

        private void Start()
        {
            var draggableObject = GetComponent<DraggableObject>();
            draggableObject.OnStartDrag.AddListener(OnStartDrag);
            draggableObject.OnEndDrag.AddListener(OnEndDrag);
        }

        private void OnStartDrag()
        {
            onDragStart?.Invoke(this);
        }

        private void OnEndDrag()
        {
            onEndStart?.Invoke(this);
        }
    }
}