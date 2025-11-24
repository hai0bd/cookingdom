using System;
using System.Collections;
using DG.Tweening;
using MoreMountains.NiceVibrations;
using UnityEngine;
using Utilities;

namespace TidyCooking.Levels
{
    public class HarvestableRootItem : MonoBehaviour
    {
        public enum State
        {
            None,
            InDirt,
            Pulling,
            Detached
        }

        [System.Serializable]
        public class GeneralConfig
        {
            [Header("Appear")]
            public float durationAppear = 0.25f;
            public Ease easeScaleAppear = Ease.OutBack;

            [Header("Pull")]
            public float lerpSpeedStretch = 0.4f;
            [ValueCurve(true, true)] public AnimationCurve pullDistanceToStrength = AnimationCurve.Linear(0, 0, 3, 1);
            [ValueCurve(true, true)] public AnimationCurve pullStrengthToScaleX = AnimationCurve.Linear(0, 1, 1, 0.9f);
            [ValueCurve(true, true)] public AnimationCurve pullStrengthToScaleY = AnimationCurve.Linear(0, 1, 1, 1.5f);

            [Header("Shake Body")]
            public float lerpSpeedShakeFx = 0.4f;
            [ValueCurve(true, true)] public AnimationCurve pullStrengthToBodyShakeAmp = AnimationCurve.Linear(0, 0, 1, 0.1f);
            [ValueCurve(true, true)] public AnimationCurve pullStrengthToBodyShakeFreq = AnimationCurve.Linear(0, 0, 1, 100f);

            [Header("Shake Dirt")]
            [ValueCurve(true, true)] public AnimationCurve pullStrengthToDirtShakeAmp = AnimationCurve.Linear(0, 0, 1, 0.1f);
            [ValueCurve(true, true)] public AnimationCurve pullStrengthToDirtShakeFreq = AnimationCurve.Linear(0, 0, 1, 100f);

            [Header("Detach")]
            public int layerOnDetached = 10;
            public Vector2 offsetDetached = Vector2.up;
            public float heighAddY = 1f;
            public float durationFlyDetach = 0.5f;
            public AnimationCurve curveAddY = AnimationCurve.Constant(0, 1, 0);

            [Header("SFX")]
            public AudioSource sfxSource;
            public AudioClip sfxStartPull;
            public AudioClip sfxCancelPull;
            public AudioSource loopPullingSfxSource;
            [ValueCurve(true, true)] public AnimationCurve pullProgressToPitchSfxlPulling = AnimationCurve.Linear(0, 1, 0.8f, 1.5f);

            [Header("Haptic")]
            public LoopVibrateHaptic loopVibrateHaptic;
            public HapticTypes hapticStartPull = HapticTypes.LightImpact;
            public HapticTypes hapticCancelPull = HapticTypes.None;
            public HapticTypes hapticDetach = HapticTypes.HeavyImpact;
        }

        private GeneralConfig _generalConfig = null;
        public State CurrentState { get; private set; } = State.None;

        [Header("Pull")]
        [SerializeField] private Transform _scaleAnchor = null;
        [SerializeField] private ForwardMouseEvent _forwardMouseEvent = null;
        [SerializeField] private Collider2D _colliderPulling = null;
        [SerializeField] private ParticleSystem _vfxlPulling;
        [SerializeField] private float _totalStrengthToDetach = 3f;

        [Header("Fx")]
        [SerializeField] private LoopAnimVibratePosition _fxShakeBody;
        [SerializeField] private LoopAnimVibratePosition _fxShakeDirt;

        [Header("Detach")]
        [SerializeField] private CookItemIngredientScatter _cookItemIngredient;
        [SerializeField] private AudioClip sfxDetached;
        [SerializeField] private ParticleSystem _vfxDetached;
        [SerializeField] private SortingLayer _sortingLayer;

        private Camera _mainCamera;
        private Vector3 _posMouseDown;
        private float _strengthAccumulated = 0f;

        public event System.Action<HarvestableRootItem> onDetached;

        private void Awake()
        {
            _forwardMouseEvent.onMouseDown += OnPlayerMouseDown;
            _forwardMouseEvent.onMouseUp += OnPlayerMouseUp;
            // LevelBase.instance.onBlockPlayerInteractChanged += OnBlockPlayerInteractChanged;
        }

        private void OnBlockPlayerInteractChanged()
        {
            // if (!LevelBase.instance.IsAllowInteract) OnPlayerMouseUp(null);
        }

        private void OnDestroy()
        {
            // if (LevelBase.instance) LevelBase.instance.onBlockPlayerInteractChanged -= OnBlockPlayerInteractChanged;
        }

        public void SetUp(GeneralConfig generalConfig)
        {
            _generalConfig = generalConfig;
            _mainCamera = Camera.main;
            CurrentState = State.None;
            transform.localScale = Vector3.zero;
        }

        public void Appear()
        {
            CurrentState = State.InDirt;
            transform.DOScale(Vector3.one, _generalConfig.durationAppear).SetEase(_generalConfig.easeScaleAppear).Play();
        }

        private void OnPlayerMouseDown(ForwardMouseEvent @event)
        {
            // if (!LevelBase.instance.IsAllowInteract) return;
            if (CurrentState != State.InDirt) return;

            CurrentState = State.Pulling;
            _posMouseDown = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            _strengthAccumulated = 0f;

            _generalConfig.loopVibrateHaptic.enabled = true;
            _generalConfig.sfxSource.PlaySfx(_generalConfig.sfxStartPull);
            MMVibrationManager.Haptic(_generalConfig.hapticStartPull);
            _generalConfig.loopPullingSfxSource.Play();
            _vfxlPulling.PlayIfExisted();
        }

        private void OnPlayerMouseUp(ForwardMouseEvent @event)
        {
            if (CurrentState != State.Pulling) return;
            CurrentState = State.InDirt;
            _strengthAccumulated = 0f;

            _generalConfig.loopVibrateHaptic.enabled = false;
            _generalConfig.sfxSource.PlaySfx(_generalConfig.sfxCancelPull);
            MMVibrationManager.Haptic(_generalConfig.hapticCancelPull);
            _generalConfig.loopPullingSfxSource.Stop();
            _vfxlPulling.StopIfExisted();
        }

        private void Update()
        {
            Vector3 scale = _scaleAnchor.localScale;
            float shakeBodyAmp = _fxShakeBody.amplitude;
            float shakeBodyFreq = _fxShakeBody.frequency;
            float shakeDirtAmp = _fxShakeDirt ? _fxShakeDirt.amplitude : 0f;
            float shakeDirtFreq = _fxShakeDirt ? _fxShakeDirt.frequency : 0f;

            if (CurrentState == State.Pulling)
            {
                Vector3 posMouse = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
                //float distance = Vector3.Distance(_posMouseDown, posMouse);
                float distance = Mathf.Abs(_posMouseDown.y - posMouse.y);
                float strength = _generalConfig.pullDistanceToStrength.Evaluate(distance);

                scale.LerpTo(new Vector3(
                    _generalConfig.pullStrengthToScaleX.Evaluate(strength),
                    _generalConfig.pullStrengthToScaleY.Evaluate(strength),
                    1f
                ), _generalConfig.lerpSpeedStretch);

                shakeBodyAmp.LerpTo(_generalConfig.pullStrengthToBodyShakeAmp.Evaluate(strength), _generalConfig.lerpSpeedShakeFx);
                shakeBodyFreq.LerpTo(_generalConfig.pullStrengthToBodyShakeFreq.Evaluate(strength), _generalConfig.lerpSpeedShakeFx);

                if (_fxShakeDirt)
                {
                    shakeDirtAmp.LerpTo(_generalConfig.pullStrengthToDirtShakeAmp.Evaluate(strength), _generalConfig.lerpSpeedShakeFx);
                    shakeDirtFreq.LerpTo(_generalConfig.pullStrengthToDirtShakeFreq.Evaluate(strength), _generalConfig.lerpSpeedShakeFx);
                }

                _strengthAccumulated += strength * Time.deltaTime;
                _generalConfig.loopPullingSfxSource.pitch = _generalConfig.pullProgressToPitchSfxlPulling.Evaluate(_strengthAccumulated);

                if (_strengthAccumulated > _totalStrengthToDetach)
                {
                    Detach();
                }
            }
            else
            {
                scale.LerpTo(Vector3.one, _generalConfig.lerpSpeedStretch);

                shakeBodyAmp.LerpTo(0f, _generalConfig.lerpSpeedShakeFx);
                shakeBodyFreq.LerpTo(0.01f, _generalConfig.lerpSpeedShakeFx);
                if (_fxShakeDirt)
                {
                    shakeDirtAmp.LerpTo(0f, _generalConfig.lerpSpeedShakeFx);
                    shakeDirtFreq.LerpTo(0.01f, _generalConfig.lerpSpeedShakeFx);
                }
            }

            _scaleAnchor.localScale = scale;

            _fxShakeBody.amplitude = shakeBodyAmp;
            _fxShakeBody.frequency = shakeBodyFreq;

            if (_fxShakeDirt)
            {
                _fxShakeDirt.amplitude = shakeDirtAmp;
                _fxShakeDirt.frequency = shakeDirtFreq;
            }
        }

        private void Detach()
        {
            if (CurrentState == State.Detached) return;
            CurrentState = State.Detached;

            _generalConfig.loopVibrateHaptic.enabled = false;
            _generalConfig.loopPullingSfxSource.Stop();
            _vfxlPulling.StopIfExisted();
            _vfxDetached.PlayIfExisted();

            _cookItemIngredient.SetTransformation(IEFlyDetached());

            onDetached?.Invoke(this);

            _generalConfig.sfxSource.PlaySfx(sfxDetached);
            MMVibrationManager.Haptic(_generalConfig.hapticDetach);

            IEnumerator IEFlyDetached()
            {
                _colliderPulling.enabled = false;
                transform.SetParent(null);
                // _sortingLayer.Order = _generalConfig.layerOnDetached;
                Vector3 posStart = transform.position;
                Vector3 posEnd = posStart + (Vector3)_generalConfig.offsetDetached;
                float timeStart = Time.time;
                while (Time.time - timeStart < _generalConfig.durationFlyDetach)
                {
                    float t = (Time.time - timeStart) / _generalConfig.durationFlyDetach;
                    transform.position = Vector3.Lerp(posStart, posEnd, t) + Vector3.up * _generalConfig.curveAddY.Evaluate(t) * _generalConfig.heighAddY;
                    yield return null;
                }
            }
        }
    }
}