using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Utilities
{
    public class ObjectScatterGroup : MonoBehaviour
    {
        [Header("Settings")]
        [DrawWithUnity][SerializeField] private GameObject[] _prefabs;
        [SerializeField] private Transform _container;
        [SerializeField] private Vector2Int _countRange = new Vector2Int(3, 5);
        [SerializeField] private float _circleAreaRadius = 1f;
        public Transform Container => _container;

        [Header("Distribution control")]
        [FoldoutGroup("Distribution control")]
        [SerializeField] private Vector2 _rectAreaSize = Vector2.zero;
        [FoldoutGroup("Distribution control")]
        [SerializeField] private Vector2Int _division = Vector2Int.one;
        [FoldoutGroup("Distribution control")]
        [SerializeField] private Vector2 _rectRandomness = new Vector2(0.5f, 0.5f);

        [Header("Randomize Range")]
        [SerializeField] private bool _isUseSeed = false;
        [SerializeField] private int _seed = 0;
        [SerializeField] private Vector2 _angleRange = new Vector2(0f, 360f);
        [SerializeField] private Vector2 _scaleRange = new Vector2(0.9f, 1.1f);
        [SerializeField] private Gradient _colorRange = new Gradient();
        [DrawWithUnity][SerializeField] private Sprite[] _sprites;

        public int Seed { get => _seed; set => _seed = value; }

        public Vector3[] GetRandomPositions()
        {
            Vector3[] positions = new Vector3[_container.childCount];

            Vector2Int division = new Vector2Int(Mathf.Max(1, _division.x), Mathf.Max(1, _division.y));
            int randomDataDivision = division.x * 89 + division.y * 97;

            Vector2[] randomPosNorm = _isUseSeed
                ? Utilities.RandomUtility.GetRandomPositionsInDividedAreaNormalized(division, _rectRandomness, _container.childCount, randomDataDivision, _seed)
                : Utilities.RandomUtility.GetRandomPositionsInDividedAreaNormalized(division, _rectRandomness, _container.childCount);
            Vector2 startPos = new Vector2(-_rectAreaSize.x / 2f, -_rectAreaSize.y / 2f);
            for (int i = _container.childCount - 1; i >= 0; i--)
            {
                int randomDataIndex = i * 97 + _container.childCount;

                Transform child = _container.GetChild(i);
                Vector2 circleRandomNorm = _isUseSeed ? RandomUtility.RandomInsideInitCircle(randomDataIndex, _seed) : UnityEngine.Random.insideUnitCircle;
                positions[i] = new Vector2(startPos.x + randomPosNorm[i].x * _rectAreaSize.x, startPos.y + randomPosNorm[i].y * _rectAreaSize.y) + circleRandomNorm * _circleAreaRadius;
            }

            return positions;
        }

        public Vector3 GetRandomPosition(int index)
        {
            index = Mathf.Clamp(index, 0, _container.childCount - 1);

            Vector2Int division = new Vector2Int(Mathf.Max(1, _division.x), Mathf.Max(1, _division.y));
            int randomDataDivision = division.x * 89 + division.y * 97;

            Vector2[] randomPosNorm = _isUseSeed
                ? Utilities.RandomUtility.GetRandomPositionsInDividedAreaNormalized(division, _rectRandomness, _container.childCount, randomDataDivision, _seed)
                : Utilities.RandomUtility.GetRandomPositionsInDividedAreaNormalized(division, _rectRandomness, _container.childCount);
            Vector2 startPos = new Vector2(-_rectAreaSize.x / 2f, -_rectAreaSize.y / 2f);

            int randomDataIndex = index * 97 + _container.childCount;
            Transform child = _container.GetChild(index);
            Vector2 circleRandomNorm = _isUseSeed ? RandomUtility.RandomInsideInitCircle(randomDataIndex, _seed) : UnityEngine.Random.insideUnitCircle;
            return new Vector2(startPos.x + randomPosNorm[index].x * _rectAreaSize.x, startPos.y + randomPosNorm[index].y * _rectAreaSize.y) + circleRandomNorm * _circleAreaRadius;
        }

        public float[] GetRandomAngles()
        {
            float[] angles = new float[_container.childCount];
            for (int i = _container.childCount - 1; i >= 0; i--)
            {
                int randomDataIndex = i * 97 + _container.childCount;
                angles[i] = _isUseSeed ? RandomUtility.GetRange(_angleRange.x, _angleRange.y, randomDataIndex, _seed) : UnityEngine.Random.Range(_angleRange.x, _angleRange.y);
            }
            return angles;
        }

        public float GetRandomAngle(int index)
        {
            index = Mathf.Clamp(index, 0, _container.childCount - 1);
            int randomDataIndex = index * 97 + _container.childCount;
            return _isUseSeed ? RandomUtility.GetRange(_angleRange.x, _angleRange.y, randomDataIndex, _seed) : UnityEngine.Random.Range(_angleRange.x, _angleRange.y);
        }

        public float[] GetRandomScales()
        {
            float[] scales = new float[_container.childCount];
            for (int i = _container.childCount - 1; i >= 0; i--)
            {
                int randomDataIndex = i * 97 + _container.childCount;
                scales[i] = _isUseSeed ? RandomUtility.GetRange(_scaleRange.x, _scaleRange.y, randomDataIndex, _seed) : UnityEngine.Random.Range(_scaleRange.x, _scaleRange.y);
            }
            return scales;
        }

        public float GetRandomScale(int index)
        {
            index = Mathf.Clamp(index, 0, _container.childCount - 1);
            int randomDataIndex = index * 97 + _container.childCount;
            return _isUseSeed ? RandomUtility.GetRange(_scaleRange.x, _scaleRange.y, randomDataIndex, _seed) : UnityEngine.Random.Range(_scaleRange.x, _scaleRange.y);
        }

        public void GenerateScatterObjects()
        {
            for (int i = _container.childCount - 1; i >= 0; i--)
            {
                Destroy(_container.GetChild(i).gameObject);
            }

            if (_prefabs == null || _prefabs.Length == 0) return;

            int randomDataCount = _countRange.x * 503 + _countRange.y * 509;
            int count = _isUseSeed ? RandomUtility.GetRange(_countRange.x, _countRange.y + 1, randomDataCount, _seed) : UnityEngine.Random.Range(_countRange.x, _countRange.y + 1);
            if (count <= 0) count = 1;

            Vector2Int division = new Vector2Int(Mathf.Max(1, _division.x), Mathf.Max(1, _division.y));
            int[] shuffledPrefabIndex = Utilities.RandomUtility.GetShuffledIndexArray(_prefabs.Length);
            Vector2[] randomPosNorm = Utilities.RandomUtility.GetRandomPositionsInDividedAreaNormalized(division, _rectRandomness, count);

            Vector2 startPos = new Vector2(-_rectAreaSize.x / 2f, -_rectAreaSize.y / 2f);
            for (int i = 0; i < count; i++)
            {
                int prefabIndex = shuffledPrefabIndex[i % _prefabs.Length];

                if (_prefabs[prefabIndex] == null) continue;

                GameObject obj;
                obj = Instantiate(_prefabs[prefabIndex], _container);

                Vector3 position = new Vector2(startPos.x + randomPosNorm[i].x * _rectAreaSize.x, startPos.y + randomPosNorm[i].y * _rectAreaSize.y) + UnityEngine.Random.insideUnitCircle * _circleAreaRadius;
                Vector3 scale = Vector3.one * UnityEngine.Random.Range(_scaleRange.x, _scaleRange.y);
                Quaternion rotation = Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(_angleRange.x, _angleRange.y));
                Color color = _colorRange.Evaluate(UnityEngine.Random.value);

                obj.transform.SetLocalPositionAndRotation(position, rotation);
                obj.transform.localScale = scale;

                if (obj.TryGetComponent(out SpriteRenderer spriteRenderer))
                {
                    spriteRenderer.color = color;
                }

                obj.SetActive(true);
            }
        }

#if UNITY_EDITOR
        [Header("Editor Tool")]
        [SerializeField] private bool _isUpdateOnValidate = false;
        private void OnValidate()
        {
            if (_isUpdateOnValidate && gameObject.scene.name != null)
            {
                RandomizeRotationsEditor();
                RandomizeScalesEditor();
                RandomizeColorsEditor();
            }
        }

        [Sirenix.OdinInspector.Button(ButtonHeight = 50)]
        private void GenerateScatterObjectsEditor()
        {
            ClearChildsEditor();

            if (_prefabs == null || _prefabs.Length == 0) return;

            int randomDataCount = _countRange.x * 503 + _countRange.y * 509;
            int count = _isUseSeed ? RandomUtility.GetRange(_countRange.x, _countRange.y + 1, randomDataCount, _seed) : UnityEngine.Random.Range(_countRange.x, _countRange.y + 1);
            if (count <= 0) count = 1;

            int randomDataPrefab = _prefabs.Length;
            int[] shuffledPrefabIndex = _isUseSeed
                ? Utilities.RandomUtility.GetShuffledIndexArray(_prefabs.Length, randomDataPrefab, _seed)
                : Utilities.RandomUtility.GetShuffledIndexArray(_prefabs.Length);

            Vector2Int division = new Vector2Int(Mathf.Max(1, _division.x), Mathf.Max(1, _division.y));
            int randomDataDivision = division.x * 89 + division.y * 97;
            Vector2[] randomPosNorm = _isUseSeed
                ? Utilities.RandomUtility.GetRandomPositionsInDividedAreaNormalized(division, _rectRandomness, count, randomDataDivision, _seed)
                : Utilities.RandomUtility.GetRandomPositionsInDividedAreaNormalized(division, _rectRandomness, count);

            Vector2 startPos = new Vector2(-_rectAreaSize.x / 2f, -_rectAreaSize.y / 2f);
            for (int i = 0; i < count; i++)
            {
                int randomDataIndex = i * 97 + count;

                int prefabIndex = shuffledPrefabIndex[i % _prefabs.Length];

                if (_prefabs[prefabIndex] == null) continue;

                GameObject obj;
                if (_prefabs[prefabIndex].scene.name == null) // is prefab
                {
                    obj = UnityEditor.PrefabUtility.InstantiatePrefab(_prefabs[prefabIndex], _container) as GameObject;
                }
                else
                {
                    obj = Instantiate(_prefabs[prefabIndex], _container);
                }

                Vector2 circleRandomNorm = _isUseSeed ? RandomUtility.RandomInsideInitCircle(randomDataIndex, _seed) : UnityEngine.Random.insideUnitCircle;
                Vector3 position = new Vector2(startPos.x + randomPosNorm[i].x * _rectAreaSize.x, startPos.y + randomPosNorm[i].y * _rectAreaSize.y) + circleRandomNorm * _circleAreaRadius;
                Vector3 scale = Vector3.one * (_isUseSeed ? RandomUtility.GetRange(_scaleRange.x, _scaleRange.y, randomDataIndex, _seed) : UnityEngine.Random.Range(_scaleRange.x, _scaleRange.y));
                Quaternion rotation = Quaternion.Euler(0f, 0f, _isUseSeed ? RandomUtility.GetRange(_angleRange.x, _angleRange.y, randomDataIndex, _seed) : UnityEngine.Random.Range(_angleRange.x, _angleRange.y));
                Color color = _colorRange.Evaluate(_isUseSeed ? RandomUtility.GetValue01(randomDataIndex, _seed) : UnityEngine.Random.value);

                obj.transform.SetLocalPositionAndRotation(position, rotation);
                obj.transform.localScale = scale;

                if (obj.TryGetComponent(out SpriteRenderer spriteRenderer))
                {
                    spriteRenderer.color = color;
                    if (_sprites != null && _sprites.Length > 0)
                    {
                        int randomSpriteIndex = _isUseSeed ? RandomUtility.GetRange(0, _sprites.Length, randomDataIndex, _seed) : UnityEngine.Random.Range(0, _sprites.Length);
                        spriteRenderer.sprite = _sprites[randomSpriteIndex];
                    }
                }

                obj.SetActive(true);
            }
        }

        [Sirenix.OdinInspector.Button]
        private void RandomizePositionsEditor()
        {
            Vector2Int division = new Vector2Int(Mathf.Max(1, _division.x), Mathf.Max(1, _division.y));
            int randomDataDivision = division.x * 89 + division.y * 97;

            Vector2[] randomPosNorm = _isUseSeed
                ? Utilities.RandomUtility.GetRandomPositionsInDividedAreaNormalized(division, _rectRandomness, _container.childCount, randomDataDivision, _seed)
                : Utilities.RandomUtility.GetRandomPositionsInDividedAreaNormalized(division, _rectRandomness, _container.childCount);
            Vector2 startPos = new Vector2(-_rectAreaSize.x / 2f, -_rectAreaSize.y / 2f);
            for (int i = _container.childCount - 1; i >= 0; i--)
            {
                int randomDataIndex = i * 97 + _container.childCount;

                Transform child = _container.GetChild(i);
                Vector2 circleRandomNorm = _isUseSeed ? RandomUtility.RandomInsideInitCircle(randomDataIndex, _seed) : UnityEngine.Random.insideUnitCircle;
                Vector3 position = new Vector2(startPos.x + randomPosNorm[i].x * _rectAreaSize.x, startPos.y + randomPosNorm[i].y * _rectAreaSize.y) + circleRandomNorm * _circleAreaRadius;
                child.SetLocalPositionAndRotation(position, child.rotation);
            }
        }

        [Sirenix.OdinInspector.Button]
        private void RandomizeRotationsEditor()
        {
            for (int i = _container.childCount - 1; i >= 0; i--)
            {
                int randomDataIndex = i * 97 + _container.childCount;

                Transform child = _container.GetChild(i);
                Quaternion rotation = Quaternion.Euler(0f, 0f, _isUseSeed ? RandomUtility.GetRange(_angleRange.x, _angleRange.y, randomDataIndex, _seed) : UnityEngine.Random.Range(_angleRange.x, _angleRange.y));
                child.SetLocalPositionAndRotation(child.localPosition, rotation);
            }
        }

        [Sirenix.OdinInspector.Button]
        private void RandomizeScalesEditor()
        {
            for (int i = _container.childCount - 1; i >= 0; i--)
            {
                int randomDataIndex = i * 97 + _container.childCount;

                Transform child = _container.GetChild(i);
                Vector3 scale = Vector3.one * (_isUseSeed ? RandomUtility.GetRange(_scaleRange.x, _scaleRange.y, randomDataIndex, _seed) : UnityEngine.Random.Range(_scaleRange.x, _scaleRange.y));
                child.localScale = scale;
            }
        }

        [Sirenix.OdinInspector.Button]
        private void RandomizeColorsEditor()
        {
            for (int i = _container.childCount - 1; i >= 0; i--)
            {
                int randomDataIndex = i * 97 + _container.childCount;

                Transform child = _container.GetChild(i);
                SpriteRenderer spriteRenderer = child.GetComponent<SpriteRenderer>();
                if (spriteRenderer)
                {
                    Color color = _colorRange.Evaluate(_isUseSeed ? RandomUtility.GetValue01(randomDataIndex, _seed) : UnityEngine.Random.value);
                    spriteRenderer.color = color;
                }
            }
        }

        [Sirenix.OdinInspector.Button]
        private void RandomizeSpritesEditor()
        {
            if (_sprites != null && _sprites.Length > 0)
            {
                for (int i = _container.childCount - 1; i >= 0; i--)
                {
                    int randomDataIndex = i * 97 + _container.childCount;

                    Transform child = _container.GetChild(i);
                    SpriteRenderer spriteRenderer = child.GetComponent<SpriteRenderer>();
                    if (spriteRenderer)
                    {
                        int randomSpriteIndex = _isUseSeed ? RandomUtility.GetRange(0, _sprites.Length, randomDataIndex, _seed) : UnityEngine.Random.Range(0, _sprites.Length);
                        spriteRenderer.sprite = _sprites[randomSpriteIndex];
                    }
                }
            }
        }

        [Sirenix.OdinInspector.Button]
        private void ClearChildsEditor()
        {
            for (int i = _container.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(_container.GetChild(i).gameObject);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.white;

            Matrix4x4 backupGizmo = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);

            if (_circleAreaRadius < 0.01f)
            {
                Gizmos.DrawWireCube(Vector3.zero, new Vector3(_rectAreaSize.x, _rectAreaSize.y, 0.1f));
            }
            else if (_rectAreaSize.x + _rectAreaSize.y < 0.01f)
            {
                Gizmos.DrawWireSphere(Vector3.zero, _circleAreaRadius);
            }
            else
            {
                Vector3 cornerLT = new Vector3(-_rectAreaSize.x / 2f, _rectAreaSize.y / 2f);
                Vector3 cornerRT = new Vector3(_rectAreaSize.x / 2f, _rectAreaSize.y / 2f);
                Vector3 cornerLB = new Vector3(-_rectAreaSize.x / 2f, -_rectAreaSize.y / 2f);
                Vector3 cornerRB = new Vector3(_rectAreaSize.x / 2f, -_rectAreaSize.y / 2f);

                Gizmos.DrawWireSphere(cornerLT, _circleAreaRadius);
                Gizmos.DrawWireSphere(cornerRT, _circleAreaRadius);
                Gizmos.DrawWireSphere(cornerLB, _circleAreaRadius);
                Gizmos.DrawWireSphere(cornerRB, _circleAreaRadius);

                Gizmos.DrawLine(cornerLT + Vector3.up * _circleAreaRadius, cornerRT + Vector3.up * _circleAreaRadius);
                Gizmos.DrawLine(cornerRT + Vector3.right * _circleAreaRadius, cornerRB + Vector3.right * _circleAreaRadius);
                Gizmos.DrawLine(cornerRB + Vector3.down * _circleAreaRadius, cornerLB + Vector3.down * _circleAreaRadius);
                Gizmos.DrawLine(cornerLB + Vector3.left * _circleAreaRadius, cornerLT + Vector3.left * _circleAreaRadius);
            }

            Gizmos.matrix = backupGizmo;
        }
#endif
    }
}