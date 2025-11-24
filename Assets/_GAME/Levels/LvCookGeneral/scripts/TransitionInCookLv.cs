using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace TidyCooking.Levels
{
    public class TransitionInCookLv : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        private Tween _tweenShow, _tweenHide;
        private System.Action _onDoneShow, _onDoneHide;

        public void SetUp()
        {
            _tweenShow = _canvasGroup.DOFade(1f, 0.25f).From(0f).OnComplete(OnDoneTweenShow).SetUpdate(true).SetAutoKill(false).Pause();
            _tweenHide = _canvasGroup.DOFade(0f, 0.25f).From(1f).OnComplete(OnDoneTweenHide).SetUpdate(true).SetAutoKill(false).Pause();
        }

        public void Show(System.Action onDone)
        {
            _onDoneHide = null;
            _tweenHide.Pause();
            _onDoneShow = onDone;
            _tweenShow.Restart();
        }
        private void OnDoneTweenShow()
        {
            _onDoneShow?.Invoke();
        }

        public void Hide(System.Action onDone)
        {
            _onDoneShow = null;
            _tweenShow.Pause();
            _onDoneHide = onDone;
            _tweenHide.Restart();
        }
        private void OnDoneTweenHide()
        {
            _onDoneHide?.Invoke();
        }
    }
}