using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TidyCooking.Levels
{
    public class SpriteMultiplePhase : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer[] _renderers;
        [SerializeField] private SpriteMultiplePhase[] _childs;
        [SerializeField] private float _currentProgress = 0f;

        public void SetChangeProgress(float progress)
        {
            // on off and fade in out use coroutine
            _currentProgress = progress;

            if (_renderers != null)
            {
                progress = Mathf.Clamp(progress, 0, _renderers.Length);
                int startPhaseIndex = Mathf.FloorToInt(progress) - 1;
                int endPhaseIndex = Mathf.CeilToInt(progress) - 1;
                float phaseProgress = progress - startPhaseIndex;

                for (int i = 0; i < _renderers.Length; i++)
                {
                    if (_renderers[i] == null) continue;
                    if (i == startPhaseIndex)
                    {
                        _renderers[i].enabled = true;
                        Color color = _renderers[i].color;
                        color.a = 1f - phaseProgress;
                        _renderers[i].color = color;
                    }
                    else if (i == endPhaseIndex)
                    {
                        _renderers[i].enabled = true;
                        Color color = _renderers[i].color;
                        color.a = phaseProgress;
                        _renderers[i].color = color;
                    }
                    else
                    {
                        _renderers[i].enabled = false;
                    }
                }
            }

            if (_childs != null)
            {
                foreach (var child in _childs)
                {
                    child.SetChangeProgress(progress);
                }
            }
        }

#if UNITY_EDITOR
        [Header("Editor")]
        [SerializeField] private bool _isAutoUpdateOnValidate = false;

        private void OnValidate()
        {
            if (_isAutoUpdateOnValidate)
            {
                UpdateVisualEditor();
            }
        }

        [Button]
        private void UpdateVisualEditor()
        {
            if (_renderers != null)
            {
                int group = UnityEditor.Undo.GetCurrentGroup();
                UnityEditor.Undo.RecordObjects(_renderers, "UpdateRenderers");

                float progress = Mathf.Clamp(_currentProgress, 0, _renderers.Length);

                int startPhaseIndex = Mathf.FloorToInt(progress) - 1;
                int endPhaseIndex = Mathf.CeilToInt(progress) - 1;
                float phaseProgress = progress - (startPhaseIndex + 1);

                for (int i = 0; i < _renderers.Length; i++)
                {
                    if (_renderers[i] == null) continue;
                    if (i == startPhaseIndex)
                    {
                        _renderers[i].enabled = true;
                        Color color = _renderers[i].color;
                        color.a = 1f - phaseProgress;
                        _renderers[i].color = color;
                    }
                    else if (i == endPhaseIndex)
                    {
                        _renderers[i].enabled = true;
                        Color color = _renderers[i].color;
                        color.a = phaseProgress;
                        _renderers[i].color = color;
                    }
                    else
                    {
                        _renderers[i].enabled = false;
                    }
                }

                UnityEditor.Undo.CollapseUndoOperations(group);
            }

            if (_childs != null)
            {
                foreach (var child in _childs)
                {
                    child._currentProgress = _currentProgress;
                    child.UpdateVisualEditor();
                }
            }
        }
#endif
    }
}