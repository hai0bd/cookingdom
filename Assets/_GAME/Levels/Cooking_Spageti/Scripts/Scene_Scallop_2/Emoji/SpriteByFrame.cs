using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link.Cooking
{
    public class SpriteByFrame : MonoBehaviour
    {
        [SerializeField] SpriteRenderer spriteRenderer;
        [SerializeField] Sprite[] sprites;
        [SerializeField] float step = 0.1f;
        float time = 0; 
        int index = 0;

        // Update is called once per frame
        void Update()
        {
            time += Time.deltaTime;
            if (time >= step)
            {
                time = 0;
                spriteRenderer.sprite = sprites[index = (index + 1) % sprites.Length];
            }
        }
    }
}