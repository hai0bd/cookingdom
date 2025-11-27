using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hai.Cooking.NewTest
{
    public class MovingTween : MonoBehaviour
    {
        public enum DirectionType
        {
            Up,
            Down,
            Left,
            Right
        }

        [SerializeField] private bool onActive, onDisable;
        [SerializeField] private DirectionType directionType;
        [SerializeField] private float distance = 0f;
        [SerializeField] private float duration = 0f;
        [SerializeField] private float scale = 1f;
        [SerializeField] private float delay = 0f;

        private Vector2 originalPos;
        private Vector3 targetPos;
        private Vector2 direction;

        private void Awake()
        {
            StepCook.OnLevelEnd += OnEnd;

            if (onActive)
            {
                targetPos = transform.position;
                switch (directionType)
                {
                    case DirectionType.Up:
                        direction = Vector2.up;
                        break;
                    case DirectionType.Down:
                        direction = Vector2.down;
                        break;
                    case DirectionType.Left:
                        direction = Vector2.left;
                        break;
                    case DirectionType.Right:
                        direction = Vector2.right;
                        break;
                }

            }
        }

        // Start is called before the first frame update
        void Start()
        {
                StartMoveTween();
        }

        private void StartMoveTween()
        {
            direction = direction.normalized;
            Vector3 startPos = targetPos + distance * new Vector3(direction.x, direction.y);

            transform.position = startPos;
            transform.localScale = Vector3.zero;

            transform.DOMove(targetPos, duration).SetEase(Ease.OutBack).SetDelay(delay);
            transform.DOScale(scale, duration / 3).SetEase(Ease.InOutQuart).SetDelay(delay);

            //transform.DORotate(new Vector3(0, 0, -360), duration, RotateMode.FastBeyond360).SetEase(Ease.OutBack);
        }

        public void OnEnd()
        {
            if (onDisable)
            {
                direction = direction.normalized;
                Vector3 endPos = targetPos + distance * new Vector3(direction.x, direction.y);
                transform.DOMove(endPos, duration).SetEase(Ease.InBack).SetDelay(delay);
                transform.DOScale(0f, duration).SetEase(Ease.InBack).SetDelay(delay);
            }
            StepCook.OnLevelEnd -= OnEnd;
        }
    }
}