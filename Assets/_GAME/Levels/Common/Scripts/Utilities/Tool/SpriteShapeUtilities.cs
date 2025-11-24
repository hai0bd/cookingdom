using System.Collections;
using UnityEngine;
using UnityEngine.U2D;

namespace Utilities
{
    public static class SpriteShapeUtilities
    {
        public static Vector2 GetNearestPointInShape(this SpriteShapeController shapeController, Vector2 point)
            => GetNearestPointInShape(shapeController, point, out var pointIndexStart, out var pointIndexEnd, out Vector2 normal);
        public static Vector2 GetNearestPointInShape(this SpriteShapeController shapeController, Vector2 point, out int nearestSegmentStartIndex, out int nearestSegmentEndIndex, out Vector2 normal)
        {
            Transform spriteShapeTransform = shapeController.transform;

            Vector2 nearestProjectedPoint = point;
            normal = Vector2.up;
            nearestSegmentStartIndex = nearestSegmentEndIndex = -1;

            int pointCount = shapeController.spline.GetPointCount();
            if (pointCount == 0) return point;
            if (pointCount == 1)
            {
                nearestSegmentStartIndex = nearestSegmentEndIndex = 0;
                nearestProjectedPoint = spriteShapeTransform.TransformPoint(shapeController.spline.GetPosition(0));
                return nearestProjectedPoint;
            }

            float minDistance = -1;
            int startIndex, endIndex;
            Vector2 segmentStartPoint, segmentEndPoint;
            float distanceToLine, sqrDistanceToSegment, distanceProjected;
            Vector2 projectedPoint;
            Vector2 directionNorm;
            for (int i = 0; i < pointCount - 1; i++)
            {
                startIndex = i;
                endIndex = i + 1;
                segmentStartPoint = spriteShapeTransform.TransformPoint(shapeController.spline.GetPosition(startIndex));
                segmentEndPoint = spriteShapeTransform.TransformPoint(shapeController.spline.GetPosition(endIndex));
                directionNorm = (segmentEndPoint - segmentStartPoint).normalized;

                GeometryUtilities.ProjectFromPointToLine(point, segmentStartPoint, directionNorm, out distanceToLine, out distanceProjected, out projectedPoint);

                if (distanceProjected < 0)
                {
                    sqrDistanceToSegment = (point - segmentStartPoint).sqrMagnitude;
                    projectedPoint = segmentStartPoint;
                }
                else if (distanceProjected * distanceProjected > (segmentEndPoint - segmentStartPoint).sqrMagnitude)
                {
                    sqrDistanceToSegment = (point - segmentEndPoint).sqrMagnitude;
                    projectedPoint = segmentEndPoint;
                }
                else
                {
                    // nothing to do
                    sqrDistanceToSegment = distanceToLine * distanceToLine;
                }

                if (minDistance < 0 || sqrDistanceToSegment < minDistance * minDistance)
                {
                    minDistance = Mathf.Sqrt(sqrDistanceToSegment);
                    nearestSegmentStartIndex = startIndex;
                    nearestSegmentEndIndex = endIndex;
                    nearestProjectedPoint = projectedPoint;
                    normal = new Vector2(-directionNorm.y, directionNorm.x);
                }
            }

            return nearestProjectedPoint;
        }
    }
}