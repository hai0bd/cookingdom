using System.Collections;
using UnityEngine;

namespace Utilities
{
    public static class CameraExtension
    {
        public static Rect GetOrthorWorldViewArea(this Camera camera)
        {
            float height = 2f * camera.orthographicSize;
            float width = height * camera.aspect;
            return new Rect(camera.transform.position.x - width / 2, camera.transform.position.y - height / 2, width, height);
        }
    }
}