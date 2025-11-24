using System.Collections;
using UnityEngine;

namespace Utilities
{
    public static class MathExtension
    {
        public static Vector2 GetPerpendicular(this Vector2 v)
        {
            return new Vector2(v.y, -v.x);
        }

        public static float ShortestAngleFromRightVector(this Vector2 vector2)
        {
            return 360 - (Mathf.Atan2(vector2.x, vector2.y) * Mathf.Rad2Deg * Mathf.Sign(vector2.x));
        }

        public static float SignedAngleFromRightVector(this Vector2 vector2)
        {
            return Mathf.Repeat(Mathf.Atan2(vector2.y, vector2.x) * Mathf.Rad2Deg, 360f);
        }

        public static float ModularClamp(float val, float min, float max, float rangemin = -180f, float rangemax = 180f)
        {
            var modulus = Mathf.Abs(rangemax - rangemin);
            if ((val %= modulus) < 0f) val += modulus;
            return Mathf.Clamp(val + Mathf.Min(rangemin, rangemax), min, max);
        }

        private const float AVG_FRAME_RATE = 50f;
        public static void LerpTo(this ref float current, float target, float speed)
        {
            current = Mathf.Lerp(current, target, speed * Time.deltaTime * AVG_FRAME_RATE);
        }
        public static void LerpToWithUnscaledTime(this ref float current, float target, float speed)
        {
            current = Mathf.Lerp(current, target, speed);
        }
        public static void LerpTo(this ref Vector2 current, Vector2 target, float speed)
        {
            current = Vector2.Lerp(current, target, speed * Time.deltaTime * AVG_FRAME_RATE);
        }
        public static void LerpTo(this ref Vector3 current, Vector3 target, float speed)
        {
            current = Vector3.Lerp(current, target, speed * Time.deltaTime * AVG_FRAME_RATE);
        }
        public static void LerpTo(this ref Quaternion current, Quaternion target, float speed)
        {
            current = Quaternion.Slerp(current, target, speed * Time.deltaTime * AVG_FRAME_RATE);
        }
        public static void LerpToWithUnscaledTime(this ref Vector2 current, Vector2 target, float speed)
        {
            current = Vector2.Lerp(current, target, speed);
        }
        public static void LerpToWithUnscaledTime(this ref Vector3 current, Vector3 target, float speed)
        {
            current = Vector3.Lerp(current, target, speed);
        }
        public static void LerpToWithUnscaledTime(this ref Quaternion current, Quaternion target, float speed)
        {
            current = Quaternion.Slerp(current, target, speed);
        }
    }
}