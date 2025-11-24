using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace AnhPD
{
    public class Floating : MonoBehaviour
    {
        public float deltaYMin = 0.001f;
        public float deltaYMax = 0.1f;
        public float duration = 2f;
        float originalY;

        public bool isSmooth = false;

        private void Start()
        {
            originalY = transform.localPosition.y;

            Delay();
        }
        private async void Delay()
        {
            await Task.Delay(Random.Range(0, 3000));
            Tween t = transform.DOLocalMoveY(originalY + Random.Range(deltaYMin, deltaYMax), duration)
            .SetLoops(-1, LoopType.Yoyo);
            if(isSmooth )
            {
                t.SetEase(Ease.Linear);
            }
        }
    }
}

