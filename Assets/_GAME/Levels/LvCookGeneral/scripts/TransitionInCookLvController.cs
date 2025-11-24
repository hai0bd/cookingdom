using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace TidyCooking.Levels
{
    public class TransitionInCookLvController : MonoBehaviour
    {
        public enum TransitionType
        {
            None,
            FadeOverlay,
            Slide
        }

        public enum State
        {
            None,
            Showing,
            Shown,
            Hiding,
            Hidden
        }

        [SerializeField, ArrayElementNameMatchEnum(typeof(TransitionType))] private AssetReference[] _transitionPrefabs;
        private AsyncOperationHandle<GameObject>[] _opLoadTransitionPrefabs;
        private TransitionInCookLv[] _transitions;
        [SerializeField] private RectTransform _transitionContainer;

        private int _currentTransitionIndex = -1;
        public State CurrentState { get; private set; } = State.None;

        private void Awake()
        {
            _opLoadTransitionPrefabs = new AsyncOperationHandle<GameObject>[_transitionPrefabs.Length];
            _transitions = new TransitionInCookLv[_transitionPrefabs.Length];
        }

        public async void ShowTransition(TransitionType transitionType, System.Action onDoneShow = null)
        {
            if (_currentTransitionIndex >= 0)
            {
                int prevTransition = _currentTransitionIndex;
                _currentTransitionIndex = -1;
                _transitions[prevTransition].Hide(OnDoneHidePrev);

                void OnDoneHidePrev()
                {
                    ShowTransition(transitionType, onDoneShow);
                }

                return;
            }

            int index = Mathf.Clamp((int)transitionType, 0, _transitionPrefabs.Length - 1);
            _currentTransitionIndex = index;

            if (!_opLoadTransitionPrefabs[index].IsValid())
            {
                _opLoadTransitionPrefabs[index] = Addressables.LoadAssetAsync<GameObject>(_transitionPrefabs[index]);
            }
            await _opLoadTransitionPrefabs[index].Task;

            if (!_transitions[index])
            {
                _transitions[index] = Instantiate(_opLoadTransitionPrefabs[index].Result, _transitionContainer).GetComponent<TransitionInCookLv>();
                _transitions[index].SetUp();
            }

            if (_currentTransitionIndex == index)
            {
                _transitions[index].Show(onDoneShow);
            }
        }

        public void HideTransition(System.Action onDoneHide)
        {
            if (_currentTransitionIndex >= 0)
            {
                int index = _currentTransitionIndex;
                _currentTransitionIndex = -1;
                _transitions[index].Hide(onDoneHide);
            }
        }

        private void OnDestroy()
        {
            if (_opLoadTransitionPrefabs != null)
            {
                for (int i = 0; i < _opLoadTransitionPrefabs.Length; i++)
                {
                    if (_opLoadTransitionPrefabs[i].IsValid())
                    {
                        Addressables.Release(_opLoadTransitionPrefabs[i]);
                    }
                }
            }
        }
    }
}