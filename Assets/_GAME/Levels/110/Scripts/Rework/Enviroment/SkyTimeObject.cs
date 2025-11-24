using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AnhPD.Fishing
{
    public enum SkyTime
    {
        Morning = 0,
        Afternoon = 1,
        Night = 2,
    }
    public class SkyTimeObject : MonoBehaviour
    {
        [SerializeField] SpriteRenderer spriteRenderer;
        [SerializeField] Sprite[] spites;
        [SerializeField] bool isUIImage;
        [SerializeField] Image img;

        public void ChangeSpriteByTime(SkyTime time)
        {
            if (isUIImage)
            {
                img.sprite = spites[(int)time];
            }
            else
            {
                spriteRenderer.sprite = spites[(int)time];
            }
        }
        [Sirenix.OdinInspector.Button]
        private void Init()
        {
            if (isUIImage)
            {
                img = GetComponent<Image>();
            }
            else
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }
        }
    }

}

