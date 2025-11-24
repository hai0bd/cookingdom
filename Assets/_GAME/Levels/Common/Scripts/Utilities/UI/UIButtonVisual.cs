using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
// using GoogleMobileAds.Api;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Utilities
{
    public static class UIButtonVisualExtension
    {
        public static void SetInteractable(this Selectable selectable, bool isInteractable)
        {
            if (selectable == null) return;
            selectable.interactable = isInteractable;
            if (selectable.TryGetComponent<UIButtonVisual>(out var buttonVisual))
            {
                buttonVisual.UpdateVisual();
            }
        }
    }

    [RequireComponent(typeof(Selectable))]
    public class UIButtonVisual : MonoBehaviour
    {
        [System.Serializable]
        public class GraphicConfig
        {
            private const float Duration = 0.2f;

            public Graphic image;
            public Color colorNormal = Color.white;
            public Color colorDisabled = Color.gray;

            private Tween _tween;

            public void Apply(bool isInteractable)
            {
                _tween.Kill();
                Color targetColor = isInteractable ? colorNormal : colorDisabled;
                // _tween = image.DOColor(targetColor, Duration).SetEase(Ease.OutCubic);
            }

            public void ApplyImmediate(bool isInteractable)
            {
                _tween.Kill();
                image.color = isInteractable ? colorNormal : colorDisabled;
            }
        }

        [System.Serializable]
        public class TextConfig
        {
            private const float Duration = 0.2f;

            public Text text;
            public string txtNormal;
            public string txtDisabled;

            private Tween _tween;

            public void Apply(bool isInteractable)
            {
                _tween.Kill();
                string targetText = isInteractable ? txtNormal : txtDisabled;
                // _tween = text.DOText(targetText, Duration, false, ScrambleMode.None).SetUpdate(true).SetEase(Ease.OutSine).Play();
            }

            public void ApplyImmediate(bool isInteractable, params string[] strs)
            {
                _tween.Kill();
                text.text = isInteractable ? string.Format(txtNormal, strs) : string.Format(txtDisabled, strs);
            }
        }

        [SerializeField] private Selectable _component;
        [SerializeField] private GraphicConfig[] _graphicConfigs;
        [SerializeField] private TextConfig[] _textConfigs;
        public float transitionDuration = 0.2f;

        private bool CheckIsInteractable()
        {
            if (_component)
            {
                return _component.interactable;
            }
            else if (TryGetComponent<Selectable>(out var component))
            {
                return component.interactable;
            }
            else
            {
                return false;
            }
        }

        public void UpdateVisual()
        {
            bool isInteractable = CheckIsInteractable();
            if (_graphicConfigs != null) foreach (var config in _graphicConfigs) config.Apply(isInteractable);
            if (_textConfigs != null) foreach (var config in _textConfigs) config.Apply(isInteractable);
        }

        public void UpdateVisualImmediate()
        {
            bool isInteractable = CheckIsInteractable();
            if (_graphicConfigs != null) foreach (var config in _graphicConfigs) config.ApplyImmediate(isInteractable);
            if (_textConfigs != null) foreach (var config in _textConfigs) config.ApplyImmediate(isInteractable);
        }

#if UNITY_EDITOR
        [Button]
        private void UpdateVisualEditor()
        {
            int group = UnityEditor.Undo.GetCurrentGroup();

            List<Object> objs = new();
            if (_graphicConfigs != null)
            {
                foreach (var config in _graphicConfigs)
                {
                    if (config.image) objs.Add(config.image);
                }
            }
            if (_textConfigs != null)
            {
                foreach (var config in _textConfigs)
                {
                    if (config.text) objs.Add(config.text);
                }
            }

            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.Undo.RecordObjects(objs.ToArray(), "UpdateButtonVisual");
            UpdateVisualImmediate();
            UnityEditor.Undo.CollapseUndoOperations(group);
            foreach (var obj in objs)
            {
                UnityEditor.EditorUtility.SetDirty(obj);
            }
        }
#endif
    }
}