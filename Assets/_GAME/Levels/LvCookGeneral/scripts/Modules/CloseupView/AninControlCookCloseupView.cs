using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace TidyCooking.Levels
{
    public class AninControlCookCloseupView : MonoBehaviour
    {
        public RectTransform mainFrameTransform;
        public Button btnOverlay;

        [System.Serializable]
        public class AnimConfig
        {
            
        }

        public float alphaOverlay = 0.5f;
        public float durationOpen = 0.5f;
        public float durationClose = 0.25f;
        private Tween _tweenOpenClose;

        private bool _isShow;

        public void SetUp(bool isInitStateShow)
        {
            if (isInitStateShow)
            {
                ShowInstantly();
            }
            else
            {
                HideInstantly();
            }
        }

        public void ShowInstantly()
        {
            _isShow = true;
            mainFrameTransform.localScale = Vector3.one;
            btnOverlay.image.SetA(alphaOverlay);
        }

        public void PlayAnimShow(System.Action onDone)
        {
            if (_isShow) return;
            _isShow = true;

            _tweenOpenClose.Kill();
            _tweenOpenClose = DOTween.Sequence()
                .Append(mainFrameTransform.DOScale(Vector3.one, durationOpen).SetEase(Ease.OutBack))
                .Join(btnOverlay.image.DOFade(alphaOverlay, durationOpen).SetEase(Ease.OutQuad))
                .SetUpdate(true)
                .OnComplete(OnDone)
                .Play();

            void OnDone()
            {
                onDone?.Invoke();
            }
        }

        public void HideInstantly()
        {
            _isShow = false;
            mainFrameTransform.localScale = Vector3.zero;
            btnOverlay.image.SetA(0);
        }

        public void PlayAnimHide(System.Action onDone)
        {
            if (!_isShow) return;
            _isShow = false;

            _tweenOpenClose.Kill();
            _tweenOpenClose = DOTween.Sequence()
                .Append(mainFrameTransform.DOScale(Vector3.zero, durationClose).SetEase(Ease.InBack))
                .Join(btnOverlay.image.DOFade(0f, durationClose).SetEase(Ease.OutQuad))
                .SetUpdate(true)
                .OnComplete(OnDone)
                .Play();

            void OnDone()
            {
                onDone?.Invoke();
            }
        }

        private void OnDestroy()
        {
            _tweenOpenClose.Kill();
        }

#if UNITY_EDITOR
        [Sirenix.OdinInspector.Button("Show Immediate"), Sirenix.OdinInspector.ButtonGroup("ShowHideEditor")]
        private void ShowImmediateEditor()
        {
            ShowInstantly();
            // set dirty
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.EditorUtility.SetDirty(mainFrameTransform);
            UnityEditor.EditorUtility.SetDirty(btnOverlay);
        }

        [Sirenix.OdinInspector.Button("Hide Immediate"), Sirenix.OdinInspector.ButtonGroup("ShowHideEditor")]
        private void HideImmediateEditor()
        {
            HideInstantly();
            // set dirty
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.EditorUtility.SetDirty(mainFrameTransform);
            UnityEditor.EditorUtility.SetDirty(btnOverlay);
        }
#endif
    }
}