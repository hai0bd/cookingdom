using System.Collections;
using MoreMountains.NiceVibrations;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Utilities
{
    public class SwipeDetectUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public float timeWindowSwipe = 0.5f;
        public float minSwipeDistanceCm = 2f;

        public bool isOnlySwipeHorizontal = true;
        public float swipeHorizontalAngleThreshold = 30f;

        private float _unscaledTimeStartSwipe;
        private Vector2 _screenPosMouseStartFlip;

        /// <summary>
        /// Param: delta movement
        /// </summary>
        public event System.Action<Vector2> onSwipe;

        public void OnPointerDown(PointerEventData eventData)
        {
            _unscaledTimeStartSwipe = Time.unscaledTime;
            _screenPosMouseStartFlip = Input.mousePosition;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            // check if swipe
            Vector2 screenPosMouseEnd = Input.mousePosition;
            const float INCH_TO_CM = 2.54f;
            float dpi = Screen.dpi > 0 ? Screen.dpi : 96; // default DPI is 96
            Vector2 delta = screenPosMouseEnd - _screenPosMouseStartFlip;
            float swipeDistanceCm = delta.magnitude / dpi * INCH_TO_CM;
            float angle = Mathf.Min(Vector2.Angle(Vector2.right, delta), Vector2.Angle(Vector2.left, delta)); // TODO (daivq): rewrite to optimize
            float deltaUnscaledTime = Time.unscaledTime - _unscaledTimeStartSwipe;

            bool isSwipe = swipeDistanceCm > minSwipeDistanceCm
                && (Mathf.Abs(angle) < swipeHorizontalAngleThreshold || !isOnlySwipeHorizontal)
                && deltaUnscaledTime < timeWindowSwipe;

            if (isSwipe) onSwipe?.Invoke(delta);
        }
    }
}