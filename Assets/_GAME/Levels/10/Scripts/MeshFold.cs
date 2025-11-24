using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UIElements;

namespace Utilities
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class MeshFold : MonoBehaviour
    {
        private static readonly Vector3 MainNormal = Vector3.forward;
        private static readonly Vector3 BackNormal = Vector3.back;

        public Vector2 bound;
        public Transform pinPointTransform;
        public Transform dragPointTransform;
        public Transform detachPointTransform;
        private Vector2 currentPinPoint;
        private Vector2 currentDragPoint;
        public bool isAutoUpdateMeshWhenPinPointOrDragPointChange = false;

        public bool isCalculateAttachRatio = false;
        [ShowInInspector, ReadOnly]
        public float AttachRatio { get; private set; } = -1f;

        private Vector2 _foldLineCenter;
        private Vector2 _foldLineDirectionNorm;
        public float GetDistancToFoldLine(Vector2 point) => GeometryUtilities.CalculateDistanceFromPointToLine(point, _foldLineCenter, _foldLineDirectionNorm);
        public Vector2 GetProjectedPointToFoldLine(Vector2 point) => GeometryUtilities.CalculateProjectFromPointToLine(point, _foldLineCenter, _foldLineDirectionNorm);
        public Vector2 GetMirrorPointByFoldLine(Vector2 point) => GeometryUtilities.CalculateMirroredPoint(point, _foldLineCenter, _foldLineDirectionNorm);
        public bool IsPointInsideBound(Vector2 point) => Mathf.Abs(point.x) <= bound.x / 2 && Mathf.Abs(point.y) <= bound.y / 2;
        public bool IsPointInsideExtrudedBound(Vector2 point, float extrudedRadius) => Mathf.Abs(point.x) <= bound.x / 2 + extrudedRadius && Mathf.Abs(point.y) <= bound.y / 2 + extrudedRadius;
        public Vector2 ClampPointToBound(Vector2 point, float extrudedRadius = 0f) => new Vector2(
            Mathf.Clamp(point.x, -bound.x / 2 - extrudedRadius, bound.x / 2 + extrudedRadius),
            Mathf.Clamp(point.y, -bound.y / 2 - extrudedRadius, bound.y / 2 + extrudedRadius)
            );
        
        public Plane MeshPlane { get; private set; }
        //public Vector2 GetProjectedScreenPosition(Vector2 screenPos)
        //{
        //    Camera mainCam = Camera.main;
        //    Ray ray = mainCam.ScreenPointToRay(screenPos);
        //    if (MeshPlane.Raycast(ray, out float distanceIntersect))
        //    {
        //        return ray.GetPoint(distanceIntersect);
        //    }
        //    else
        //    {
        //        throw new System.Exception("MeshFold: GetProjectedScreenPosition: Raycast failed");
        //    }
        //}
        //public Vector2 GetProjectedPosition(Vector3 worldPosition) 
        //{
        //    Camera mainCam = Camera.main;
        //    Vector3 direction = mainCam.orthographic ? mainCam.transform.forward : (worldPosition - mainCam.transform.position).normalized;

        //    Ray ray = new Ray(worldPosition, direction);
        //    float distanceIntersect;
        //    if (MeshPlane.Raycast(ray, out distanceIntersect))
        //    {
        //        return ray.GetPoint(distanceIntersect);
        //    }
        //    else
        //    {
        //        ray = new Ray(worldPosition, -direction);
        //        if (MeshPlane.Raycast(ray, out distanceIntersect))
        //        {
        //            return ray.GetPoint(distanceIntersect);
        //        }
        //        else
        //        {
        //            return worldPosition;
        //        }
        //    }
        //}

        private void Start()
        {
            GenerateMesh();
        }

        private void Update()
        {
            if (isAutoUpdateMeshWhenPinPointOrDragPointChange && (currentPinPoint != (Vector2)pinPointTransform.localPosition || currentDragPoint != (Vector2)dragPointTransform.localPosition))
            {
                GenerateMesh();
            }
        }

        [Sirenix.OdinInspector.Button]
        public bool GenerateMesh()
        {
            MeshPlane = new Plane(-transform.forward, transform.position);
            currentPinPoint = pinPointTransform.localPosition;
            currentDragPoint = dragPointTransform.localPosition;
            return GenerateMesh(bound, currentPinPoint, currentDragPoint);
        }

        /// <summary>
        /// Return if mesh is still attached and can be updated
        /// </summary>
        public bool GenerateMesh(Vector2 bound, Vector2 pinPoint, Vector2 dragPoint)
        {
            _foldLineCenter = (pinPoint + dragPoint) / 2;
            _foldLineDirectionNorm = (dragPoint - pinPoint).normalized.GetPerpendicular();

            Vector2 foldLineStart = _foldLineCenter - _foldLineDirectionNorm * (bound.x + bound.y) * 2f; // ensure the line is long enough
            Vector2 foldLineEnd = _foldLineCenter + _foldLineDirectionNorm * (bound.x + bound.y) * 2f; // ensure the line is long enough

            Vector2[] intersection = Utilities.GeometryUtilities.CalculateIntersections(bound, foldLineStart, foldLineEnd);
            if (intersection == null || (intersection[0] - intersection[1]).sqrMagnitude < float.Epsilon
                || (detachPointTransform && ((Vector2)detachPointTransform.localPosition - dragPoint).sqrMagnitude > ((Vector2)detachPointTransform.localPosition - pinPoint).sqrMagnitude))
            {
                //Debug.Log("Bandage is detached");
                if (isCalculateAttachRatio) AttachRatio = 0f;
                return false;
            }
            else
            {
                List<Vector2> pointAttached = new List<Vector2>();
                List<Vector2> pointDetached = new List<Vector2>();

                float halfWidth = bound.x / 2;
                float halfHeight = bound.y / 2;

                Vector2[] rectangleCorners = new Vector2[4]
                {
                    new Vector2(-halfWidth, -halfHeight),
                    new Vector2(halfWidth, -halfHeight),
                    new Vector2(halfWidth, halfHeight),
                    new Vector2(-halfWidth, halfHeight)
                };

                for (int i = 0; i < rectangleCorners.Length; i++)
                {
                    if ((rectangleCorners[i] - pinPoint).sqrMagnitude < (rectangleCorners[i] - dragPoint).sqrMagnitude)
                    {
                        pointDetached.Add(rectangleCorners[i]);
                    }
                    else
                    {
                        pointAttached.Add(rectangleCorners[i]);
                    }
                }

                pointAttached.Add(intersection[0]);
                pointAttached.Add(intersection[1]);
                pointDetached.Add(intersection[0]);
                pointDetached.Add(intersection[1]);

                Mesh mesh = new Mesh();

                int verticeIndexCounter = 0;
                Vector3[] vertices = new Vector3[8];
                Vector3[] normals = new Vector3[8];
                Vector2[] uvs = new Vector2[8];

                Vector3 position;

                SortPositionByClockwise(pointAttached);
                for (int i = 0; i < pointAttached.Count; i++)
                {
                    position = pointAttached[i];
                    vertices[verticeIndexCounter] = position;
                    normals[verticeIndexCounter] = MainNormal;
                    uvs[verticeIndexCounter] = PositionToUV(bound, position);
                    verticeIndexCounter += 1;
                }

                int[] triangles = new int[(pointAttached.Count - 2) * 3 + (pointDetached.Count - 2) * 3];
                int triangleIndexCounter = 0;
                int offsetPointIndex = 0;
                for (int i = 0; i < pointAttached.Count - 2; i++)
                {
                    triangles[triangleIndexCounter] = offsetPointIndex;
                    triangleIndexCounter += 1;
                    triangles[triangleIndexCounter] = offsetPointIndex + (i + 1);
                    triangleIndexCounter += 1;
                    triangles[triangleIndexCounter] = offsetPointIndex + (i + 2);
                    triangleIndexCounter += 1;
                }

                SortPositionByClockwise(pointDetached);
                for (int i = 0; i < pointDetached.Count; i++)
                {
                    position = pointDetached[i];
                    vertices[verticeIndexCounter] = GeometryUtilities.CalculateMirroredPoint(position, _foldLineCenter, _foldLineDirectionNorm);
                    normals[verticeIndexCounter] = BackNormal;
                    uvs[verticeIndexCounter] = PositionToUV(bound, position);
                    verticeIndexCounter += 1;
                }
                offsetPointIndex = pointAttached.Count;
                for (int i = 0; i < pointDetached.Count - 2; i++)
                {
                    triangles[triangleIndexCounter] = offsetPointIndex;
                    triangleIndexCounter += 1;
                    triangles[triangleIndexCounter] = offsetPointIndex + (i + 1);
                    triangleIndexCounter += 1;
                    triangles[triangleIndexCounter] = offsetPointIndex + (i + 2);
                    triangleIndexCounter += 1;
                }

                //Vector3 localPos = transform.position;
                //for (int i=0;i<vertices.Length;i++)
                //{
                //    vertices[i] = vertices[i] - localPos;
                //}

                mesh.vertices = vertices;
                mesh.uv = uvs;
                mesh.triangles = triangles;
                mesh.normals = normals;

                GetComponent<MeshFilter>().mesh = mesh;

#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(GetComponent<MeshFilter>());
#endif

                if (isCalculateAttachRatio)
                {
                    float attachedArea = GeometryUtilities.CalculateAreaPolygon(pointAttached.ToArray());
                    float detachedArea = GeometryUtilities.CalculateAreaPolygon(pointDetached.ToArray());
                    AttachRatio = attachedArea / (attachedArea + detachedArea);
                }

                return true;
            }

            void SortPositionByClockwise(List<Vector2> points)
            {
                Vector2 sum = Vector2.zero;
                foreach (Vector2 point in points)
                {
                    sum += point;
                }
                Vector2 center = sum / points.Count;

                points.Sort((a, b) => ComparePoints(a, b, center));

                int ComparePoints(Vector2 a, Vector2 b, Vector2 center)
                {
                    float angleA = Mathf.Atan2(a.y - center.y, a.x - center.x);
                    float angleB = Mathf.Atan2(b.y - center.y, b.x - center.x);

                    return angleA.CompareTo(angleB);
                }
            }

            Vector2 PositionToUV(Vector2 bound, Vector2 position)
                => new Vector2(
                    Mathf.InverseLerp(-bound.x / 2, bound.x / 2, position.x),
                    Mathf.InverseLerp(-bound.y / 2, bound.y / 2, position.y)
                    );
        }

#if UNITY_EDITOR
        [Header("Editor Tool")]
        [SerializeField] private Sprite sprite;
        [SerializeField] private bool isAlwaysDrawGizmo = false;

        private void OnDrawGizmosSelected()
        {
            if (!isAlwaysDrawGizmo) DrawGizmo();
        }

        private void OnDrawGizmos()
        {
            if (isAlwaysDrawGizmo) DrawGizmo();
        }

        private void DrawGizmo()
        {
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);

            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(bound.x, bound.y, 0));

            Vector2 pinPoint = pinPointTransform ? pinPointTransform.localPosition : Vector2.zero;
            Vector2 dragPoint = dragPointTransform ? dragPointTransform.localPosition : Vector2.zero;

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(pinPoint, 0.1f);
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(dragPoint, 0.1f);
            if (detachPointTransform)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawSphere(detachPointTransform.localPosition, 0.1f);
            }

            Vector2 centerBetweenPinPointAndDragPoint = (pinPoint + dragPoint) / 2;
            Vector2 foldDirectionNorm = (dragPoint - pinPoint).normalized.GetPerpendicular();

            Vector2 foldLineStart = centerBetweenPinPointAndDragPoint - foldDirectionNorm * (bound.x + bound.y) * 2f; // ensure the line is long enough
            Vector2 foldLineEnd = centerBetweenPinPointAndDragPoint + foldDirectionNorm * (bound.x + bound.y) * 2f; // ensure the line is long enough

            Vector2[] intersection = Utilities.GeometryUtilities.CalculateIntersections(bound, foldLineStart, foldLineEnd);
            if (intersection == null || (intersection[0] - intersection[1]).sqrMagnitude < float.Epsilon)
            {
                Gizmos.color = Color.magenta;
            }
            else
            {
                Gizmos.color = Color.blue;

                Gizmos.DrawSphere(intersection[0], 0.1f);
                Gizmos.DrawSphere(intersection[1], 0.1f);
            }
            Gizmos.DrawLine(foldLineStart, foldLineEnd);

            List<Vector2> pointAttached = new List<Vector2>();
            List<Vector2> pointDetached = new List<Vector2>();

            float halfWidth = bound.x / 2;
            float halfHeight = bound.y / 2;

            Vector2[] rectangleCorners = new Vector2[4]
            {
                    new Vector2(-halfWidth, -halfHeight),
                    new Vector2(halfWidth, -halfHeight),
                    new Vector2(halfWidth, halfHeight),
                    new Vector2(-halfWidth, halfHeight)
            };

            for (int i = 0; i < rectangleCorners.Length; i++)
            {
                if ((rectangleCorners[i] - pinPoint).sqrMagnitude < (rectangleCorners[i] - dragPoint).sqrMagnitude)
                {
                    pointDetached.Add(rectangleCorners[i]);
                }
                else
                {
                    pointAttached.Add(rectangleCorners[i]);
                }
            }

            Gizmos.color = Color.green;
            for (int i = 0; i < pointAttached.Count; i++)
            {
                Gizmos.DrawSphere(pointAttached[i], 0.1f);
            }

            Gizmos.color = Color.red;
            for (int i = 0; i < pointDetached.Count; i++)
            {
                Gizmos.DrawSphere(pointDetached[i], 0.1f);
            }
        }

        [Sirenix.OdinInspector.Button]
        private void AdjustBoundBySpriteEditor()
        {
            bound = new Vector2(sprite.texture.width, sprite.texture.height) / sprite.pixelsPerUnit;
            UnityEditor.EditorUtility.SetDirty(this);
        }

        [Sirenix.OdinInspector.Button]
        private void ResetDragPinDetachPointEditor()
        {
            pinPointTransform.localPosition = new Vector3(bound.x / 2f - 0.001f, bound.y / 2f - 0.001f, 0f);
            dragPointTransform.localPosition = new Vector3(bound.x / 2f - 0.002f, bound.y / 2f - 0.002f, 0f);
            if (detachPointTransform) detachPointTransform.localPosition = new Vector3(-bound.x / 2f + 0.001f, -bound.y / 2f + 0.001f, 0f);
            UnityEditor.EditorUtility.SetDirty(pinPointTransform);
            UnityEditor.EditorUtility.SetDirty(dragPointTransform);
            if (detachPointTransform) UnityEditor.EditorUtility.SetDirty(detachPointTransform);
        }
#endif
    }
}