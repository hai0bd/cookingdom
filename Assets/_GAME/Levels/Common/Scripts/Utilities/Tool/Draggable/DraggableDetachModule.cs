using MoreMountains.NiceVibrations;
using System.Collections;
using UnityEngine;

namespace Utilities
{
    public class DraggableDetachModule : MonoBehaviour
    {
        public float distanceDragCauseDetach = 3f;
        public AnimationCurve distanceReduceRateByDragDistanceRatio = AnimationCurve.Linear(0, 0, 1, 1);

        private float _realDistanceDragCauseDetach;

        public bool IsDetached { get; private set; } = false;

        [Header("Drag")]
        public AnimationCurve dragMoveBeforeDetach = AnimationCurve.Linear(0, 0, 1, 1);
        public float positionLerpSpeed = 0.5f;
        public HapticTypes hapticDrag = HapticTypes.SoftImpact;
        public float deltaDistanceDragTriggerHaptic = 0.5f;

        [Header("Detach")]
        public AudioClip sfxDetach;
        public Vector2 sfxDetachRandoPitch = new Vector2(0.9f, 1.1f);
        public HapticTypes hapticDetach = HapticTypes.MediumImpact;

        public int numTouchCauseDetach = 3;
        public float dragDistanceCauseDetach = 0.5f;
        public Transform shakeTransform;
        public ParticleSystem vfxDetach;
        public SpriteRenderer spriteRenderer;

        private int _numTouched = 0;

        private bool _isDragging;
        private bool _isDetached;
        private Vector3 _startDragPos;
        private Vector3 _startDragLocalPos;
        private float _lastDistanceDragTriggerHaptic;

        //private Vector3 _velocity;
        //private Vector3 _desiredPosition;
        //private Transform _transform;
        //private Camera _mainCamera;
        //private System.Action<TrashAttachedOnPet> _onDetachedAndThrow;

        //private void Start()
        //{
        //    _isDetached = false;
        //    _isDragging = false;
        //    _numTouched = 0;
        //    _transform = transform;
        //    _mainCamera = Camera.main;
        //}

        //public void SetUp(GeneralConfig generalConfig, System.Action<TrashAttachedOnPet> onDetachedAndThrow)
        //{
        //    _generalConfig = generalConfig;
        //    _onDetachedAndThrow = onDetachedAndThrow;
        //}

        //private void OnMouseDown()
        //{
        //    if (!LevelBase.instance.IsAllowInteract) return;
        //    if (_isDetached) return;

        //    _numTouched++;
        //    if (_numTouched >= numTouchCauseDetach)
        //    {
        //        Detach();
        //    }

        //    _isDragging = true;
        //    _tweenAnim?.Kill();
        //    _tweenAnim = shakeTransform.DOShakePosition(shakeDurationOnTouch, shakeStrengthOnTouch).Play();
        //    _startDragPos = transform.position;
        //    _startDragLocalPos = transform.localPosition;
        //    _lastDistanceDragTriggerHaptic = 0;

        //    audioSource.PlayOneShot(sfxTouch, Random.Range(sfxTouchRandoPitch.x, sfxTouchRandoPitch.y));
        //    MMVibrationManager.Haptic(hapticTouch);
        //}

        //private void Detach()
        //{
        //    _isDetached = true;
        //    _transform.SetParent(null);
        //    spriteRenderer.sortingOrder = renderOrderOnDetach;
        //    if (vfxDetach != null)
        //    {
        //        vfxDetach.Play();
        //    }
        //    else
        //    {
        //        vfxDetach.PlayIfExisted(transform.position);
        //    }
        //    audioSource.PlayOneShot(sfxDetach, Random.Range(sfxDetachRandoPitch.x, sfxDetachRandoPitch.y));
        //    MMVibrationManager.Haptic(hapticDetach);
        //}

        //private void OnMouseUp()
        //{
        //    _isDragging = false;
        //    if (_isDetached)
        //    {
        //        // throw
        //        var dir = (transform.position - _startDragPos).normalized;
        //        _velocity = dir * throwInitVelocity;
        //        audioSource.PlayOneShot(sfxThrow, Random.Range(sfxThrowRandoPitch.x, sfxThrowRandoPitch.y));
        //        _onDetachedAndThrow?.Invoke(this);
        //        _onDetachedAndThrow = null;
        //    }
        //}

        //private void Update()
        //{
        //    if (_isDragging)
        //    {
        //        if (_isDetached)
        //        {
        //            _desiredPosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        //            _desiredPosition.z = _transform.position.z;
        //            _transform.position = Vector3.Lerp(_transform.position, _desiredPosition, positionLerpSpeed * Time.deltaTime * 50f);
        //        }
        //        else
        //        {
        //            Vector2 mousePos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        //            float dragDistance = (mousePos - (Vector2)_startDragPos).magnitude;

        //            if (dragDistance > dragDistanceCauseDetach)
        //            {
        //                Detach();
        //            }
        //            else
        //            {
        //                Vector2 mouseLocalPos = mousePos;
        //                if (_transform.parent) mouseLocalPos = _transform.parent.InverseTransformPoint(mousePos);
        //                Vector2 dragLocalDir = (mouseLocalPos - (Vector2)_startDragLocalPos).normalized;

        //                _transform.localPosition = _startDragLocalPos + (Vector3)dragLocalDir * dragMoveBeforeDetach.Evaluate(dragDistance / dragDistanceCauseDetach);

        //                if (dragDistance - _lastDistanceDragTriggerHaptic > deltaDistanceDragTriggerHaptic)
        //                {
        //                    _lastDistanceDragTriggerHaptic = dragDistance;
        //                    MMVibrationManager.Haptic(hapticDrag);
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        if (_isDetached)
        //        {
        //            // fall
        //            _velocity.y -= gravity * Time.deltaTime;
        //            _transform.position += _velocity * Time.deltaTime;
        //            if (_transform.position.y < positionYDisappeared)
        //            {
        //                gameObject.SetActive(false);
        //            }
        //        }
        //        else
        //        {
        //            // nothing happen
        //        }
        //    }
        //}
    }
}