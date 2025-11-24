using System.Collections;
using MoreMountains.NiceVibrations;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Utilities
{
    public class UISliderMultipeImage : MonoBehaviour
    {
        [System.Serializable]
        public class EffectConfig
        {
            public GameObject vfxObj;
            public AudioClip sfx;
            public HapticTypes haptic = HapticTypes.None;

            public void Play(RectTransform parent, AudioSource audioSource = null)
            {
                if (vfxObj != null && vfxObj.TryGetComponent<IParticleEffect>(out var particle))
                {
                    RectTransform tf = vfxObj.GetComponent<RectTransform>();
                    tf.SetParent(parent);
                    tf.localScale = Vector3.one;
                    tf.anchoredPosition = Vector2.zero;
                    particle.Play();
                }
                if (audioSource != null && sfx != null)
                {
                    audioSource.PlaySfx(sfx);
                }
                MMVibrationManager.Haptic(haptic);
            }
        }

        [System.Serializable]
        public class MoveSliderAnimConfig
        {
            public AudioSource audioSource;
            public float sliderSpeed = 5f;
            public Vector2 limitTimeMoveSlider = new Vector2(0.1f, 1f);
            public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

            public EffectConfig fxGain;
            public EffectConfig fxGainFull;
            public EffectConfig fxLose;
            public EffectConfig fxLoseFull;
        }

        private float _currentVisualValue = 0f;
        [SerializeField] private float _currentValue = 0;
        [SerializeField] private Image[] _imgFillSequence;
        public Image[] ImgFillSequence => _imgFillSequence;
        public int MaxValue => _imgFillSequence != null ? _imgFillSequence.Length : 0;

        [Header("Anim & Effect")]
        [SerializeField] private MoveSliderAnimConfig _moveSliderAnimConfig;
        private Coroutine _coroutineUpdateVisual;

        public void UpdateImgSequence(Image[] imgFillSequence)
        {
            _imgFillSequence = imgFillSequence;
            _currentValue = Mathf.Clamp(_currentValue, 0f, MaxValue);
            _currentVisualValue = _currentValue;
            UpdateFill();
        }

        public int ValueInt => (int)_currentValue;
        public void SetValueIntImmediate(int value) => SetValueImmediate(value);

        public float Value => _currentValue;
        public void SetValueImmediate(float value)
        {
            _currentValue = Mathf.Clamp(value, 0f, MaxValue);

            if (_coroutineUpdateVisual != null)
            {
                StopCoroutine(_coroutineUpdateVisual);
                _coroutineUpdateVisual = null;
            }
            _currentVisualValue = _currentValue;
            UpdateFill();
        }

        public void ChangeValue(float value, float animSpeed = 1f)
        {
            _currentValue = Mathf.Clamp(value, 0f, MaxValue);

            if (_coroutineUpdateVisual != null) StopCoroutine(_coroutineUpdateVisual);
            _coroutineUpdateVisual = StartCoroutine(IEChangeValue());

            IEnumerator IEChangeValue()
            {
                float timeStart = Time.unscaledTime;
                float startValue = _currentVisualValue;

                float duration = (_currentValue - startValue) / _moveSliderAnimConfig.sliderSpeed;
                duration = Mathf.Clamp(duration, _moveSliderAnimConfig.limitTimeMoveSlider.x, _moveSliderAnimConfig.limitTimeMoveSlider.y);
                duration /= Mathf.Max(0.1f, animSpeed);

                int imgIndexStartCheckFx;
                float fillAmountOnImgStartCheckFx;
                if (startValue < _currentValue) // up
                {
                    imgIndexStartCheckFx = _imgFillSequence.Length;
                    fillAmountOnImgStartCheckFx = 0f;
                    for (int i = 0; i < _imgFillSequence.Length; i++)
                    {
                        if (_imgFillSequence[i].fillAmount < 1f - Mathf.Epsilon)
                        {
                            imgIndexStartCheckFx = i;
                            fillAmountOnImgStartCheckFx = _imgFillSequence[i].fillAmount;
                            break;
                        }
                    }
                }
                else // down
                {
                    imgIndexStartCheckFx = 0;
                    fillAmountOnImgStartCheckFx = 1f;
                    for (int i = _imgFillSequence.Length - 1; i >= 0; i--)
                    {
                        if (_imgFillSequence[i].fillAmount > 0f + Mathf.Epsilon)
                        {
                            imgIndexStartCheckFx = i;
                            fillAmountOnImgStartCheckFx = _imgFillSequence[i].fillAmount;
                            break;
                        }
                    }
                }

                while (Time.unscaledTime < timeStart + duration)
                {
                    float t = (Time.unscaledTime - timeStart) / duration;
                    _currentVisualValue = Mathf.Lerp(startValue, _currentValue, _moveSliderAnimConfig.curve.Evaluate(t));
                    UpdateFill();

                    if (startValue < _currentValue) // up
                    {
                        if (imgIndexStartCheckFx < _imgFillSequence.Length)
                        {
                            if (_currentVisualValue > imgIndexStartCheckFx)
                            {
                                if (fillAmountOnImgStartCheckFx <= 0f + Mathf.Epsilon && (_currentValue - imgIndexStartCheckFx >= 1f - Mathf.Epsilon))
                                {
                                    _moveSliderAnimConfig.fxGainFull.Play(_imgFillSequence[imgIndexStartCheckFx].rectTransform, _moveSliderAnimConfig.audioSource);
                                    imgIndexStartCheckFx += 1;
                                    fillAmountOnImgStartCheckFx = 0f;
                                }
                                else
                                {
                                    _moveSliderAnimConfig.fxGain.Play(_imgFillSequence[imgIndexStartCheckFx].rectTransform, _moveSliderAnimConfig.audioSource);
                                    imgIndexStartCheckFx += 1;
                                    fillAmountOnImgStartCheckFx = 0f;
                                }
                            }
                        }
                    }
                    else // down
                    {
                        if (imgIndexStartCheckFx >= 0)
                        {
                            if (_currentVisualValue < imgIndexStartCheckFx)
                            {
                                if (fillAmountOnImgStartCheckFx >= 1f - Mathf.Epsilon && (imgIndexStartCheckFx + 1 - _currentValue >= 1f - Mathf.Epsilon))
                                {
                                    _moveSliderAnimConfig.fxLoseFull.Play(_imgFillSequence[imgIndexStartCheckFx].rectTransform, _moveSliderAnimConfig.audioSource);
                                    imgIndexStartCheckFx -= 1;
                                    fillAmountOnImgStartCheckFx = 1f;
                                }
                                else
                                {
                                    _moveSliderAnimConfig.fxLose.Play(_imgFillSequence[imgIndexStartCheckFx].rectTransform, _moveSliderAnimConfig.audioSource);
                                    imgIndexStartCheckFx -= 1;
                                    fillAmountOnImgStartCheckFx = 1f;
                                }
                            }
                        }
                    }
                    yield return null;
                }

                _currentVisualValue = _currentValue;
                UpdateFill();

                _coroutineUpdateVisual = null;
            }
        }

        private void UpdateFill()
        {
            int floor = (int)_currentVisualValue;
            for (int i = 0; i < _imgFillSequence.Length; i++)
            {
                if (i < floor)
                {
                    _imgFillSequence[i].fillAmount = 1f;
                }
                else if (i == floor)
                {
                    _imgFillSequence[i].fillAmount = _currentVisualValue - floor;
                }
                else
                {
                    _imgFillSequence[i].fillAmount = 0;
                }
            }
        }

#if UNITY_EDITOR
        [Header("Editor Tool")]
        [SerializeField] private bool _isUpdateOnValidate = false;

        private void OnValidate()
        {
            if (_isUpdateOnValidate)
            {
                UpdateFillEditor();
            }
        }

        [Button]
        private void UpdateFillEditor()
        {
            int group = UnityEditor.Undo.GetCurrentGroup();
            _currentValue = Mathf.Clamp(_currentValue, 0f, MaxValue);
            _currentVisualValue = _currentValue;
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.Undo.RecordObjects(_imgFillSequence, "UpdateRenderers");
            UpdateFill();
            UnityEditor.Undo.CollapseUndoOperations(group);
        }
#endif
    }
}