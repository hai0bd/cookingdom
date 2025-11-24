using UnityEngine;
using DG.Tweening;

namespace Utilities
{
    public class FadePanelUIWithTransition : MonoBehaviour, IPanelUIWithTransition
    {
        [SerializeField] protected CanvasGroup _canvasGroup;
        [SerializeField] protected float _fadeDuration = 0.25f;
        private IPanelUIWithTransition.ShowState _state;
        private Tween _tweenFade;

        public IPanelUIWithTransition.ShowState State => _state;

        public void HideImmediately()
        {
            _canvasGroup.alpha = 0;
            _canvasGroup.blocksRaycasts = false;
            gameObject.SetActive(false);
            _state = IPanelUIWithTransition.ShowState.Hide;
        }

        public void HideWithTransition(System.Action onComplete)
        {
            _canvasGroup.blocksRaycasts = false;
            _tweenFade.Kill();
            _state = IPanelUIWithTransition.ShowState.Hiding;
            // _tweenFade = _canvasGroup.DOFade(0, _fadeDuration).OnComplete(() =>
            // {
            //     gameObject.SetActive(false);
            //     _state = IPanelUIWithTransition.ShowState.Hide;
            //     onComplete?.Invoke();
            // });
        }

        public void ShowImmediately()
        {
            _canvasGroup.alpha = 1;
            _canvasGroup.blocksRaycasts = true;
            gameObject.SetActive(true);
            _state = IPanelUIWithTransition.ShowState.Show;
        }

        public void ShowWithTransition(System.Action onComplete)
        {
            gameObject.SetActive(true);
            _tweenFade.Kill();
            _state = IPanelUIWithTransition.ShowState.Showing;
            // _tweenFade = _canvasGroup.DOFade(1, _fadeDuration).OnComplete(() =>
            // {
            //     _canvasGroup.blocksRaycasts = true;
            //     _state = IPanelUIWithTransition.ShowState.Show;
            //     onComplete?.Invoke();
            // });
        }
    }
}