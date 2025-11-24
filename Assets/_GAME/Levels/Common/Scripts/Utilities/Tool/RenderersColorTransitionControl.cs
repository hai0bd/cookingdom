using System.Collections;
using UnityEngine;

namespace Utilities
{
    public class RenderersColorTransitionControl : MonoBehaviour
    {
        [SerializeField, Range(0f,1f)] private float _progress = 0f;
        public float Progress
        {
            get => _progress;
            set
            {
                _progress = Mathf.Clamp01(value);
                UpdateAllColors();
            }
        }

        public abstract class ColorTransitionConfig<T> where T:Component
        {
            [SerializeField] protected T[] renderers;
            [SerializeField] private Gradient gradient;

            public void UpdateColor(float progress)
            {
                SetColor(gradient.Evaluate(progress));
            }

            protected abstract void SetColor(Color color);

#if UNITY_EDITOR
            public void SetDirty()
            {
                foreach (var renderer in renderers)
                {
                    UnityEditor.EditorUtility.SetDirty(renderer);
                }
            }
#endif
        }

        [System.Serializable]
        public class ColorTransitionConfigSpriteRenderer : ColorTransitionConfig<SpriteRenderer>
        {
            protected override void SetColor(Color color)
            {
                foreach (var renderer in renderers)
                {
                    renderer.color = color;
                }
            }
        }

        [System.Serializable]
        public class ColorTransitionConfigImageUI : ColorTransitionConfig<UnityEngine.UI.Image>
        {
            protected override void SetColor(Color color)
            {
                foreach (var renderer in renderers)
                {
                    renderer.color = color;
                }
            }
        }

        [System.Serializable]
        public class ColorTransitionConfigCamera : ColorTransitionConfig<Camera>
        {
            protected override void SetColor(Color color)
            {
                foreach (var renderer in renderers)
                {
                    renderer.backgroundColor = color;
                }
            }
        }

        [Sirenix.OdinInspector.DrawWithUnity]
        [SerializeField] private ColorTransitionConfigSpriteRenderer[] _spriteRendererConfig;
        [Sirenix.OdinInspector.DrawWithUnity]
        [SerializeField] private ColorTransitionConfigImageUI[] _imageUIConfig;
        [Sirenix.OdinInspector.DrawWithUnity]
        [SerializeField] private ColorTransitionConfigCamera _cameraConfig;

        private void UpdateAllColors()
        {
            foreach (var config in _spriteRendererConfig)
            {
                config.UpdateColor(_progress);
            }
            foreach (var config in _imageUIConfig)
            {
                config.UpdateColor(_progress);
            }
            _cameraConfig.UpdateColor(_progress);
        }

        private void OnEnable()
        {
            UpdateAllColors();
        }

#if UNITY_EDITOR
        [Header("Editor")]
        [SerializeField] private bool _isAutoUpdateAllColorOnValidate = false;
        private void OnValidate()
        {
            if (_isAutoUpdateAllColorOnValidate)
            {
                UpdateAllColors();
            }
        }

        [Sirenix.OdinInspector.Button("Update All Color Editor")]
        private void UpdateAllColorEditor()
        {
            UpdateAllColors();

            foreach (var config in _spriteRendererConfig)
            {
                config.SetDirty();
            }
            foreach (var config in _imageUIConfig)
            {
                config.SetDirty();
            }
            _cameraConfig.SetDirty();
        }

        public void SetProgressEditor(float value)
        {
            _progress = value;
            UpdateAllColorEditor();
        }
#endif
    }
}