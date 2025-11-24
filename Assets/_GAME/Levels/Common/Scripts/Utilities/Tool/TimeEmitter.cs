using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Utilities
{
    public class TimeEmitter : MonoBehaviour
    {
        public enum UpdateType
        {
            Update,
            FixedUpdate,
            LateUpdate
        }

        public float duration = 10f;
        public bool isLoop = true;
        public float emitOverTimeMultiplier = 1f;
        [ValueCurve] public AnimationCurve emitOverTime;

        [System.Serializable]
        public class BurstConfig
        {
            public float time = 0f;
            public int count = 1;
            public float probability = 1f;

            public static int CompareByTime(BurstConfig a, BurstConfig b)
            {
                return a.time.CompareTo(b.time);
            }
        }
        [DrawWithUnity]
        public BurstConfig[] burstConfigs;
        public bool timeInBurstUseRelativeTime = false;

        [Header("Update")]
        public UpdateType updateType = UpdateType.Update;
        public bool isUseUnscaledTime = false;
        public float simulateSpeed = 1f;

        private float _timePassed;
        private float _lastTimeEmit;
        private int _currentBurstIndex = 0;
        private float _timeBurst;

        public event System.Action onEmit;
        public event System.Action<int> onEmitBurst;

        private void OnEnable()
        {
            _timePassed = 0f;
            _lastTimeEmit = 0f;
            _currentBurstIndex = 0;
            UpdateTimeBurst();
        }

        private float TimeNow => isUseUnscaledTime ? Time.unscaledTime : Time.time;

        private void Update()
        {
            if (updateType == UpdateType.Update)
            {
                UpdateEmit();
            }
        }

        private void FixedUpdate()
        {
            if (updateType == UpdateType.FixedUpdate)
            {
                UpdateEmit();
            }
        }

        private void LateUpdate()
        {
            if (updateType == UpdateType.LateUpdate)
            {
                UpdateEmit();
            }
        }

        private void UpdateEmit()
        {
            _timePassed += (isUseUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime) * simulateSpeed;

            if (_timePassed > duration)
            {
                if (isLoop)
                {
                    _timePassed = Mathf.Repeat(_timePassed, duration);
                    _currentBurstIndex = 0;
                    UpdateTimeBurst();
                }
                else
                {
                    this.enabled = false;
                    return;
                }
            }

            float emitRate = emitOverTimeMultiplier * emitOverTime.Evaluate(_timePassed / duration);
            if (emitRate > 0)
            {
                float timeStep = 1f / emitRate;
                if (_timePassed > _lastTimeEmit + timeStep)
                {
                    _lastTimeEmit = _timePassed;
                    onEmit?.Invoke();
                }
            }

            if (_currentBurstIndex < burstConfigs.Length)
            {
                if (_timePassed >= _timeBurst)
                {
                    if (Random.value < burstConfigs[_currentBurstIndex].probability)
                    {
                        onEmitBurst?.Invoke(burstConfigs[_currentBurstIndex].count);
                    }
                    _currentBurstIndex += 1;
                    UpdateTimeBurst();
                }
            }
        }

        private void UpdateTimeBurst()
        {
            if (_currentBurstIndex < burstConfigs.Length)
            {
                _timeBurst = (timeInBurstUseRelativeTime ? duration : 1f) * burstConfigs[_currentBurstIndex].time;
            }
        }
    }
}