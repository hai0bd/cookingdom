using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Utilities
{
    [AddComponentMenu("Layout/Scale Fitter", 142)]
    [ExecuteInEditMode]
    [RequireComponent(typeof(RectTransform))]
    public class ScaleFitterUI : UIBehaviour, ILayoutSelfController
    {
        public enum ScaleMode { None, AlwaysFitHeight, AlwaysFitWidth, FitInParent, EnvelopeParent }

        [SerializeField] private ScaleMode m_AspectMode = ScaleMode.None;
        public ScaleMode aspectMode { get { return m_AspectMode; } set { if (SetStruct(ref m_AspectMode, value)) SetDirty(); } }

        [System.NonSerialized]
        private RectTransform m_Rect;

        private RectTransform rectTransform
        {
            get
            {
                if (m_Rect == null)
                    m_Rect = GetComponent<RectTransform>();
                return m_Rect;
            }
        }

        private DrivenRectTransformTracker m_Tracker;

        protected ScaleFitterUI() { }

        #region Unity Lifetime calls

        protected override void OnEnable()
        {
            base.OnEnable();
            SetDirty();
        }

        protected override void OnDisable()
        {
            m_Tracker.Clear();
            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
            base.OnDisable();
        }

        #endregion

        protected override void OnRectTransformDimensionsChange()
        {
            UpdateRect();
        }

        private void UpdateRect()
        {
            m_Tracker.Clear();

            Vector2 parentSize = GetParentSize();

            switch (m_AspectMode)
            {
#if UNITY_EDITOR
                case ScaleMode.None:
                {
                    break;
                }
#endif
                case ScaleMode.AlwaysFitWidth:
                    ScaleMatchWidth();
                    break;
                case ScaleMode.AlwaysFitHeight:
                    ScaleMatchHeight();
                    break;
                case ScaleMode.FitInParent:
                    if (rectTransform.rect.width / rectTransform.rect.height > parentSize.x / parentSize.y)
                    {
                        ScaleMatchWidth();
                    }
                    else
                    {
                        ScaleMatchHeight();
                    }
                    break;
                case ScaleMode.EnvelopeParent:
                    if (rectTransform.rect.width / rectTransform.rect.height > parentSize.x / parentSize.y)
                    {
                        ScaleMatchHeight();
                    }
                    else
                    {
                        ScaleMatchWidth();
                    }
                    break;
            }

            void ScaleMatchWidth()
            {
                rectTransform.anchorMin = rectTransform.anchorMax = new Vector2(rectTransform.pivot.x, 0.5f);
                rectTransform.anchoredPosition = Vector2.zero;
                float scaleMultiplier = GetParentSize().x / rectTransform.rect.width;
                rectTransform.localScale = new Vector3(scaleMultiplier, scaleMultiplier, 1);
            }

            void ScaleMatchHeight()
            {
                rectTransform.anchorMin = rectTransform.anchorMax = new Vector2(0.5f, rectTransform.pivot.y);
                rectTransform.anchoredPosition = Vector2.zero;
                float scaleMultiplier = GetParentSize().y / rectTransform.rect.height;
                rectTransform.localScale = new Vector3(scaleMultiplier, scaleMultiplier, 1);
            }
        }

        private Vector2 GetParentSize()
        {
            RectTransform parent = rectTransform.parent as RectTransform;
            if (!parent)
                return Vector2.zero;
            return parent.rect.size;
        }

        public virtual void SetLayoutHorizontal() { }

        public virtual void SetLayoutVertical() { }

        protected void SetDirty()
        {
            if (!IsActive())
                return;

            UpdateRect();
        }

        private static bool SetStruct<T>(ref T currentValue, T newValue) where T : struct
        {
            if (currentValue.Equals(newValue))
                return false;

            currentValue = newValue;
            return true;
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            SetDirty();
        }
#endif
    }
}