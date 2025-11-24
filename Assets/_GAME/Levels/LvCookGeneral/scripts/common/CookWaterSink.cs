using System;
using System.Collections;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;
using Utilities;

namespace TidyCooking.Levels
{
    public class CookWaterSink : MonoBehaviour
    {
        public enum WaterFlowStateEnum
        {
            Off,
            On
        }

        public enum SinkPlugStateEnum
        {
            Unplugged,
            Plugged
        }

        [System.Serializable]
        public class VisualConfig
        {
            [Header("Water Flow")]
            public float durationWaterFlowFade = 0.1f;

            [Header("Water")]
            public Vector2 waterScaleRangeX = new Vector2(0.925f, 1f);
            public Vector2 waterScaleRangeY = new Vector2(0.8f, 1f);

            public float durationWaterFadeIn = 0.5f;
            public float durationWaterScaleIn = 1f;

            public float durationWaterFadeOut = 0.5f;
            public float durationWaterScaleOut = 0.25f;

            [Header("Dirt")]
            public float durationDirtFadeIn = 0.25f;
            public float durationDirtFadeOut = 0.25f;

            [Header("Plug")]
            public float durationSnapPlug = 0.15f;
        }

        [SerializeField] private VisualConfig _visualConfig;
        public bool IsHaveWater { get; set; } = false;

        [Header("Water Flow Control")]
        [SerializeField] private ForwardMouseEvent _flowButton;
        [SerializeField] private Transform _waterScaleAnchor;
        [SerializeField] private SpriteRenderer _waterRenderer;
        [SerializeField] private SpriteRenderer _waterFlowRenderer;
        [SerializeField] private AudioSource _audioSourceWaterFlow;
        public BoolModifierWithRegisteredSource BlockControlWaterFlow { get; private set; } = new BoolModifierWithRegisteredSource();
        public WaterFlowStateEnum WaterFlowState { get; private set; } = WaterFlowStateEnum.Off;

        [Header("Plug")]
        [SerializeField] private DraggableObject _draggablePlug;
        [SerializeField] private SortingGroup _plugSortingGroup;
        [SerializeField] private Transform _sinkHole;
        [SerializeField] private int _plugSortingOrderOnSnap = 1;
        [SerializeField] private float _sinkHoleSnapRadius = 1f;
        public BoolModifierWithRegisteredSource BlockControlPlug { get; private set; } = null;
        public SinkPlugStateEnum SinkPlugState { get; private set; } = SinkPlugStateEnum.Unplugged;

        [Header("Dirt")]
        [SerializeField] private SpriteRenderer[] _dirtRenderers;
        [SerializeField] private int _levelDirtShow = 3;
        public int DirtLevel { get; set; } = 0;
        public bool IsDirty => DirtLevel >= _levelDirtShow;

        public event System.Action<int> onDirtLevelChanged;
        public event System.Action onDirted;

        private float[] _originDirtAlphas;

        private Tween _tweenWaterFlow;
        private Tween _tweenSnapSinkPlug;
        private Sequence _tweenDirt;

        private void Awake()
        {
            BlockControlPlug = new BoolModifierWithRegisteredSource(OnChangedBlockControlPlug);
            _flowButton.onMouseDown += OnClickWaterFlowButton;
            _draggablePlug.OnStartDrag.AddListener(OnStartDragPlug);
            _draggablePlug.OnEndDrag.AddListener(OnEndDragPlug);

            _originDirtAlphas = new float[_dirtRenderers.Length];
            for (int i = 0; i < _dirtRenderers.Length; i++)
            {
                _originDirtAlphas[i] = _dirtRenderers[i].color.a;
            }

            void OnChangedBlockControlPlug()
            {
                if (BlockControlPlug.Value)
                {
                    _draggablePlug.isPreventDrag.AddModifier(this);
                }
                else
                {
                    _draggablePlug.isPreventDrag.RemoveModifier(this);
                }
            }
        }

        private void Start()
        {
            UpdateWaterLevelImmediate();
            UpdateDirtImmediate();
        }

        private void OnStartDragPlug()
        {
            if (BlockControlPlug.Value) return;
            _tweenSnapSinkPlug.Kill();
            SinkPlugState = SinkPlugStateEnum.Unplugged;
            IsHaveWater = false;
            UpdateWaterLevel();
            UpdateDirt();
        }

        private void OnEndDragPlug()
        {
            if (Vector2.Distance(_draggablePlug.transform.position, _sinkHole.transform.position) < _sinkHoleSnapRadius)
            {
                SinkPlugState = SinkPlugStateEnum.Plugged;
                if (WaterFlowState == WaterFlowStateEnum.On) IsHaveWater = true;
                _plugSortingGroup.sortingOrder = _plugSortingOrderOnSnap;
                _tweenSnapSinkPlug = _draggablePlug.transform.DOMove(_sinkHole.transform.position + Vector3.back * 0.0001f, _visualConfig.durationSnapPlug).Play();
                UpdateWaterLevel();
                UpdateDirt();
            }
            else
            {
                SinkPlugState = SinkPlugStateEnum.Unplugged;
            }
        }

        private void OnClickWaterFlowButton(ForwardMouseEvent @event)
        {
            if (BlockControlWaterFlow.Value) return;
            switch (WaterFlowState)
            {
                case WaterFlowStateEnum.Off:
                    ChangeWaterFlowState(WaterFlowStateEnum.On);
                    break;
                case WaterFlowStateEnum.On:
                    ChangeWaterFlowState(WaterFlowStateEnum.Off);
                    break;
            }
        }

        private void ChangeWaterFlowState(WaterFlowStateEnum waterFlowState)
        {
            if (BlockControlWaterFlow.Value) return;
            if (waterFlowState == WaterFlowState) return;
            WaterFlowState = waterFlowState;

            if (waterFlowState == WaterFlowStateEnum.On && SinkPlugState == SinkPlugStateEnum.Plugged)
            {
                IsHaveWater = true;
            }

            UpdateWaterLevel();
            UpdateDirt();
        }

        private void UpdateWaterLevelImmediate()
        {
            _tweenWaterFlow?.Kill();
            if (IsHaveWater)
            {
                _waterRenderer.SetA(1f);
                _waterScaleAnchor.localScale = Vector3.one;
                _waterFlowRenderer.SetA(1f);
                _audioSourceWaterFlow.Play();
            }
            else
            {
                _waterRenderer.SetA(0f);
                _waterScaleAnchor.localScale = Vector3.zero;
                _waterFlowRenderer.SetA(0f);
                _audioSourceWaterFlow.Stop();
            }
        }

        private void UpdateWaterLevel()
        {
            _tweenWaterFlow?.Kill();
            
            float waterFlowAlpha = WaterFlowState switch
            {
                WaterFlowStateEnum.Off => 0f,
                WaterFlowStateEnum.On => 1f,
                _ => 0f
            };

            if (IsHaveWater)
            {
                _tweenWaterFlow = DOTween.Sequence()
                    .Join(_waterRenderer.DOFade(1f, _visualConfig.durationWaterFadeIn))
                    .Join(_waterScaleAnchor.DOScale(new Vector3(_visualConfig.waterScaleRangeX.y, _visualConfig.waterScaleRangeY.y, 1f), _visualConfig.durationWaterScaleIn))
                    .Join(_waterFlowRenderer.DOFade(waterFlowAlpha, _visualConfig.durationWaterFlowFade))
                    .Play();
                _audioSourceWaterFlow.Play();
            }
            else
            {
                _tweenWaterFlow = DOTween.Sequence()
                    .Join(_waterRenderer.DOFade(0f, _visualConfig.durationWaterFadeOut))
                    .Join(_waterScaleAnchor.DOScale(new Vector3(_visualConfig.waterScaleRangeX.x, _visualConfig.waterScaleRangeY.x, 1f), _visualConfig.durationWaterScaleOut))
                    .Join(_waterFlowRenderer.DOFade(waterFlowAlpha, _visualConfig.durationWaterFlowFade))
                    .Play();
                _audioSourceWaterFlow.Stop();
            }
        }

        private void UpdateDirtImmediate()
        {
            _tweenDirt?.Kill();
            if (IsDirty)
            {
                for (int i = 0; i < _dirtRenderers.Length; i++)
                {
                    _dirtRenderers[i].SetA(_originDirtAlphas[i]);
                }
            }
            else
            {
                for (int i = 0; i < _dirtRenderers.Length; i++)
                {
                    _dirtRenderers[i].SetA(0f);
                }
            }
        }

        private void UpdateDirt()
        {
            _tweenDirt?.Kill();
            _tweenDirt = DOTween.Sequence();
            if (IsDirty && IsHaveWater)
            {
                for (int i = 0; i < _dirtRenderers.Length; i++)
                {
                    _tweenDirt.Join(_dirtRenderers[i].DOFade(_originDirtAlphas[i], _visualConfig.durationDirtFadeIn));
                }
            }
            else
            {
                for (int i = 0; i < _dirtRenderers.Length; i++)
                {
                    _tweenDirt.Join(_dirtRenderers[i].DOFade(0f, _visualConfig.durationDirtFadeOut));
                }
            }
            _tweenDirt.Play();
        }

        [Button]
        public void AddDirt()
        {
            bool isHaveWater = (WaterFlowState == WaterFlowStateEnum.On) && (SinkPlugState == SinkPlugStateEnum.Plugged);
            if (!isHaveWater) return; // only add dirt when have water

            int prevDirt = DirtLevel;
            DirtLevel++;
            UpdateDirt();
            onDirtLevelChanged?.Invoke(DirtLevel);

            if (prevDirt < _levelDirtShow && DirtLevel >= _levelDirtShow)
            {
                onDirted?.Invoke();
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (_sinkHole != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(_sinkHole.position, _sinkHoleSnapRadius);
            }
        }
    }
}