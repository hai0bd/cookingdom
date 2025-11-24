using System.Collections;
using UnityEngine;

namespace Utilities
{
    public static class RectExtension
    {
        public static float ClampX(this Rect rect, float value) => Mathf.Clamp(value, rect.xMin, rect.xMax);
        public static float ClampY(this Rect rect, float value) => Mathf.Clamp(value, rect.yMin, rect.yMax);

        public static Vector2 Clamp(this Rect rect, Vector2 value)
            => new Vector3(
                Mathf.Clamp(value.x, rect.xMin, rect.xMax),
                Mathf.Clamp(value.y, rect.yMin, rect.yMax)
                );

        public static Vector2 ClampOutside(this Rect rect, Vector2 value)
            => new Vector3(
                (value.x < rect.center.x) ? Mathf.Min(value.x, rect.xMin) : Mathf.Max(value.x, rect.xMax),
                (value.y < rect.center.y) ? Mathf.Min(value.y, rect.yMin) : Mathf.Max(value.y, rect.yMax)
                );

        public static Vector3 Clamp(this Rect rect, Vector3 value)
            => new Vector3(
                Mathf.Clamp(value.x, rect.xMin, rect.xMax),
                Mathf.Clamp(value.y, rect.yMin, rect.yMax),
                value.z
                );

        public static bool IsContain(this Rect rect, Vector2 position)
            => rect.xMin < position.x && position.x < rect.xMax && rect.yMin < position.y && position.y < rect.yMax;
        public static bool IsNotContain(this Rect rect, Vector2 position)
            => rect.xMin > position.x || position.x > rect.xMax || rect.yMin > position.y || position.y > rect.yMax;

        public static Vector2 GetRandomInside(this Rect rect) => new Vector2(
            UnityEngine.Random.Range(rect.xMin, rect.xMax),
            UnityEngine.Random.Range(rect.yMin, rect.yMax)
            );

        public static Vector2 GetRandomInside(this Rect rect, int seed, Vector2Int data) => new Vector2(
            RandomUtility.GetRange(rect.xMin, rect.xMax, data.x, seed),
            RandomUtility.GetRange(rect.yMin, rect.yMax, data.y, seed)
            );

        public static void DrawGizmo(this Rect rect, Color color)
        {
            Gizmos.color = color;
            Gizmos.DrawWireCube(rect.center, new Vector3(rect.size.x, rect.size.y, 0.1f));
        }

        public static Vector2 Repeat(this Rect rect, Vector2 value)
            => new Vector2(
                Mathf.Repeat(value.x - rect.xMin, rect.width) + rect.xMin,
                Mathf.Repeat(value.y - rect.yMin, rect.height) + rect.yMin
                );


        public static Rect Expand(this Rect rect, Vector2 expandSize) => Expand(rect, expandSize.x, expandSize.y);
        public static Rect Expand(this Rect rect, float expandX, float expandY)
        {
            return new Rect(rect.xMin - expandX, rect.yMin - expandY, rect.width + expandX * 2, rect.height + expandY * 2);
        }
        public static Rect Shrink(this Rect rect, Vector2 shrinkSize) => Shrink(rect, shrinkSize.x, shrinkSize.y);
        public static Rect Shrink(this Rect rect, float shrinkX, float shrinkY)
        {
            if (shrinkX * 2 > rect.width) shrinkX = rect.width / 2;
            if (shrinkY * 2 > rect.height) shrinkY = rect.height / 2;
            return new Rect(rect.xMin + shrinkX, rect.yMin + shrinkY, rect.width - shrinkX * 2, rect.height - shrinkY * 2);
        }

        public static void SetLeft(this RectTransform @this, float val)
        {
            @this.offsetMin = new Vector2(val, @this.offsetMin.y);
        }
        public static void SetRight(this RectTransform @this, float val)
        {
            @this.offsetMax = new Vector2(-val, @this.offsetMin.y);
        }
        public static void SetTop(this RectTransform @this, float val)
        {
            @this.offsetMax = new Vector2(@this.offsetMin.x, -val);
        }
        public static void SetBottom(this RectTransform @this, float val)
        {
            @this.offsetMin = new Vector2(@this.offsetMin.x, val);
        }
        public static void Set4Bounds(this RectTransform @this, float left, float right, float top, float bottom)
        {
            @this.offsetMin = new Vector2(left, bottom);
            @this.offsetMax = new Vector2(right, top);
        }

        public static float GetLeft(this RectTransform @this)
        {
            return @this.offsetMin.x;
        }
        public static float GetRight(this RectTransform @this)
        {
            return -@this.offsetMax.x;
        }
        public static float GetTop(this RectTransform @this)
        {
            return -@this.offsetMax.y;
        }
        public static float GetBottom(this RectTransform @this)
        {
            return @this.offsetMin.y;
        }

        /// <summary>
        /// Return 4 corner of the rect in counter-clockwise order, top-right first
        /// </summary>
        public static Vector2[] GetCorners(this Rect @this)
        {
            Vector2[] corners = new Vector2[4];
            corners[0] = new Vector2(@this.xMax, @this.yMax); // top-right
            corners[1] = new Vector2(@this.xMin, @this.yMax); // top-left
            corners[2] = new Vector2(@this.xMin, @this.yMin); // bottom-left
            corners[3] = new Vector2(@this.xMax, @this.yMin); // bottom-right
            return corners;
        }

        public static Vector2[] GetCorners(this Rect @this, float rotateAngle)
        {
            float cos = Mathf.Cos(rotateAngle * Mathf.Deg2Rad);
            float sin = Mathf.Sin(rotateAngle * Mathf.Deg2Rad);

            Vector2[] corners = new Vector2[4];
            corners[0] = new Vector2(@this.xMax * cos - @this.yMax * sin, @this.xMax * sin + @this.yMax * cos); // top-right
            corners[1] = new Vector2(@this.xMin * cos - @this.yMax * sin, @this.xMin * sin + @this.yMax * cos); // top-left
            corners[2] = new Vector2(@this.xMin * cos - @this.yMin * sin, @this.xMin * sin + @this.yMin * cos); // bottom-left
            corners[3] = new Vector2(@this.xMax * cos - @this.yMin * sin, @this.xMax * sin + @this.yMin * cos); // bottom-right

            return corners;
        }

        public static Rect CalculateNewBoundingRectByRotation(this Rect @this, float rotateAngle)
        {
            float cos = Mathf.Cos(rotateAngle * Mathf.Deg2Rad);
            float sin = Mathf.Sin(rotateAngle * Mathf.Deg2Rad);

            Vector2[] rotatedCorners = new Vector2[4];
            rotatedCorners[0] = new Vector2(@this.xMax * cos - @this.yMax * sin, @this.xMax * sin + @this.yMax * cos); // top-right
            rotatedCorners[1] = new Vector2(@this.xMin * cos - @this.yMax * sin, @this.xMin * sin + @this.yMax * cos); // top-left
            rotatedCorners[2] = new Vector2(@this.xMin * cos - @this.yMin * sin, @this.xMin * sin + @this.yMin * cos); // bottom-left
            rotatedCorners[3] = new Vector2(@this.xMax * cos - @this.yMin * sin, @this.xMax * sin + @this.yMin * cos); // bottom-right

            float minX = Mathf.Min(rotatedCorners[0].x, rotatedCorners[1].x, rotatedCorners[2].x, rotatedCorners[3].x);
            float maxX = Mathf.Max(rotatedCorners[0].x, rotatedCorners[1].x, rotatedCorners[2].x, rotatedCorners[3].x);
            float minY = Mathf.Min(rotatedCorners[0].y, rotatedCorners[1].y, rotatedCorners[2].y, rotatedCorners[3].y);
            float maxY = Mathf.Max(rotatedCorners[0].y, rotatedCorners[1].y, rotatedCorners[2].y, rotatedCorners[3].y);

            return new Rect(minX, minY, maxX - minX, maxY - minY);
        }
    }
}