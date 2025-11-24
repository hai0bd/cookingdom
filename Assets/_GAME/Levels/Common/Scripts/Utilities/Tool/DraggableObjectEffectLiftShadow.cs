using System;
using System.Collections;
using DG.Tweening;
using MoreMountains.NiceVibrations;
using UnityEngine;

namespace Utilities
{
    [RequireComponent(typeof(DraggableObject))]
    public class DraggableObjectEffectLiftShadow : MonoBehaviour
    {
        [System.Serializable]
        public class GeneralConfig
        {
            public float lerpSpeed = 0.5f;
            public float maxLerpDuration = 1f;

            [Header("Main")]
            public float liftHeightOnDrag = .5f;
            public float scaleMulOnDrag = 1.05f;
            public float rotateOnDrag = 10f;
            [Range(0f, 1f)] public float minRotateRatio = 0.5f;

            [Header("Shadow")]
            public float shadowScaleMulOnDrag = .5f;
            public float shadowAlphaMulOnDrag = 0f;

            [Header("Audio-Haptic")]
            public AudioSource audioSource;
            public AudioClip sfxPick;
            public HapticTypes hapticPick = HapticTypes.None;
            public AudioClip sfxRelease;
            public HapticTypes hapticRelease = HapticTypes.None;
        }

        [SerializeField] private DraggableObject _draggable;
        [SerializeField] private Transform _liftTransform;
        [SerializeField] private Transform _scaleTransform;
        [SerializeField] private Transform _rotateTransform;
        [SerializeField] private SpriteRenderer _shadowRenderer;
        [SerializeField] private Transform _shadowTransform;
        [SerializeField] private GeneralConfig _generalConfig;

        private Vector3 _originPos;
        private float _originAngle;
        private Vector3 _originScale;
        private Vector3 _originShadowScale;
        private float _originShadowAlpha;

        public event System.Action onTweenDrag;
        public event System.Action onTweenNormal;

        public event System.Action onSetDrag;
        public event System.Action onSetNormal;

        private Coroutine _corTween;
        private bool _isSetUp = false;

        private void SetUpOriginData()
        {
            _isSetUp = true;

            var draggableObject = GetComponent<DraggableObject>();
            draggableObject.OnStartDrag.AddListener(OnStartDrag);
            draggableObject.OnEndDrag.AddListener(OnEndDrag);

            _originPos = _liftTransform ? _liftTransform.localPosition : Vector3.zero;
            _originAngle = _rotateTransform ? _rotateTransform.eulerAngles.z : 0f;
            _originScale = _scaleTransform ? _scaleTransform.localScale : Vector3.one;
            _originShadowScale = _shadowTransform ? _shadowTransform.localScale : Vector3.one;
            _originShadowAlpha = _shadowRenderer ? _shadowRenderer.color.a : 1f;
        }

        private void OnStartDrag()
        {
            if (!_isSetUp) SetUpOriginData();

            if (_corTween != null) StopCoroutine(_corTween);
            _corTween = StartCoroutine(IETweenDrag());

            _generalConfig.audioSource?.PlaySfx(_generalConfig.sfxPick);
            MMVibrationManager.Haptic(_generalConfig.hapticPick);
        }

        private void OnEndDrag()
        {
            if (!_isSetUp) SetUpOriginData();

            if (_corTween != null) StopCoroutine(_corTween);
            _corTween = StartCoroutine(IETweenNormal());

            _generalConfig.audioSource?.PlaySfx(_generalConfig.sfxRelease);
            MMVibrationManager.Haptic(_generalConfig.hapticRelease);
        }

        private IEnumerator IETweenDrag()
        {
            float timeEnd = Time.time + _generalConfig.maxLerpDuration;
            Vector3 liftPos = _originPos + Vector3.up * _generalConfig.liftHeightOnDrag;
            float rotate = UnityEngine.Random.Range(_generalConfig.minRotateRatio, 1f) * _generalConfig.rotateOnDrag * (UnityEngine.Random.value > 0.5f ? 1f : -1f);
            Quaternion rot = Quaternion.Euler(0, 0, _originAngle + rotate);
            Vector3 scale = _originScale * _generalConfig.scaleMulOnDrag;
            Vector3 shadowScale = _originShadowScale * _generalConfig.shadowScaleMulOnDrag;
            Color shadowColor = _shadowRenderer ? _shadowRenderer.color : Color.black;
            shadowColor.a = _originShadowAlpha * _generalConfig.shadowAlphaMulOnDrag;
            float lerp;
            while (Time.time < timeEnd)
            {
                lerp = Time.deltaTime * 50f * _generalConfig.lerpSpeed;
                if (_liftTransform) _liftTransform.localPosition = Vector3.LerpUnclamped(_liftTransform.localPosition, liftPos, lerp);
                if (_rotateTransform) _rotateTransform.localRotation = Quaternion.SlerpUnclamped(_rotateTransform.localRotation, rot, lerp);
                if (_scaleTransform) _scaleTransform.localScale = Vector3.LerpUnclamped(_scaleTransform.localScale, scale, lerp);
                if (_shadowTransform) _shadowTransform.localScale = Vector3.LerpUnclamped(_shadowTransform.localScale, shadowScale, lerp);
                if (_shadowRenderer) _shadowRenderer.color = Color.LerpUnclamped(_shadowRenderer.color, shadowColor, lerp);
                onTweenDrag?.Invoke();
                yield return null;
            }
            SetDragImmediate();
            if (_rotateTransform)  _rotateTransform.localRotation = Quaternion.Euler(0, 0, _originAngle + rotate);
            _corTween = null;
        }

        private void SetDragImmediate()
        {
            if (_liftTransform) _liftTransform.localPosition = _originPos + Vector3.up * _generalConfig.liftHeightOnDrag;
            if (_rotateTransform)
            {
                float rotate = UnityEngine.Random.Range(_generalConfig.minRotateRatio, 1f) * _generalConfig.rotateOnDrag * (UnityEngine.Random.value > 0.5f ? 1f : -1f);
                _rotateTransform.localRotation = Quaternion.Euler(0, 0, _originAngle + rotate);
            }
            if (_scaleTransform) _scaleTransform.localScale = _originScale * _generalConfig.scaleMulOnDrag;
            if (_shadowTransform) _shadowTransform.localScale = _originShadowScale * _generalConfig.shadowScaleMulOnDrag;
            if (_shadowRenderer)
            {
                Color newColor = _shadowRenderer.color;
                newColor.a = _originShadowAlpha * _generalConfig.shadowAlphaMulOnDrag;
                _shadowRenderer.color = newColor;
            }
            onSetDrag?.Invoke();
        }

        private IEnumerator IETweenNormal()
        {
            float timeEnd = Time.time + _generalConfig.maxLerpDuration;
            Quaternion originRot = Quaternion.Euler(0, 0, _originAngle);

            Color shadowColor = _shadowRenderer ? _shadowRenderer.color : Color.black;
            shadowColor.a = _originShadowAlpha;

            float lerp;
            while (Time.time < timeEnd)
            {
                lerp = Time.deltaTime * 50f * _generalConfig.lerpSpeed;
                if (_liftTransform) _liftTransform.localPosition = Vector3.LerpUnclamped(_liftTransform.localPosition, _originPos, lerp);
                if (_rotateTransform) _rotateTransform.localRotation = Quaternion.SlerpUnclamped(_rotateTransform.localRotation, originRot, lerp);
                if (_scaleTransform) _scaleTransform.localScale = Vector3.LerpUnclamped(_scaleTransform.localScale, _originScale, lerp);
                if (_shadowTransform) _shadowTransform.localScale = Vector3.LerpUnclamped(_shadowTransform.localScale, _originShadowScale, lerp);
                if (_shadowRenderer) _shadowRenderer.color = Color.LerpUnclamped(_shadowRenderer.color, shadowColor, lerp);
                onTweenNormal?.Invoke();
                yield return null;
            }

            SetNormalImmediate();
            _corTween = null;
        }

        private void SetNormalImmediate()
        {
            if (_liftTransform) _liftTransform.localPosition = _originPos;
            if (_rotateTransform) _rotateTransform.localRotation = Quaternion.Euler(0, 0, _originAngle);
            if (_scaleTransform) _scaleTransform.localScale = _originScale;
            if (_shadowTransform) _shadowTransform.localScale = _originShadowScale;
            if (_shadowRenderer)
            {
                Color newColor = _shadowRenderer.color;
                newColor.a = _originShadowAlpha;
                _shadowRenderer.color = newColor;
            }
            onSetNormal?.Invoke();
        }

        private void OnEnable()
        {
            if (!_isSetUp) SetUpOriginData();

            if (_corTween != null) StopCoroutine(_corTween);
            if (_draggable.IsDragging)
            {
                SetDragImmediate();
            }
            else
            {
                SetNormalImmediate();
            }
        }
    }
}