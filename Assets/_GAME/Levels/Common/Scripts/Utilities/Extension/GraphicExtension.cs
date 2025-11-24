using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Utilities
{
    public static class GraphicExtension
    {
        public static void SetA(this SpriteRenderer spriteRenderer, float alpha)
        {
            Color color = spriteRenderer.color;
            color.a = alpha;
            spriteRenderer.color = color;
        }

        public static void SetA(this Graphic graphic, float alpha)
        {
            Color color = graphic.color;
            color.a = alpha;
            graphic.color = color;
        }
    }
}