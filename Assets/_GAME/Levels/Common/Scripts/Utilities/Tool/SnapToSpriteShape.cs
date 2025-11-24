using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace Utilities
{
    public class SnapToSpriteShape : MonoBehaviour
    {
        public Transform snapTransform;
        public float detectRadius = 1f;
        public bool flipNormal = false;

        [Sirenix.OdinInspector.Button]
        public void Snap()
        {
            if (!snapTransform)
            {
                snapTransform = this.transform;

#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(this);
#endif
            }

            var hits = Physics2D.OverlapCircleAll(transform.position, detectRadius);
            SpriteShapeController spriteShapeController = null;
            foreach (var hit in hits)
            {
                if (hit.transform.TryGetComponent<SpriteShapeController>(out spriteShapeController))
                {
                    break;
                }
            }
            if (!spriteShapeController) return;

            Vector2 nearestPoint = spriteShapeController.GetNearestPointInShape(transform.position, out int segmentStartIndex, out int segmentEndIndex, out Vector2 normal);
            if (flipNormal) normal = -normal;
            Vector2 snapPoint = nearestPoint + normal * spriteShapeController.colliderOffset;
            snapTransform.SetPositionAndRotation(snapPoint, Quaternion.LookRotation(Vector3.forward, normal));

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(snapTransform);
#endif
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.gray;
            Gizmos.DrawWireSphere(transform.position, detectRadius);
        }
    }
}