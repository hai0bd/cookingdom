using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

namespace Utilities
{
    public class DraggableObjectEffect : MonoBehaviour
    {
        [SerializeField] private DraggableObject _draggableObject;
        public float lerpSpeed = 0.2f;
        private float _timeStartLerp;

        public SpriteRenderer spriteRenderer;
        public float durationLerp = 0.25f;

        [Header("Placed")]
        public Vector3 offsetPlaced = Vector3.zero;
        public float rotationPlaced = 0f;
        public Color colorPlaced = Color.white;
        public float scalePlaced = 1f;

        [Header("Dragging")]
        public Vector3 offsetDragging = Vector3.zero;
        public float rotationDragging = 0f;
        public Color colorDragging = Color.white;
        public float scaleDragging = 1f;

        private Vector3 _originScale;
        private Quaternion _desiredRotation;
        private Color _desiredColor;
        private Vector3 _desiredScale;
        private Vector3 _desiredOffsetPosition;

        private void Awake()
        {
            _draggableObject.OnStartDrag.AddListener(OnStartDrag);
            _draggableObject.OnEndDrag.AddListener(OnEndDrag);
            _timeStartLerp = Time.time;

            SetUp();

            void SetUp()
            {
                _originScale = spriteRenderer.transform.localScale;

                _desiredScale = _originScale * scalePlaced;
                _desiredColor = colorPlaced;
                _desiredOffsetPosition = offsetPlaced;
                _desiredRotation = Quaternion.Euler(0, 0, rotationPlaced);

                spriteRenderer.transform.localScale = _desiredScale;
                spriteRenderer.color = _desiredColor;
                spriteRenderer.transform.position = _draggableObject.transform.position + _desiredOffsetPosition;
                spriteRenderer.transform.localRotation = _desiredRotation;
            }
        }

        private void OnStartDrag()
        {
            _timeStartLerp = Time.time;

            _desiredColor = colorDragging;
            _desiredScale = _originScale * scaleDragging;
            _desiredOffsetPosition = offsetDragging;
            _desiredRotation = Quaternion.Euler(0, 0, rotationDragging);
        }

        private void OnEndDrag()
        {
            _timeStartLerp = Time.time;

            _desiredColor = colorPlaced;
            _desiredScale = _originScale * scalePlaced;
            _desiredOffsetPosition = offsetPlaced;
            _desiredRotation = Quaternion.Euler(0, 0, rotationPlaced);
        }

        private void Update()
        {
            if (Time.time < _timeStartLerp + durationLerp)
            {
                float lerpValue = lerpSpeed * Time.deltaTime * 50f;

                spriteRenderer.transform.position = Vector3.Lerp(spriteRenderer.transform.position, _draggableObject.transform.position + _desiredOffsetPosition, lerpValue);
                spriteRenderer.color = Color.Lerp(spriteRenderer.color, _desiredColor, lerpValue);
                spriteRenderer.transform.localScale = Vector3.Lerp(spriteRenderer.transform.localScale, _desiredScale, lerpValue);
                spriteRenderer.transform.localRotation = Quaternion.Lerp(spriteRenderer.transform.localRotation, _desiredRotation, lerpValue);
            }
        }
    }
}