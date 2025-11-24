using UnityEngine;

namespace Utilities
{
    public class AdjustVolumeByTouchSpeed : MonoBehaviour
    {
        public AudioSource audioSource;
        public bool useViewportSpace = false;
        public Rect affectArea = new Rect(0, 0, 1, 1);
        public float speedMultiplier = 1f / 1000f;
        //private Vector2[] previousPositions = new Vector2[10];
        private Vector2 previousMousePosition = Vector2.zero;
        public float totalSpeed = 0f;
        public AnimationCurve volumeMapCurve;
        [Range(0f, 1f)] public float lerpVolume = 0.1f;

        private void OnDisable()
        {
            audioSource.volume = 0f;
        }

        private void OnEnable()
        {
            audioSource.volume = 0f;
        }

        void Update()
        {
            if (Time.timeScale <= Mathf.Epsilon)
            {
                this.totalSpeed = 0f;
                audioSource.volume = 0f;
                return;
            }

            float totalSpeed = 0.0f;

            if (Input.GetMouseButtonDown(0))
            {
                previousMousePosition = Input.mousePosition;
            }
            else if (Input.GetMouseButton(0))
            {
                Vector2 mousePosition = Input.mousePosition;

                if (useViewportSpace)
                {
                    Vector2 mouseViewportPos = Camera.main.ScreenToViewportPoint(mousePosition);
                    if (affectArea.Contains(mouseViewportPos))
                    {
                        Vector2 speed = new Vector2(
                            Mathf.Abs(mousePosition.x - previousMousePosition.x) / Screen.width,
                            Mathf.Abs(mousePosition.y - previousMousePosition.y) / Screen.width
                        ) / Time.deltaTime;

                        totalSpeed += speed.magnitude;
                    }
                }
                else
                {
                    Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePosition);

                    if (affectArea.Contains(new Vector2(mouseWorldPos.x, mouseWorldPos.y)))
                    {
                        Vector2 speed = new Vector2(
                            Mathf.Abs(mousePosition.x - previousMousePosition.x) / Screen.width,
                            Mathf.Abs(mousePosition.y - previousMousePosition.y) / Screen.width
                        ) / Time.deltaTime;

                        totalSpeed += speed.magnitude;
                    }
                }

                previousMousePosition = mousePosition;
            }

            audioSource.volume = Mathf.Lerp(audioSource.volume, volumeMapCurve.Evaluate(totalSpeed * speedMultiplier), lerpVolume);
            this.totalSpeed = totalSpeed;
        }

        private void OnDrawGizmosSelected()
        {
            GizmoUtility.DrawRect(affectArea, Color.green);
        }
    }
}