using System.Collections;
using MoreMountains.NiceVibrations;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;
using Utilities;

namespace TidyCooking.Levels
{
    [RequireComponent(typeof(ObjectScatterGroup))]
    public class ObjectScatterAnim : MonoBehaviour
    {
        public enum AnimState
        {
            None,
            Static,
            Drag,
            Floating
        }

        [System.Serializable]
        public class GeneralConfig
        {
            public float lerpSpeed = 0.4f;

            public float delayBetweenParticle = 0.015f;
            public float maxTotalDelayAnim = 0.25f;

            [Header("Drag")]
            public Vector2 liftOnDrag = new Vector2(0.5f, 0.7f);
            public Vector2 rotateOnDrag = new Vector2(5f, 10f);
            public float scaleMulOnDrag = 1.05f;

            [Header("Floating")]
            [ValueCurve(true, true)] public AnimationCurve curveFloatingX = AnimationCurve.Constant(0, 1, 1f);
            [ValueCurve(true, true)] public AnimationCurve curveFloatingY = AnimationCurve.Constant(0, 1, 1f);
            public float durationFloatingCycle = 3f;
            public float floatingNoiseScale = 10f;
        }

        [Header("Config")]
        [SerializeField] private GeneralConfig _generalConfig;
        [SerializeField] private bool _isAutoSetUpOnStart = false;

        [ShowInInspector, ReadOnly]
        public AnimState CurrentAnimState { get; private set; } = AnimState.None;

        [SerializeField] private ObjectScatterGroup _scatterControl;
        [SerializeField] private float _positionExpandMul = 1f;
        [SerializeField] private float _floatingHeight = 0.1f;

        public float PositionExpandMul
        {
            get => _positionExpandMul;
            set
            {
                _positionExpandMul = value;
                UpdateParticles();
            }
        }

        public float FloatingHeight
        {
            get => _floatingHeight;
            set
            {
                _floatingHeight = value;
                if (CurrentAnimState == AnimState.Floating)
                {
                    UpdateParticles();
                }
            }
        }

        private Transform[] _particlesTransform;

        private Vector3[] _particlesOriginLocalPos;
        private float[] _particlesOriginRotate;

        private Vector3[] _desiredParticlesLocalPos;
        private Quaternion[] _desiredParticlesRotation;

        private float[] _delays;
        private float _lastTimeChangeState;
        private float[] _floatingTimeOffsets;

        private void Start()
        {
            if (_isAutoSetUpOnStart) SetUp();
        }

        public void SetUp(GeneralConfig generalConfig = null)
        {
            if (generalConfig != null) _generalConfig = generalConfig;

            // spawn
            int numParticles = _scatterControl.Container.childCount;
            _particlesTransform = new Transform[numParticles];
            _particlesOriginLocalPos = new Vector3[numParticles];
            _particlesOriginRotate = new float[numParticles];

            _desiredParticlesLocalPos = new Vector3[numParticles];
            _desiredParticlesRotation = new Quaternion[numParticles];

            _floatingTimeOffsets = new float[numParticles];

            _delays = new float[numParticles];
            _lastTimeChangeState = -999f;

            for (int i = 0; i < numParticles; i++)
            {
                _particlesTransform[i] = _scatterControl.Container.GetChild(i);
                _particlesOriginLocalPos[i] = _particlesTransform[i].localPosition * _positionExpandMul;
                _particlesOriginRotate[i] = _particlesTransform[i].localEulerAngles.z;
                _desiredParticlesLocalPos[i] = _particlesOriginLocalPos[i];
                _desiredParticlesRotation[i] = Quaternion.Euler(0, 0, _particlesOriginRotate[i]);
                _delays[i] = 0f;
            }
        }

        public void ShuffleItemTransforms()
        {
            _particlesOriginLocalPos = _scatterControl.GetRandomPositions();
            _particlesOriginRotate = _scatterControl.GetRandomAngles();
        }

        public void SetAnim(AnimState animState)
        {
            if (CurrentAnimState == animState) return;
            CurrentAnimState = animState;
            _lastTimeChangeState = Time.time;
            UpdateParticles();
        }

        private void UpdateParticles()
        {
            int[] shuffledIndex = RandomUtility.GetShuffledIndexArray(_particlesTransform.Length);
            float delay = Mathf.Min(_generalConfig.delayBetweenParticle, _generalConfig.maxTotalDelayAnim / _particlesTransform.Length);

            switch (CurrentAnimState)
            {
                case AnimState.Static:
                    for (int i = 0; i < _particlesTransform.Length; i++)
                    {
                        _desiredParticlesLocalPos[i] = _particlesOriginLocalPos[i] * _positionExpandMul;
                        _desiredParticlesRotation[i] = Quaternion.Euler(0, 0, _particlesOriginRotate[i]);
                        _delays[i] = delay * shuffledIndex[i];
                    }
                    break;
                case AnimState.Drag:
                    for (int i = 0; i < _particlesTransform.Length; i++)
                    {
                        _desiredParticlesLocalPos[i] = _particlesOriginLocalPos[i] * _positionExpandMul + Vector3.up * _generalConfig.liftOnDrag.GetRandomWithin();
                        _desiredParticlesRotation[i] = Quaternion.Euler(0, 0, _particlesOriginRotate[i] + _generalConfig.rotateOnDrag.GetRandomWithin() * (UnityEngine.Random.value > 0.5f ? 1 : -1));
                        _delays[i] = delay * shuffledIndex[i];
                    }
                    break;
                case AnimState.Floating:
                    for (int i = 0; i < _particlesTransform.Length; i++)
                    {
                        _desiredParticlesLocalPos[i] = _particlesOriginLocalPos[i] * _positionExpandMul;
                        _desiredParticlesRotation[i] = Quaternion.Euler(0, 0, _particlesOriginRotate[i]);
                        _delays[i] = 0f;
                    }

                    for (int i = 0; i < _particlesTransform.Length; i++)
                    {
                        _floatingTimeOffsets[i] = Mathf.PerlinNoise(_particlesOriginLocalPos[i].x, _particlesOriginLocalPos[i].y) * _generalConfig.durationFloatingCycle;
                    }
                    break;
            }
        }

        private void Update()
        {
            if (CurrentAnimState == AnimState.Floating)
            {
                // floating
                Vector3 pos;
                float floatingProgress;
                Vector3 desiredPos = Vector3.zero;
                for (int i = 0; i < _particlesTransform.Length; i++)
                {
                    if (Time.time < _lastTimeChangeState + _delays[i]) continue;
                    pos = _particlesTransform[i].localPosition;
                    floatingProgress = Mathf.Repeat((Time.time + _floatingTimeOffsets[i]) / _generalConfig.durationFloatingCycle, 1f);
                    desiredPos.x = _desiredParticlesLocalPos[i].x;
                    desiredPos.y = _desiredParticlesLocalPos[i].y + _floatingHeight * _generalConfig.curveFloatingY.Evaluate(floatingProgress);
                    pos.LerpTo(desiredPos, _generalConfig.lerpSpeed);
                    _particlesTransform[i].localPosition = pos;
                }
            }
            else
            {
                Vector3 pos;
                Quaternion rot;
                for (int i = 0; i < _particlesTransform.Length; i++)
                {
                    if (Time.time < _lastTimeChangeState + _delays[i]) continue;
                    pos = _particlesTransform[i].localPosition;
                    pos.LerpTo(_desiredParticlesLocalPos[i], _generalConfig.lerpSpeed);
                    rot = _particlesTransform[i].localRotation;
                    rot.LerpTo(_desiredParticlesRotation[i], _generalConfig.lerpSpeed);
                    _particlesTransform[i].SetLocalPositionAndRotation(pos, rot);
                }
            }
        }
    }
}