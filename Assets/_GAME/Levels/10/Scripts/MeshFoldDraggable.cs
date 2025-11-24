using DG.Tweening;
using MoreMountains.NiceVibrations;
using System.Collections;
using UnityEngine;

namespace Utilities
{
    public class MeshFoldDraggable : MonoBehaviour
    {
        //public float EXTRUDE_DRAGABLE_AREA_SIZE = 99f; // ensure the dragable area is big enough to cover the whole screen
        //public float MARGIN_DRAGABLE_AREA = 0.01f; // ensure not drag too near the pin point, causing NaN fold line

        //public enum DragSpaceHorizontal { LEFT = 0, RIGHT = 1 }
        //public enum DragSpaceVertical { BOTTOM = 0, TOP = 1 }

        //[EnumMask(typeof(DragSpaceHorizontal))] public int dragableSpaceHorizontalMask = 0;
        //[EnumMask(typeof(DragSpaceVertical))] public int dragableSpaceVerticalMask = 0;

        //private Rect _localDragableArea;
        //private Rect _localBlockArea;

        public MeshFold meshFold;
        public float mainDragableAngle = 0f;
        public float rangeDragableAngle = 360f;
        public float blockRadiusFromPinPoint = 0.1f;
        public float extrudeTouchDragableRadius = 0.25f;

        [Tooltip("Set it >0 to prevent detach")]
        public float blockDetachPointRadius = -1f;

        public BoolModifierWithRegisteredSource IsPreventDrag { get; private set; }
        public MeshFoldDraggable[] requiredMeshesDetached = null;

        public bool IsDragging { get; private set; } = false;
        public bool IsDetached { get; private set; } = false;

        [Tooltip("Setting this from 1 to 0 to make it pull harder over pull time")]
        public AnimationCurve dampingMulByMouseDistanceTraveledInScaledViewportSpace = AnimationCurve.Constant(0, 1, 1f);
        private float _mouseDistanceTraveledInScaledViewportSpace;
        private Vector2 _prevMouseScreenPosition;
        private Vector2 _dampedMouseScreenPosition;
        private Vector2 _offsetMouseScreenPosition;

        [Header("Vibrate")]
        public float vibrateIntervalByMouseDistanceTraveledInScaledViewportSpace = 0.1f;
        public HapticTypes vibrateType = HapticTypes.LightImpact;
        private float _prevVibrateTime;

        [Header("Sfx")]
        public AudioSource audioSourceDragging;
        public AnimationCurve volumeMapToDeltaLocalDragPointChangedDistance = AnimationCurve.Linear(0, 0, 1, 1);
        [Range(0f, 1f)] public float lerpVolume = 0.1f;
        
        public event System.Action<MeshFoldDraggable> onStartDrag = null;
        public event System.Action<MeshFoldDraggable> onEndDrag = null;
        public event System.Action<MeshFoldDraggable> onDetached = null;

        [Header("Fall")]
        public Vector2 velocityFallRange = new Vector2(7f, 8f);
        public Vector2 constForce = new Vector2(0f, -50f);
        public float positionYDisableObject = -20f;

        [Header("Shake")]
        public float shakeMagnitude = 0.1f;
        public float shakeDuration = 0.1f;
        public int shakeVibrato = 10;
        private Tween _shakeTween;

        private void Awake()
        {
            IsPreventDrag ??= new BoolModifierWithRegisteredSource(OnChangedIsPreventDrag);
        }

        public void ManualSetUpIsPreventDrag()
        {
            IsPreventDrag = new BoolModifierWithRegisteredSource(OnChangedIsPreventDrag);
        }
        
        public void SetUpNew()
        {
            IsDetached = false;
        }
        
        private void OnChangedIsPreventDrag()
        {
            if (IsPreventDrag.Value)
            {
                if (IsDragging)
                {
                    OnEndDrag();
                }
            }
        }

        private void Update()
        {
            if (IsDetached) return;
            if (IsDragging)
            {
                if (Input.GetMouseButton(0))
                {
                    OnDragging();
                }
                else
                {
                    OnEndDrag();
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (_shakeTween != null && _shakeTween.IsActive()) _shakeTween.Complete();

                    Camera mainCam = Camera.main;

                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    float distanceIntersect = 0f;
                    meshFold.MeshPlane.Raycast(ray, out distanceIntersect);
                    Vector3 dragPos = ray.GetPoint(distanceIntersect);

                    Vector3 localDragPos = meshFold.transform.InverseTransformPoint(dragPos);

                    if (Vector2.SqrMagnitude(meshFold.dragPointTransform.localPosition - localDragPos) > Vector2.SqrMagnitude(meshFold.pinPointTransform.localPosition - localDragPos)) // must be on the drag side to start drag
                    {
                        if (meshFold.GetDistancToFoldLine(localDragPos) < extrudeTouchDragableRadius)
                        {
                            Vector2 projectedPointToFoldLine = meshFold.GetProjectedPointToFoldLine(localDragPos);
                            localDragPos = projectedPointToFoldLine + (projectedPointToFoldLine - (Vector2)localDragPos).normalized * (blockRadiusFromPinPoint + 0.01f);
                            TryDrag();
                        }
                    }
                    else
                    { 
                        TryDrag();
                    }

                    void TryDrag()
                    {
                        if (IsPreventDrag.Value) return;

                        Vector2 localPinPos = meshFold.GetMirrorPointByFoldLine(localDragPos);
                        if (meshFold.IsPointInsideExtrudedBound(localPinPos, extrudeTouchDragableRadius))
                        {
                            if (!meshFold.IsPointInsideBound(localPinPos))
                            {
                                localPinPos = meshFold.ClampPointToBound(localPinPos, -0.01f);

                                if (Vector2.SqrMagnitude((Vector2)meshFold.dragPointTransform.localPosition - localPinPos) < Vector2.SqrMagnitude((Vector2)meshFold.pinPointTransform.localPosition - localPinPos))
                                {
                                    // in some special case, the pin point is clamp to the origin drag point side, causing bug
                                    // so force use current pin point and drag point
                                    localPinPos = meshFold.pinPointTransform.localPosition;
                                }
                            }

                            bool isCanDrag = true;
                            if (requiredMeshesDetached != null)
                            {
                                foreach (var item in requiredMeshesDetached)
                                {
                                    if (!item.IsDetached)
                                    {
                                        item.Shake();
                                        isCanDrag = false;
                                    }
                                }
                            }
                            if (!isCanDrag) return;

                            if (audioSourceDragging)
                            {
                                audioSourceDragging.Play();
                                audioSourceDragging.volume = 0f;
                            }

                            localDragPos = meshFold.GetMirrorPointByFoldLine(localPinPos);

                            meshFold.pinPointTransform.localPosition = localPinPos;
                            meshFold.dragPointTransform.localPosition = localDragPos;

                            IsDragging = true;

                            _prevMouseScreenPosition = Input.mousePosition;
                            _dampedMouseScreenPosition = Input.mousePosition;
                            _offsetMouseScreenPosition = mainCam.WorldToScreenPoint(meshFold.transform.TransformPoint(localDragPos)) - Input.mousePosition;
                            _mouseDistanceTraveledInScaledViewportSpace = 0f;
                            _prevVibrateTime = 0f;

                            //float left, right, bottom, top;
                            //left = EnumMaskUtility.IsEnumPositive(dragableSpaceHorizontalMask, (int)DragSpaceHorizontal.LEFT) ? localPinPos.x - EXTRUDE_DRAGABLE_AREA_SIZE : localPinPos.x;
                            //right = EnumMaskUtility.IsEnumPositive(dragableSpaceHorizontalMask, (int)DragSpaceHorizontal.RIGHT) ? localPinPos.x + EXTRUDE_DRAGABLE_AREA_SIZE : localPinPos.x;
                            //bottom = EnumMaskUtility.IsEnumPositive(dragableSpaceVerticalMask, (int)DragSpaceVertical.BOTTOM) ? localPinPos.ySpeed - EXTRUDE_DRAGABLE_AREA_SIZE : localPinPos.ySpeed;
                            //top = EnumMaskUtility.IsEnumPositive(dragableSpaceVerticalMask, (int)DragSpaceVertical.TOP) ? localPinPos.ySpeed + EXTRUDE_DRAGABLE_AREA_SIZE : localPinPos.ySpeed;
                            //_localDragableArea = new Rect(left, bottom, right - left, top - bottom);
                            //_localBlockArea = new Rect(localPinPos.x - MARGIN_DRAGABLE_AREA, localPinPos.ySpeed - MARGIN_DRAGABLE_AREA, MARGIN_DRAGABLE_AREA * 2, MARGIN_DRAGABLE_AREA * 2);

                            meshFold.GenerateMesh();

                            transform.position += Vector3.back;

                            onStartDrag?.Invoke(this);
                        }
                    }
                }
            }
        }

        private void OnDragging()
        {
            Camera mainCam = Camera.main;
            Vector2 deltaScreenPos = (Vector2)Input.mousePosition - _prevMouseScreenPosition;

            float deltaMouseTraveledInScaledViewportSpace = Vector2.Distance(
                Vector2.Scale(mainCam.ScreenToViewportPoint(_prevMouseScreenPosition), new Vector2(1f, 1f / mainCam.aspect)),
                Vector2.Scale(mainCam.ScreenToViewportPoint(Input.mousePosition), new Vector2(1f, 1f / mainCam.aspect))
                );

            _mouseDistanceTraveledInScaledViewportSpace += deltaMouseTraveledInScaledViewportSpace;
            if (_mouseDistanceTraveledInScaledViewportSpace > _prevVibrateTime + vibrateIntervalByMouseDistanceTraveledInScaledViewportSpace)
            {
                MMVibrationManager.Haptic(vibrateType);
                _prevVibrateTime = _mouseDistanceTraveledInScaledViewportSpace;
            }


            _prevMouseScreenPosition = Input.mousePosition;

            float dampingMul = dampingMulByMouseDistanceTraveledInScaledViewportSpace.Evaluate(_mouseDistanceTraveledInScaledViewportSpace);
            _dampedMouseScreenPosition += deltaScreenPos * dampingMul;

            Ray ray = Camera.main.ScreenPointToRay(_dampedMouseScreenPosition + _offsetMouseScreenPosition);
            float distanceIntersect = 0f;
            meshFold.MeshPlane.Raycast(ray, out distanceIntersect);
            Vector3 dragPos = ray.GetPoint(distanceIntersect);

            Vector3 localDragPos = meshFold.transform.InverseTransformPoint(dragPos);

            // clamp
            bool isCanDrag = false;

            Vector2 deltaLocalDragPosToPinPoint = localDragPos - meshFold.pinPointTransform.localPosition;
            float angleLocalPinPointToNextDragPos = deltaLocalDragPosToPinPoint.SignedAngleFromRightVector();

            float deltaAngleToMainDragableAngle = Mathf.Abs(Mathf.DeltaAngle(angleLocalPinPointToNextDragPos, mainDragableAngle));

            if (deltaAngleToMainDragableAngle <= rangeDragableAngle / 2f)
            {
                isCanDrag = true;
            }
            else if (deltaAngleToMainDragableAngle < 90f)
            {
                float leftBoundAngle = mainDragableAngle - rangeDragableAngle / 2f;
                float rightBoundAngle = mainDragableAngle + rangeDragableAngle / 2f;
                float deltaAngleToLeftBound = Mathf.Abs(Mathf.DeltaAngle(angleLocalPinPointToNextDragPos, leftBoundAngle));
                float deltaAngleToRightBound = Mathf.Abs(Mathf.DeltaAngle(angleLocalPinPointToNextDragPos, rightBoundAngle));
                if (deltaAngleToLeftBound < deltaAngleToRightBound)
                {
                    localDragPos = meshFold.pinPointTransform.localPosition + new Vector3(Mathf.Cos(Mathf.Deg2Rad * leftBoundAngle), Mathf.Sin(Mathf.Deg2Rad * leftBoundAngle), 0f) * deltaLocalDragPosToPinPoint.magnitude * Mathf.Cos(Mathf.Deg2Rad * deltaAngleToLeftBound);
                }
                else
                {
                    localDragPos = meshFold.pinPointTransform.localPosition + new Vector3(Mathf.Cos(Mathf.Deg2Rad * rightBoundAngle), Mathf.Sin(Mathf.Deg2Rad * rightBoundAngle), 0f) * deltaLocalDragPosToPinPoint.magnitude * Mathf.Cos(Mathf.Deg2Rad * deltaAngleToRightBound);
                }

                isCanDrag = true;
            }
            else
            {
                isCanDrag = false;
            }

            float volume = 0f;
            if (isCanDrag 
                && (meshFold.pinPointTransform.localPosition - localDragPos).sqrMagnitude > blockRadiusFromPinPoint * blockRadiusFromPinPoint
                && !(meshFold.detachPointTransform && blockDetachPointRadius > 0f && Vector2.Distance(meshFold.detachPointTransform.localPosition, localDragPos) + blockDetachPointRadius > Vector2.Distance(meshFold.detachPointTransform.localPosition, meshFold.pinPointTransform.localPosition)))
            {
                volume = volumeMapToDeltaLocalDragPointChangedDistance.Evaluate(Vector2.Distance(meshFold.dragPointTransform.localPosition, localDragPos));
                meshFold.dragPointTransform.localPosition = localDragPos;

                bool isCanUpdateMesh = meshFold.GenerateMesh();
                if (!isCanUpdateMesh)
                {
                    IsDetached = true;
                    onDetached?.Invoke(this);
                    audioSourceDragging?.Stop();
                    FallOutside();
                }
            }

            if (audioSourceDragging) audioSourceDragging.volume = Mathf.Lerp(audioSourceDragging.volume, volume, lerpVolume);
        }

        private void OnEndDrag()
        {
            audioSourceDragging?.Stop();
            IsDragging = false;
            transform.position += Vector3.forward;
            onEndDrag?.Invoke(this);
        }

        public void FallOutside()
        {
            Vector2 speedFall = (meshFold.dragPointTransform.position - meshFold.pinPointTransform.position).normalized * UnityEngine.Random.Range(velocityFallRange.x, velocityFallRange.y);
            StartCoroutine(IEFall(speedFall));
            IEnumerator IEFall(Vector2 speedFall)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, -2f);
                while (true)
                {
                    speedFall += constForce * Time.deltaTime;
                    transform.position += (Vector3)speedFall * Time.deltaTime;

                    if (transform.position.y < positionYDisableObject)
                    {
                        break;
                    }

                    yield return null;
                }
                gameObject.SetActive(false);
            }
        }

        public void Shake()
        {
            if (_shakeTween != null && _shakeTween.IsActive()) _shakeTween.Complete();
            _shakeTween = transform.DOShakePosition(shakeDuration, shakeMagnitude, shakeVibrato);
        }

        private void OnDrawGizmosSelected()
        {
            var backupMatrix = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(meshFold.transform.position, meshFold.transform.rotation, Vector3.one);
            //GizmoUtility.DrawRect(_localDragableArea, Color.green);
            //GizmoUtility.DrawRect(_localBlockArea, Color.red);
            GizmoUtility.DrawAngleRangeWithAnchor(mainDragableAngle, rangeDragableAngle, Color.green);
            Gizmos.matrix = backupMatrix;
        }
    }
}