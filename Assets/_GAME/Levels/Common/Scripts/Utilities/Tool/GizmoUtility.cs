using System.Collections;
using UnityEngine;

namespace Utilities
{
    public static class GizmoUtility
    {
        #region Point
        public static void DrawPoint(Vector3 position)
        {
            Gizmos.DrawSphere(position, 0.1f);
        }
        public static void DrawPoint(Vector3 position, Color color)
        {
            Color backupColor = Gizmos.color;
            Gizmos.color = color;
            Gizmos.DrawSphere(position, 0.1f);
            Gizmos.color = backupColor;
        }
        public static void DrawLocalPoint(Transform spaceTransform, Vector3 position)
        {
            Matrix4x4 backupMatrix = Gizmos.matrix;
            Gizmos.matrix = spaceTransform.worldToLocalMatrix;
            DrawPoint(position);
            Gizmos.matrix = backupMatrix;
        }
        public static void DrawLocalPoint(Transform spaceTransform, Vector3 position, Color color)
        {
            Matrix4x4 backupMatrix = Gizmos.matrix;
            Gizmos.matrix = spaceTransform.localToWorldMatrix;
            DrawPoint(position, color);
            Gizmos.matrix = backupMatrix;
        }
        #endregion

        #region Line to point
        public static void DrawLineToPoint(Vector3 startPosition, Vector3 position)
        {
            Gizmos.DrawSphere(position, 0.1f);
            Gizmos.DrawLine(startPosition, position);
        }
        public static void DrawLineToPoint(Vector3 startPosition, Vector3 position, Color color)
        {
            Color backupColor = Gizmos.color;
            Gizmos.color = color;
            Gizmos.DrawSphere(position, 0.1f);
            Gizmos.DrawLine(startPosition, position);
            Gizmos.color = backupColor;
        }
        #endregion

        #region Rect
        public static void DrawRect(Rect rect)
        {
            Gizmos.DrawWireCube(rect.center, (Vector3)(rect.size) + Vector3.forward);
        }

        public static void DrawFilledRect(Rect rect)
        {
            Gizmos.DrawCube(rect.center, (Vector3)(rect.size) + Vector3.forward);
        }

        public static void DrawRect(Rect rect, Color color)
        {
            Color backupColor = Gizmos.color;
            Gizmos.color = color;
            Gizmos.DrawWireCube(rect.center, (Vector3)(rect.size) + Vector3.forward);
            Gizmos.color = backupColor;
        }

        public static void DrawFilledRect(Rect rect, Color color)
        {
            Color backupColor = Gizmos.color;
            Gizmos.color = color;
            Gizmos.DrawCube(rect.center, (Vector3)(rect.size) + Vector3.forward);
            Gizmos.color = backupColor;
        }
        #endregion

        #region Other
        public static void DrawPlane(Plane plane, Vector3 center, Color color)
        {
            Matrix4x4 backupMatrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(center, Quaternion.LookRotation(plane.normal, Vector3.up), Vector3.one);
            Gizmos.color = color;
            Gizmos.DrawCube(Vector3.zero, new Vector3(10, 10, 0.1f));
            Gizmos.matrix = backupMatrix;
        }
        #endregion

        #region AngleRange
        public static void DrawAngleRangeWithAnchor(float anchor, float range, Color color)
        {
            const float length = 10f;
            Gizmos.color = color;
            Vector3 centerPoint = Quaternion.Euler(0, 0, anchor) * Vector3.right * length;
            Vector3 endPointMin = Quaternion.Euler(0, 0, anchor - range / 2f) * Vector3.right * length;
            Vector3 endPointMax = Quaternion.Euler(0, 0, anchor + range / 2f) * Vector3.right * length;
            Gizmos.DrawLine(Vector3.zero, centerPoint);
            Gizmos.DrawLine(Vector3.zero, endPointMin);
            Gizmos.DrawLine(Vector3.zero, endPointMax);
            Gizmos.DrawLine(endPointMin, centerPoint);
            Gizmos.DrawLine(endPointMax, centerPoint);
        }
        #endregion
    }
}