using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Utilities
{
    public class CameraControlRotateAroundObject : MonoBehaviour
    {
        public Transform target;
        public Camera mainCamera;
        public float distanceBetweenCameraAndTarget = 10f;

        [Tooltip("How sensitive the mouse drag to camera rotation")]
        public Vector2 mouseRotateSpeed = new Vector2(3f, 3f);
        [Tooltip("How sensitive the touch drag to camera rotation")]
        public Vector2 touchRotateSpeed = new Vector2(15f, 15f);
        [Tooltip("Smaller positive value means smoother rotation, 1 means no smooth apply")]
        public float slerpValue = 0.25f;
        public enum RotateMethod { Mouse, Touch };
        [Tooltip("How do you like to rotate the camera")]
        public RotateMethod rotateMethod = RotateMethod.Mouse;
        public FloatMulModifierWithRegisteredSource rotateSpeedMul = new FloatMulModifierWithRegisteredSource();

        private Vector2 swipeDirection; //swipe delta vector2
        private Quaternion cameraRot; // store the quaternion after the slerp operation
        private Touch touch;

        private float minXRotAngle = -80; //min angle around x axis
        private float maxXRotAngle = 80; // max angle around x axis

        //Mouse rotation related
        private float rotX; // around x
        private float rotY; // around y

        [Header("Pinch Zoom")]
        public bool isAllowZoom = true;
        public float orthoZoomSpeed = 1f;
        public float perspectiveZoomSpeed = 1f;
        public Vector2 clampFOVZoom = new Vector2(35f, 100f);

#if UNITY_EDITOR
        private void Awake()
        {
            rotateMethod = RotateMethod.Mouse;
        }
#endif

        private void Start()
        {
            Vector3 dir = new Vector3(0, 0, -distanceBetweenCameraAndTarget); //assign value to the distance between the maincamera and the target
            cameraRot = Quaternion.LookRotation(mainCamera.transform.forward);
            rotX  = swipeDirection.y = cameraRot.eulerAngles.x;
            rotY  = cameraRot.eulerAngles.y;
            swipeDirection.x = -cameraRot.eulerAngles.y;
            mainCamera.transform.position = target.position + cameraRot * dir;
            mainCamera.transform.LookAt(target.position);
        }

        // Update is called once per frame
        void Update()
        {
            if (rotateMethod == RotateMethod.Mouse)
            {
                if (Input.GetMouseButton(0))
                {
                    rotX += -Input.GetAxis("Mouse Y") * mouseRotateSpeed.y * rotateSpeedMul.Value; // around X
                    rotY += Input.GetAxis("Mouse X") * mouseRotateSpeed.x * rotateSpeedMul.Value;
                }

                if (rotX < minXRotAngle)
                {
                    rotX = minXRotAngle;
                }
                else if (rotX > maxXRotAngle)
                {
                    rotX = maxXRotAngle;
                }
            }
            else if (rotateMethod == RotateMethod.Touch)
            {
                if (Input.touchCount == 1)
                {
                    touch = Input.GetTouch(0);
                    if (touch.phase == TouchPhase.Began)
                    {
                        //Debug.Log("Touch Began");

                    }
                    else if (touch.phase == TouchPhase.Moved)
                    {
                        swipeDirection -= (touch.deltaPosition * touchRotateSpeed) * Time.deltaTime * rotateSpeedMul.Value;
                    }
                    else if (touch.phase == TouchPhase.Ended)
                    {
                        //Debug.Log("Touch Ended");
                    }
                }
                else if (isAllowZoom && Input.touchCount == 2)
                {
                    // Store both touches.
                    Touch touchZero = Input.GetTouch(0);
                    Touch touchOne = Input.GetTouch(1);

                    // Find the position in the previous frame of each touch.
                    Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                    Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                    // Find the magnitude of the vector (the distance) between the touches in each frame.
                    float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                    float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

                    // Find the difference in the distances between each frame.
                    float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

                    // If the camera is orthographic...
                    if (mainCamera.orthographic)
                    {
                        // Make sure the orthographic size never drops below zero.
                        mainCamera.orthographicSize = Mathf.Max(mainCamera.orthographicSize + deltaMagnitudeDiff * orthoZoomSpeed, 0.1f);
                    }
                    else
                    {
                        // Clamp the field of view to make sure it's between 0 and 180.
                        mainCamera.fieldOfView = Mathf.Clamp(mainCamera.fieldOfView + deltaMagnitudeDiff * perspectiveZoomSpeed, clampFOVZoom.x, clampFOVZoom.y);
                    }
                }

                if (swipeDirection.y < minXRotAngle)
                {
                    swipeDirection.y = minXRotAngle;
                }
                else if (swipeDirection.y > maxXRotAngle)
                {
                    swipeDirection.y = maxXRotAngle;
                }
            }

        }

        private void LateUpdate()
        {
            Vector3 dir = new Vector3(0, 0, -distanceBetweenCameraAndTarget); //assign value to the distance between the maincamera and the target

            Quaternion newQ; // value equal to the delta change of our mouse or touch position
            if (rotateMethod == RotateMethod.Mouse)
            {
                newQ = Quaternion.Euler(rotX, rotY, 0); //We are setting the rotation around X, Y, Z axis respectively
            }
            else
            {
                newQ = Quaternion.Euler(swipeDirection.y, -swipeDirection.x, 0);
            }
            cameraRot = Quaternion.Slerp(cameraRot, newQ, slerpValue);  //let cameraRot value gradually reach newQ which corresponds to our touch
            mainCamera.transform.position = target.position + cameraRot * dir;
            mainCamera.transform.LookAt(target.position);

        }
#if UNITY_EDITOR
        [Header("Editor Tool")]
        [SerializeField] private Vector2 _sphericalCoordEditor = Vector2.zero;
        [SerializeField] private bool _isUpdateCameraTransformOnValidate = false;

        private void OnValidate()
        {
            if (_isUpdateCameraTransformOnValidate)
            {
                UpdateCameraTransformEditor();
            }
        }

        [Sirenix.OdinInspector.Button]
        private void UpdateCameraTransformEditor()
        {
            if (target && mainCamera)
            {
                Vector3 dir = new Vector3(0, 0, -distanceBetweenCameraAndTarget); //assign value to the distance between the maincamera and the target
                Quaternion newQ = Quaternion.Euler(_sphericalCoordEditor.y, -_sphericalCoordEditor.x, 0);
                cameraRot = newQ;
                mainCamera.transform.position = target.position + cameraRot * dir;
                mainCamera.transform.LookAt(target.position);
                UnityEditor.EditorUtility.SetDirty(mainCamera.transform);
            }
        }
#endif
    }
}