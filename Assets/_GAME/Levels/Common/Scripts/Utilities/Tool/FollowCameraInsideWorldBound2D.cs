using System.Collections;
using UnityEngine;

namespace Utilities
{
    public class FollowCameraInsideWorldBound2D : MonoBehaviour
    {
        public Camera followCamera;
        public Rect worldBound;
        public float lerpSpeed = 0.5f;
        private ComponentOverrideWithRegisteredSource<Transform> targets;

        private void Awake()
        {
            targets = new ComponentOverrideWithRegisteredSource<Transform>();
        }

        public void AddTarget(Object registeredSource, Transform target, int priority) => targets.AddModifier(registeredSource, target, priority);
        public void RemoveTarget(Object registeredSource) => targets.RemoveModifier(registeredSource);

        private void LateUpdate()
        {
            if (!targets.ValueHighestPriority) return;
            Vector2 targetPos = targets.ValueHighestPriority.position;
            Vector2 cameraSize = new Vector2(followCamera.orthographicSize * followCamera.aspect, followCamera.orthographicSize);
            if (worldBound.width > cameraSize.x * 2f)
            {
                targetPos.x = Mathf.Clamp(targetPos.x, worldBound.xMin + cameraSize.x, worldBound.xMax - cameraSize.x);
            }
            else
            {
                targetPos.x = worldBound.center.x;
            }
            if (worldBound.height > cameraSize.y * 2f)
            {
                targetPos.y = Mathf.Clamp(targetPos.y, worldBound.yMin + cameraSize.y, worldBound.yMax - cameraSize.y);
            }
            else
            {
                targetPos.y = worldBound.center.y;
            }
            Vector3 originPos = followCamera.transform.position;
            followCamera.transform.position = new Vector3(
                Mathf.Lerp(originPos.x, targetPos.x, lerpSpeed * 50f * Time.deltaTime),
                Mathf.Lerp(originPos.y, targetPos.y, lerpSpeed * 50f * Time.deltaTime),
                originPos.z
                );
        }

        private void OnDrawGizmosSelected()
        {
            GizmoUtility.DrawRect(worldBound, Color.green);
        }
    }
}