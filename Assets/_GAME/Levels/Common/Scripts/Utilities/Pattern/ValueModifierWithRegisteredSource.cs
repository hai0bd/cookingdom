using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    /// <summary>
    /// Call getter Value return true if there any modifier inside, otherwise return false
    /// </summary>
    [System.Serializable]
    public class BoolModifierWithRegisteredSource
    {
        private List<int> _sources = new List<int>();
#if UNITY_EDITOR
        private List<Object> _sourceObjs = new List<Object>();
#endif
        private bool _value;
        public bool Value => _value;
        private System.Action _onChanged;

        public BoolModifierWithRegisteredSource() { }
        public BoolModifierWithRegisteredSource(System.Action onChanged)
        {
            _onChanged = onChanged;
        }

        public void AddModifier(Object @object)
        {
            AddModifier(@object.GetInstanceID());
#if UNITY_EDITOR
            if (!_sourceObjs.Contains(@object))
            {
                _sourceObjs.Add(@object);
            }
#endif
        }
        public void AddModifier(int id)
        {
            if (!_sources.Contains(id))
            {
                _sources.Add(id);
                _value = _sources.Count > 0;
                _onChanged?.Invoke();
            }
        }

        public void RemoveModifier(Object @object)
        {
            RemoveModifier(@object.GetInstanceID());
#if UNITY_EDITOR
            _sourceObjs.Remove(@object);
#endif
        }
        public void RemoveModifier(int id)
        {
            _sources.Remove(id);
            _value = _sources.Count > 0;
            _onChanged?.Invoke();
        }
        public void ClearAll()
        {
            _sources.Clear();
#if UNITY_EDITOR
            _sourceObjs.Clear();
#endif
            _value = false;
            _onChanged?.Invoke();
        }
    }

    [System.Serializable]
    public class Vector2AddModifierWithRegisteredSource
    {
        protected Dictionary<int, Vector2> _source = new Dictionary<int, Vector2>();
        protected Vector2 _value = Vector2.zero;
        public Vector2 Value => _value;
        private System.Action _onChanged;

        public Vector2AddModifierWithRegisteredSource()
        {
        }
        public Vector2AddModifierWithRegisteredSource(System.Action onChanged)
        {
            _onChanged = onChanged;
        }

        public void AddModifier(Object @object, Vector2 value) => AddModifier(@object.GetInstanceID(), value);
        public void AddModifier(int id, Vector2 value)
        {
            if (_source.ContainsKey(id))
            {
                _source[id] = value;
            }
            else
            {
                _source.Add(id, value);
            }
            UpdateModifier();
        }
        public void RemoveModifier(Object @object) => RemoveModifier(@object.GetInstanceID());
        public void RemoveModifier(int id)
        {
            if (_source.Remove(id))
            {
                UpdateModifier();
            }
        }

        private void UpdateModifier()
        {
            _value = Vector2.zero;
            foreach (var value in _source.Values)
            {
                _value += value;
            }
            _onChanged?.Invoke();
        }
    }

    public enum ColorBlendOperation
    {
        Add,
        Mul,
        Min,
        Max,
    }
    public enum ColorBlendFactor
    {
        Zero,
        One,
        SrcAlpha,
        DstAlpha,
        OneMinusSrcAlpha,
        OneMinusDstAlpha
    }

    [System.Serializable]
    public class ColorBlendModifierWithRegisteredSource
    {
        private Color _originColor;
        private ColorBlendOperation _blendOp;
        private ColorBlendOperation _alphaBlendOp;
        private ColorBlendFactor _srcBlendFactor;
        private ColorBlendFactor _dstBlendFactor;
        private ColorBlendFactor _srcAlphaBlendFactor;
        private ColorBlendFactor _dstAlphaBlendFactor;
        protected Dictionary<int, Color> _source = new Dictionary<int, Color>();
        protected Color _value = Color.clear;
        public Color Value => _value;
        private System.Action _onChanged;

        public delegate Color CustomBlendFunc(Color src, Color dst);
        private CustomBlendFunc _customBlendFunc;

        public ColorBlendModifierWithRegisteredSource(Color originColor, ColorBlendOperation blendOp, ColorBlendFactor srcBlendFactor, ColorBlendFactor dstBlendFactor, System.Action onChanged = null)
        {
            _blendOp = blendOp;
            _alphaBlendOp = blendOp;
            _originColor = originColor;
            _srcBlendFactor = srcBlendFactor;
            _srcAlphaBlendFactor = srcBlendFactor;
            _dstBlendFactor = dstBlendFactor;
            _dstAlphaBlendFactor = dstBlendFactor;
            _onChanged = onChanged;
        }
        public ColorBlendModifierWithRegisteredSource(Color originColor, CustomBlendFunc customBlendFunc, System.Action onChanged = null)
        {
            _customBlendFunc = customBlendFunc;
            _originColor = originColor;
            _onChanged = onChanged;
        }
        public ColorBlendModifierWithRegisteredSource(Color originColor, ColorBlendOperation blendOp, ColorBlendOperation alphaBlendOp, ColorBlendFactor srcBlendFactor, ColorBlendFactor dstBlendFactor, ColorBlendFactor srcAlphaBlendFactor, ColorBlendFactor dstAlphaBlendFactor, System.Action onChanged = null)
        {
            _blendOp = blendOp;
            _alphaBlendOp = alphaBlendOp;
            _originColor = originColor;
            _srcBlendFactor = srcBlendFactor;
            _srcAlphaBlendFactor = srcAlphaBlendFactor;
            _dstBlendFactor = dstBlendFactor;
            _dstAlphaBlendFactor = dstAlphaBlendFactor;
            _onChanged = onChanged;
        }

        public void AddModifier(Object @object, Color value) => AddModifier(@object.GetInstanceID(), value);
        public void AddModifier(int id, Color value)
        {
            if (_source.ContainsKey(id))
            {
                _source[id] = value;
            }
            else
            {
                _source.Add(id, value);
            }
            UpdateModifier();
        }
        public void RemoveModifier(Object @object) => RemoveModifier(@object.GetInstanceID());
        public void RemoveModifier(int id)
        {
            if (_source.Remove(id))
            {
                UpdateModifier();
            }
        }
        public void RemoveAllModifier()
        {
            _source.Clear();
            UpdateModifier();
        }

        private void UpdateModifier()
        {
            _value = _originColor;
            float srcFactor, dstFactor, srcAlphaFactor, dstAlphaFactor;

            foreach (var dstValue in _source.Values)
            {
                if (_customBlendFunc != null)
                {
                    _value = _customBlendFunc(_value, dstValue);
                    continue;
                }

                float CalculateFactor(ColorBlendFactor colorBlendFactor)
                    => colorBlendFactor switch
                    {
                        ColorBlendFactor.Zero => 0f,
                        ColorBlendFactor.One => 1f,
                        ColorBlendFactor.SrcAlpha => _value.a,
                        ColorBlendFactor.DstAlpha => dstValue.a,
                        ColorBlendFactor.OneMinusSrcAlpha => 1f - _value.a,
                        ColorBlendFactor.OneMinusDstAlpha => 1f - dstValue.a,
                        _ => 0f,
                    };
                srcFactor = CalculateFactor(_srcBlendFactor);
                dstFactor = CalculateFactor(_dstBlendFactor);
                srcAlphaFactor = CalculateFactor(_srcAlphaBlendFactor);
                dstAlphaFactor = CalculateFactor(_dstAlphaBlendFactor);

                switch (_blendOp)
                {
                    case ColorBlendOperation.Add:
                        _value = new Color(
                            _value.r * srcFactor + dstValue.r * dstFactor,
                            _value.g * srcFactor + dstValue.g * dstFactor,
                            _value.b * srcFactor + dstValue.b * dstFactor,
                            _value.a);
                        break;
                    case ColorBlendOperation.Mul:
                        _value = new Color(
                            _value.r * srcFactor * dstValue.r * dstFactor,
                            _value.g * srcFactor * dstValue.g * dstFactor,
                            _value.b * srcFactor * dstValue.b * dstFactor,
                            _value.a);
                        break;
                    case ColorBlendOperation.Min:
                        _value = new Color(
                            Mathf.Min(_value.r * srcFactor, dstValue.r * dstFactor),
                            Mathf.Min(_value.g * srcFactor, dstValue.g * dstFactor),
                            Mathf.Min(_value.b * srcFactor, dstValue.b * dstFactor),
                            _value.a);
                        break;
                    case ColorBlendOperation.Max:
                        _value = new Color(
                            Mathf.Max(_value.r * srcFactor, dstValue.r * dstFactor),
                            Mathf.Max(_value.g * srcFactor, dstValue.g * dstFactor),
                            Mathf.Max(_value.b * srcFactor, dstValue.b * dstFactor),
                            _value.a);
                        break;
                }

                switch (_alphaBlendOp)
                {
                    case ColorBlendOperation.Add:
                        _value.a = _value.a * srcAlphaFactor + dstValue.a * dstAlphaFactor;
                        break;
                    case ColorBlendOperation.Mul:
                        _value.a = _value.a * srcAlphaFactor * dstValue.a * dstAlphaFactor;
                        break;
                    case ColorBlendOperation.Min:
                        _value.a = Mathf.Min(_value.a * srcAlphaFactor, dstValue.a * dstAlphaFactor);
                        break;
                    case ColorBlendOperation.Max:
                        _value.a = Mathf.Max(_value.a * srcAlphaFactor, dstValue.a * dstAlphaFactor);
                        break;
                }
            }
            _onChanged?.Invoke();
        }

        public static ColorBlendModifierWithRegisteredSource Additive(System.Action onChanged = null)
            => new ColorBlendModifierWithRegisteredSource(Color.clear, ColorBlendOperation.Add, ColorBlendFactor.One, ColorBlendFactor.One, onChanged);
        public static ColorBlendModifierWithRegisteredSource AlphaBlend(System.Action onChanged = null)
            => new ColorBlendModifierWithRegisteredSource(Color.clear, ColorBlendOperation.Add, ColorBlendFactor.SrcAlpha, ColorBlendFactor.DstAlpha, onChanged);
        public static ColorBlendModifierWithRegisteredSource Multiply(System.Action onChanged = null)
            => new ColorBlendModifierWithRegisteredSource(Color.white, ColorBlendOperation.Mul, ColorBlendFactor.One, ColorBlendFactor.One, onChanged);

        public IEnumerator IETween(UnityEngine.Object @object, Color toColor, float duration, AnimationCurve ease, bool isUseUnscaledTime = false, System.Action onFinished = null)
        {
            Color fromColor;

            int id = @object.GetInstanceID();
            if (!_source.ContainsKey(id))
            {
                fromColor = Value;
                _source.Add(id, fromColor);
            }
            else
            {
                fromColor = _source[id];
            }

            float timeStart = isUseUnscaledTime ? Time.unscaledTime : Time.time;
            float progress = 0f;
            while (progress < 1f)
            {
                progress = ((isUseUnscaledTime ? Time.unscaledTime : Time.time) - timeStart) / duration;
                _source[id] = Color.Lerp(fromColor, toColor, ease.Evaluate(progress));
                UpdateModifier();
                yield return null;
            }

            _source[id] = toColor;

            onFinished?.Invoke();
        }

        public IEnumerator IETween(UnityEngine.Object @object, Color toColor, float duration, bool isUseUnscaledTime = false, System.Action onFinished = null)
        {
            Color fromColor;

            int id = @object.GetInstanceID();
            if (!_source.ContainsKey(id))
            {
                fromColor = Value;
                _source.Add(id, fromColor);
            }
            else
            {
                fromColor = _source[id];
            }

            float timeStart = isUseUnscaledTime ? Time.unscaledTime : Time.time;
            float progress = 0f;
            while (progress < 1f)
            {
                progress = ((isUseUnscaledTime ? Time.unscaledTime : Time.time) - timeStart) / duration;
                _source[id] = Color.Lerp(fromColor, toColor, progress);
                UpdateModifier();
                yield return null;
            }

            _source[id] = toColor;

            onFinished?.Invoke();
        }

        public IEnumerator IETween(UnityEngine.Object @object, Color fromColor, Color toColor, float duration, AnimationCurve ease, bool isUseUnscaledTime = false, System.Action onFinished = null)
        {
            int id = @object.GetInstanceID();
            if (!_source.ContainsKey(id))
            {
                _source.Add(id, fromColor);
            }

            float timeStart = isUseUnscaledTime ? Time.unscaledTime : Time.time;
            float progress = 0f;
            while (progress < 1f)
            {
                progress = ((isUseUnscaledTime ? Time.unscaledTime : Time.time) - timeStart) / duration;
                _source[id] = Color.Lerp(fromColor, toColor, ease.Evaluate(progress));
                UpdateModifier();
                yield return null;
            }

            _source[id] = toColor;

            onFinished?.Invoke();
        }

        public IEnumerator IETween(UnityEngine.Object @object, Color fromColor, Color toColor, float duration, bool isUseUnscaledTime = false, System.Action onFinished = null)
        {
            int id = @object.GetInstanceID();
            if (!_source.ContainsKey(id))
            {
                _source.Add(id, fromColor);
            }

            float timeStart = isUseUnscaledTime ? Time.unscaledTime : Time.time;
            float progress = 0f;
            while (progress < 1f)
            {
                progress = ((isUseUnscaledTime ? Time.unscaledTime : Time.time) - timeStart) / duration;
                _source[id] = Color.Lerp(fromColor, toColor, progress);
                UpdateModifier();
                yield return null;
            }

            _source[id] = toColor;

            onFinished?.Invoke();
        }
    }

    public abstract class FloatModifierWithRegisteredSource
    {
        protected Dictionary<int, float> _source = new Dictionary<int, float>();
        protected float _value;
        public float Value => _value;
        private System.Action _onChanged;

#if UNITY_EDITOR
        private Dictionary<Object, float> _sourceObj = new Dictionary<Object, float>();

        [Sirenix.OdinInspector.OnInspectorGUI]
        private void OnInspectorGUI()
        {
            if (_sourceObj != null)
            {
                foreach (var pair in _sourceObj)
                {
                    UnityEditor.EditorGUILayout.LabelField(pair.Key.name + ": " + pair.Value);
                }

                foreach (var pair in _source)
                {
                    UnityEditor.EditorGUILayout.LabelField("ID " + pair.Key + ": " + pair.Value);
                }
            }
        }
#endif

        protected FloatModifierWithRegisteredSource()
        {
            _value = InitValue;
        }
        public FloatModifierWithRegisteredSource(System.Action onChanged)
        {
            _onChanged = onChanged;
        }

        public void AddModifier(Object @object, float value)
        {
#if UNITY_EDITOR
            if (_sourceObj.ContainsKey(@object))
            {
                _sourceObj[@object] = value;
            }
            else
            {
                _sourceObj.Add(@object, value);
            }
#endif

            AddModifier(@object.GetInstanceID(), value);
        }
        public void AddModifier(int id, float value)
        {
            if (_source.ContainsKey(id))
            {
                _source[id] = value;
            }
            else
            {
                _source.Add(id, value);
            }
            UpdateModifier();
            _onChanged?.Invoke();
        }
        public void RemoveModifier(Object @object)
        {
#if UNITY_EDITOR
            _sourceObj.Remove(@object);
#endif
            RemoveModifier(@object.GetInstanceID());
        }
        public void RemoveModifier(int id)
        {
            if (_source.Remove(id))
            {
                UpdateModifier();
                _onChanged?.Invoke();
            }
        }

        public IEnumerator IETween(UnityEngine.Object @object, float to, AnimationCurve valueCurve, float duration, System.Action onFinished = null, System.Action<float> onUpdate = null)
        {
            int id = @object.GetInstanceID();
            float from;
            if (_source.ContainsKey(id))
            {
                from = _source[id];
            }
            else
            {
                from = 1f;
            }
            return IETween(@object, from, to, valueCurve, duration, onFinished, onUpdate);
        }

        public IEnumerator IETween(UnityEngine.Object @object, float from, float to, AnimationCurve valueCurve, float duration, System.Action onFinished = null, System.Action<float> onUpdate = null)
        {
            int id = @object.GetInstanceID();
            if (!_source.ContainsKey(id))
            {
                _source.Add(id, Mathf.LerpUnclamped(from, to, valueCurve.Evaluate(0f)));

#if UNITY_EDITOR
                _sourceObj.Add(@object, Mathf.LerpUnclamped(from, to, valueCurve.Evaluate(0f)));
#endif
            }

            float timeStart = Time.time;
            float progress = 0f;
            float value;
            while (progress < 1f)
            {
                progress = (Time.time - timeStart) / duration;
                value = Mathf.LerpUnclamped(from, to, valueCurve.Evaluate(progress));
                _source[id] = value;

#if UNITY_EDITOR
                _sourceObj[@object] = value;
#endif
                onUpdate?.Invoke(value);
                UpdateModifier();
                yield return null;
            }

            onFinished?.Invoke();
        }

        public IEnumerator IETween(UnityEngine.Object @object, AnimationCurve valueCurve, float duration, System.Action onFinished = null, System.Action<float> onUpdate = null)
        {
            int id = @object.GetInstanceID();
            if (!_source.ContainsKey(id))
            {
                _source.Add(id, valueCurve.Evaluate(0f));
#if UNITY_EDITOR
                _sourceObj.Add(@object, valueCurve.Evaluate(0f));
#endif
            }

            float timeStart = Time.time;
            float progress = 0f;
            float value;
            while (progress < 1f)
            {
                progress = (Time.time - timeStart) / duration;
                value = valueCurve.Evaluate(progress);
                _source[id] = value;
#if UNITY_EDITOR
                _sourceObj[@object] = value;
#endif
                onUpdate?.Invoke(value);
                UpdateModifier();
                yield return null;
            }

            onFinished?.Invoke();
        }

        protected abstract float InitValue { get; }
        protected abstract void UpdateModifier();
    }

    [System.Serializable]
    public class FloatMulModifierWithRegisteredSource : FloatModifierWithRegisteredSource
    {
        public FloatMulModifierWithRegisteredSource() : base() { _value = 1f; }
        public FloatMulModifierWithRegisteredSource(System.Action onChanged) : base(onChanged) { _value = 1f; }

        protected override float InitValue => 1f;
        protected override void UpdateModifier()
        {
            _value = 1f;
            foreach (var value in _source.Values)
            {
                _value *= value;
            }
        }
    }

    [System.Serializable]
    public class FloatAddModifierWithRegisteredSource : FloatModifierWithRegisteredSource
    {
        public FloatAddModifierWithRegisteredSource() : base() { }
        public FloatAddModifierWithRegisteredSource(System.Action onChanged) : base(onChanged) { }

        protected override float InitValue => 0f;
        protected override void UpdateModifier()
        {
            _value = 0f;
            foreach (var value in _source.Values)
            {
                _value += value;
            }
        }
    }

    [System.Serializable]
    public class FloatMinModifierWithRegisteredSource : FloatModifierWithRegisteredSource
    {
        private float _init;
        protected override float InitValue => _init;
        protected override void UpdateModifier()
        {
            _value = _init;
            foreach (var value in _source.Values)
            {
                if (value < _value) _value = value;
            }
        }

        public FloatMinModifierWithRegisteredSource(float init = float.MaxValue, System.Action onChanged = null) : base(onChanged)
        {
            _value = _init = init;
        }
    }

    [System.Serializable]
    public class FloatMaxModifierWithRegisteredSource : FloatModifierWithRegisteredSource
    {
        private float _init;
        protected override float InitValue => _init;
        protected override void UpdateModifier()
        {
            _value = _init;
            foreach (var value in _source.Values)
            {
                if (value > _value) _value = value;
            }
        }

        public FloatMaxModifierWithRegisteredSource(float init = float.MinValue, System.Action onChanged = null) : base(onChanged)
        {
            _value = _init = init;
        }
    }

    [System.Serializable]
    public class ComponentOverrideWithRegisteredSource<T> where T : Component
    {
        [System.Serializable]
        public struct Data
        {
            public T component;
            public float priority;
        }

        protected Dictionary<int, Data> _source = new Dictionary<int, Data>();

        private T _componentHighestPriority = null;
        public T ValueHighestPriority => _componentHighestPriority;
        private System.Action _onChangedValueHighestPriority;

        public ComponentOverrideWithRegisteredSource() { }
        public ComponentOverrideWithRegisteredSource(System.Action onChanged)
        {
            _onChangedValueHighestPriority = onChanged;
        }

        public void AddModifier(Object @object, T component, float priority) => AddModifier(@object.GetInstanceID(), component, priority);
        public void AddModifier(int id, T component, float priority)
        {
            if (_source.ContainsKey(id))
            {
                _source[id] = new Data { component = component, priority = priority };
            }
            else
            {
                _source.Add(id, new Data { component = component, priority = priority });
            }
            UpdateModifier();
        }

        public void RemoveModifier(Object @object) => RemoveModifier(@object.GetInstanceID());
        public void RemoveModifier(int id)
        {
            if (_source.Remove(id))
            {
                UpdateModifier();
            }
        }

        private void UpdateModifier()
        {
            T componentMaxPriority = null;
            float maxPriority = float.MinValue;
            foreach (var pair in _source)
            {
                if (pair.Value.priority > maxPriority)
                {
                    maxPriority = pair.Value.priority;
                    componentMaxPriority = pair.Value.component;
                }
            }
            if (_componentHighestPriority != componentMaxPriority)
            {
                _componentHighestPriority = componentMaxPriority;
                _onChangedValueHighestPriority?.Invoke();
            }
        }
    }

    [System.Serializable]
    public class ModifierFloatCountdown
    {
        private MonoBehaviour _context;
        private FloatModifierWithRegisteredSource _modifier;
        private Dictionary<int, Coroutine> _corCountdowns = new Dictionary<int, Coroutine>();

        public ModifierFloatCountdown(MonoBehaviour context, FloatModifierWithRegisteredSource modifier)
        {
            _context = context;
            _modifier = modifier;
        }

        public void Add(int source, float value, float duration)
        {
            _modifier.AddModifier(source, value);
            if (_corCountdowns.TryGetValue(source, out Coroutine cor))
            {
                _context.StopCoroutine(cor);
                _corCountdowns[source] = _context.StartCoroutine(IECountdown(source, duration));
            }
            else
            {
                _corCountdowns.Add(source, _context.StartCoroutine(IECountdown(source, duration)));
            }
        }

        private IEnumerator IECountdown(int source, float duration)
        {
            float timeEnd = Time.time + duration;
            while (Time.time < timeEnd) yield return null;
            _corCountdowns.Remove(source);
            _modifier.RemoveModifier(source);
        }
    }

    [System.Serializable]
    public class ModifierBoolCountdown
    {
        private MonoBehaviour _context;
        private BoolModifierWithRegisteredSource _modifier;
        private Dictionary<int, Coroutine> _corCountdowns = new Dictionary<int, Coroutine>();

        public ModifierBoolCountdown(MonoBehaviour context, BoolModifierWithRegisteredSource modifier)
        {
            _context = context;
            _modifier = modifier;
        }

        public void Add(int source, float duration)
        {
            _modifier.AddModifier(source);
            if (_corCountdowns.TryGetValue(source, out Coroutine cor))
            {
                _context.StopCoroutine(cor);
                _corCountdowns[source] = _context.StartCoroutine(IECountdown(source, duration));
            }
            else
            {
                _corCountdowns.Add(source, _context.StartCoroutine(IECountdown(source, duration)));
            }
        }

        private IEnumerator IECountdown(int source, float duration)
        {
            float timeEnd = Time.time + duration;
            while (Time.time < timeEnd) yield return null;
            _corCountdowns.Remove(source);
            _modifier.RemoveModifier(source);
        }
    }
}