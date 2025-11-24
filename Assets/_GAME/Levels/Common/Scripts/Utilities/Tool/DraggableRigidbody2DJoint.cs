using System.Collections;
using UnityEngine;

namespace Utilities
{
    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class DraggableRigidbody2DJoint : MonoBehaviour
    {
        public Rigidbody2D Rb { get; private set; }
        private float _cachedGravityScale;
        private Vector2 _offset;
        protected Joint2D Joint { get; private set; }

        [Header("Collider Filter")]
        public bool isUseColliderFilterAllowDrag = false;
        [Sirenix.OdinInspector.ShowIf("isUseColliderFilterAllowDrag")]
        public Collider2D[] colliderFilterAllowDrag = null;

        [Header("Limit Time & Distance")]
        public float maxDragTime = -1f;
        public float maxDragDistance = -1f;
        private float _timeStartDrag = -1f;
        private Vector2 _posStartDrag = Vector2.zero;

        [Header("Joint2D properties")]
        public float drag = -1;
        public float angularDrag = -1f;
        public bool isFreezeRotationOnDrag = false;
        private bool _originalFreezeRotation;
        public bool isUseCenterOfMassAsDragPoint = true;
        private Vector2 _cacheCenterOfMass;

        [Header("Rotate to specific angle on drag")]
        public bool isRotateToSpecificAngleOnDrag = false;
        [Sirenix.OdinInspector.ShowIf("isRotateToSpecificAngleOnDrag")]
        public float specificAngle = 0f;
        [Sirenix.OdinInspector.ShowIf("isRotateToSpecificAngleOnDrag")]
        public float durationRotateToSpecificAngle = 1f;
        [Sirenix.OdinInspector.ShowIf("isRotateToSpecificAngleOnDrag")]
        public AnimationCurve angularLerpEase = AnimationCurve.Linear(0, 0, 1, 1);

        [Header("On End Drag")]
        public float velocityMulOnEndRelease = 1f;
        public float clampVelocityOnEndRelease = -1f;

        [Header("Drag Event")]
        public UnityEngine.Events.UnityEvent<DraggableRigidbody2DJoint> OnStartDrag;
        public UnityEngine.Events.UnityEvent<DraggableRigidbody2DJoint> OnEndDrag;

        public BoolModifierWithRegisteredSource isPreventDrag;
        public bool IsDragging { get; private set; }

        private void Awake()
        {
            IsDragging = false;
            isPreventDrag = new BoolModifierWithRegisteredSource(OnChangedCanDrag);
            Rb = GetComponent<Rigidbody2D>();
            _cacheCenterOfMass = Rb.centerOfMass;
            _originalFreezeRotation = Rb.freezeRotation;
        }

        private void OnChangedCanDrag()
        {
            if (isPreventDrag.Value)
            {
                OnMouseUp();
            }
        }

        private void OnMouseDown()
        {
            if (isPreventDrag.Value || IsDragging) return;

            Vector2 mouseWorldPos = GetMouseWorldPos();
            if (isUseColliderFilterAllowDrag)
            {
                bool isMouseOverCollider = false;
                foreach (Collider2D collider in colliderFilterAllowDrag)
                {
                    if (collider.OverlapPoint(mouseWorldPos))
                    {
                        isMouseOverCollider = true;
                        break;
                    }
                }
                if (!isMouseOverCollider) return;
            }

            IsDragging = true;

            _timeStartDrag = Time.time;
            _posStartDrag = mouseWorldPos;

            OnStartDrag.Invoke(this);

            _offset = Rb.position - mouseWorldPos;
            _cachedGravityScale = Rb.gravityScale;

            if (!Rb.isKinematic)
            {
                Joint = SetUpJoint();

                Rb.freezeRotation = isFreezeRotationOnDrag;

                Vector2 mouseLocalPoint = this.transform.InverseTransformPoint(mouseWorldPos);
                if (isUseCenterOfMassAsDragPoint)
                {
                    Rb.centerOfMass = mouseLocalPoint;
                }

                StartCoroutine(DragObject());
            }

            if (isRotateToSpecificAngleOnDrag)
            {
                StartCoroutine(IERotateToSpecificAngle());
            }
        }

        protected abstract Joint2D SetUpJoint();

        protected Vector2 GetMouseWorldPos() => Camera.main.ScreenToWorldPoint(Input.mousePosition);

        private void OnMouseDrag()
        {
            if (!IsDragging) return;

            Vector2 mouseWorldPos = GetMouseWorldPos();

            if (Rb.isKinematic)
            {
                Rb.position = mouseWorldPos + _offset;
            }

            if ((maxDragTime > 0f && Time.time > _timeStartDrag + maxDragTime)
                || (maxDragDistance > 0f && Vector2.Distance(mouseWorldPos, _posStartDrag) > maxDragDistance))
            {
                OnMouseUp();
            }
        }

        private void OnMouseUp()
        {
            if (IsDragging)
            {
                Rb.gravityScale = _cachedGravityScale;
                if (Joint) Joint.enabled = false;
                IsDragging = false;

                Rb.freezeRotation = _originalFreezeRotation;
                Rb.velocity *= velocityMulOnEndRelease;
                if (clampVelocityOnEndRelease > 0f) Vector2.ClampMagnitude(Rb.velocity, clampVelocityOnEndRelease);

                UpdateJointOnEndDrag();

                OnEndDrag.Invoke(this);
            }
        }

        IEnumerator IERotateToSpecificAngle()
        {
            float timeStart = Time.fixedTime;
            var waitFixedUpdate = new WaitForFixedUpdate();
            float startAngle = Rb.rotation;
            while (IsDragging && Time.fixedTime < timeStart + durationRotateToSpecificAngle)
            {
                float angle = Mathf.LerpAngle(startAngle, specificAngle, angularLerpEase.Evaluate((Time.fixedTime - timeStart) / durationRotateToSpecificAngle));
                //float torqueImpulse = (angle - Rb.rotation) * Mathf.Deg2Rad * Rb.inertia;
                //Rb.AddTorque(torqueImpulse, ForceMode2D.Impulse);
                Rb.MoveRotation(angle);
                yield return waitFixedUpdate;
            }
        }

        IEnumerator DragObject()
        {
            //
            // Save the drag and angular drag of the hit rigidbody, since this
            // script has a drag and angular drag of its own. We don't want the
            // rigidbody to fly to our position too quickly!
            //
            float oldDrag = Rb.drag;
            float oldAngularDrag = Rb.angularDrag;

            if (drag >= 0f) Rb.drag = drag;
            if (angularDrag >= 0f) Rb.angularDrag = angularDrag;

            while (IsDragging)
            {
                OnUpdateDraggingDynamic();
                yield return null;
            }

            Rb.centerOfMass = _cacheCenterOfMass;
            if (drag >= 0f) Rb.drag = oldDrag;
            if (angularDrag >= 0f) Rb.angularDrag = oldAngularDrag;
        }

        protected abstract void OnUpdateDraggingDynamic();
        protected abstract void UpdateJointOnEndDrag();
    }
}