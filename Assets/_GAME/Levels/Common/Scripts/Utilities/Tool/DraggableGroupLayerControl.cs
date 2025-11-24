using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    public interface IDraggableLayerBehaviour
    {
        event Action<IDraggableLayerBehaviour> onDragStart;
        event Action<IDraggableLayerBehaviour> onEndStart;
        void SetSortOrder(int value);
        void SetZPosition(float value);
    }

    public class DraggableGroupLayerControl : MonoBehaviour
    {
        [SerializeField] private GameObject[] _draggableObjectLayerBehaviours;
        [SerializeField] private int _initialSortOrder = 0;
        [SerializeField] private float _initialZPosition = 0;
        [SerializeField] private float _zPosInc = -0.001f;
        private int _currentSortOrder;
        private float _currentZPosition;

        private void Start()
        {
            IDraggableLayerBehaviour draggableLayer;
            foreach (var draggableObjectLayerBehaviour in _draggableObjectLayerBehaviours)
            {
                draggableLayer = draggableObjectLayerBehaviour.GetComponent<IDraggableLayerBehaviour>();
                if (draggableLayer != null)
                {
                    draggableLayer.onDragStart += OnDragStart;
                    draggableLayer.onEndStart += OnEndDrag;
                }
            }
            _currentSortOrder = _initialSortOrder;
            _currentZPosition = _initialZPosition;
        }

        private void OnDragStart(IDraggableLayerBehaviour behaviour)
        {
            _currentSortOrder++;
            _currentZPosition += _zPosInc;
            behaviour.SetSortOrder(_currentSortOrder);
            behaviour.SetZPosition(_currentZPosition);
        }
        private void OnEndDrag(IDraggableLayerBehaviour behaviour)
        {
            //_currentSortOrder++;
            //_currentZPosition += _zPosInc;
            //behaviour.SetSortOrder(_currentSortOrder);
            behaviour.SetZPosition(_currentZPosition);
        }

#if UNITY_EDITOR
        [Sirenix.OdinInspector.Button]
        private void LoadAllInside()
        {
            //_draggableObjectLayerBehaviours = GetComponentsInChildren<DraggableObjectLayerBehaviour>();

            // Get all children recursively
            List<GameObject> result = new List<GameObject>();
            Queue<Transform> queue = new Queue<Transform>();
            queue.Enqueue(transform);
            IDraggableLayerBehaviour behaviour;
            while (queue.Count > 0)
            {
                var trans = queue.Dequeue();
                for (int i = 0; i < trans.childCount; i++) queue.Enqueue(trans.GetChild(i));
                if (trans.gameObject.TryGetComponent<IDraggableLayerBehaviour>(out behaviour))
                {
                    result.Add(trans.gameObject);
                }
            }
            _draggableObjectLayerBehaviours = result.ToArray();

            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }
}