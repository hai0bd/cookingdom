using System.Collections;
using UnityEngine;

namespace Utilities
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(ParticleSystem))]
    public class DemoParticleOnStartEditor : MonoBehaviour
    {
        [SerializeField] private Vector3 _nameOffset = Vector3.zero;
        [SerializeField] private float _simulateTime = 0.1f;

#if UNITY_EDITOR
        [ExecuteInEditMode]
        private void OnEnable()
        {
            Simulate();
        }

        public void Simulate()
        {
            GetComponent<ParticleSystem>().Simulate(_simulateTime, true);
        }

        private void SimulateSelected()
        {
            foreach (var selectedObject in UnityEditor.Selection.gameObjects)
            {
                if (selectedObject.scene == null) continue;
                GetComponent<DemoParticleOnStartEditor>()?.Simulate();
            }
        }

        private void SimulateAll()
        {
            DemoParticleOnStartEditor[] allDemo = FindObjectsOfType<DemoParticleOnStartEditor>();
            foreach (var demo in allDemo) demo.Simulate();
        }
#endif
    }
}