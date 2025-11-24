using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Utilities
{
    public class TextKanjiFit : Text
    {
        public bool isKanji;

        readonly UIVertex[] m_TempVerts = new UIVertex[4];
        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            if (font == null)
                return;

            // We don't care if we the font Texture changes while we are doing our Update.
            // The end result of cachedTextGenerator will be valid for this instance.
            // Otherwise we can get issues like Case 619238.
            m_DisableFontTextureRebuiltCallback = true;

            Vector2 extents = rectTransform.rect.size;
            float unitsPerPixel = 1 / pixelsPerUnit;

            var settings = GetGenerationSettings(extents);

            // Best fit kanji manually
            if (isKanji && resizeTextForBestFit)
            {
                settings.fontSize = resizeTextMaxSize;
                settings.resizeTextForBestFit = false;
                float height = cachedTextGenerator.GetPreferredHeight(m_Text, settings) / pixelsPerUnit;
                if (height > rectTransform.rect.height)
                {
                    int min = resizeTextMinSize;
                    int max = resizeTextMaxSize;
                    while (min + 1 < max)
                    {
                        settings.fontSize = (min + max) / 2;
                        height = cachedTextGenerator.GetPreferredHeight(m_Text, settings) / pixelsPerUnit;
                        if (height > rectTransform.rect.height)
                        {
                            max = settings.fontSize;
                        }
                        else if (height < rectTransform.rect.height)
                        {
                            min = settings.fontSize;
                        }
                        else
                        {
                            min = settings.fontSize;
                            break;
                        }
                    }
                    settings.fontSize = min;
                }
            }

            cachedTextGenerator.PopulateWithErrors(text, settings, gameObject);

            // Apply the offset to the vertices
            IList<UIVertex> verts = cachedTextGenerator.verts;
            //Last 4 verts are always a new line... (\n)
            //int vertCount = verts.Count - 4;
            int vertCount = verts.Count;

            Vector2 roundingOffset = (vertCount > 0) ? new Vector2(verts[0].position.x, verts[0].position.y) * unitsPerPixel : Vector2.zero;
            roundingOffset = PixelAdjustPoint(roundingOffset) - roundingOffset;
            toFill.Clear();
            if (roundingOffset != Vector2.zero)
            {
                for (int i = 0; i < vertCount; ++i)
                {
                    int tempVertsIndex = i & 3;
                    m_TempVerts[tempVertsIndex] = verts[i];
                    m_TempVerts[tempVertsIndex].position *= unitsPerPixel;
                    m_TempVerts[tempVertsIndex].position.x += roundingOffset.x;
                    m_TempVerts[tempVertsIndex].position.y += roundingOffset.y;
                    if (tempVertsIndex == 3)
                        toFill.AddUIVertexQuad(m_TempVerts);
                }
            }
            else
            {
                for (int i = 0; i < vertCount; ++i)
                {
                    int tempVertsIndex = i & 3;
                    m_TempVerts[tempVertsIndex] = verts[i];
                    m_TempVerts[tempVertsIndex].position *= unitsPerPixel;
                    if (tempVertsIndex == 3)
                        toFill.AddUIVertexQuad(m_TempVerts);
                }
            }

            m_DisableFontTextureRebuiltCallback = false;
        }
    }
}