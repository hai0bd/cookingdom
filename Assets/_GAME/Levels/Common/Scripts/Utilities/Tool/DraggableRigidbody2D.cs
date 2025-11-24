using System.Collections;
using UnityEngine;

namespace Utilities
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class DraggableRigidbody2D : MonoBehaviour
    {
        public Rigidbody2D Rb { get; private set; }
        private float _cachedGravityScale;
        private Vector2 _offset;
        private TargetJoint2D _targetJoint;

        [Header("Joint2D properties")]
        public float maxForce = 100.0f;
        public float dampingRatio = 5.0f;
        public float frequency = 2.5f;
        public float drag = 10.0f;
        public float angularDrag = 5.0f;
        public bool isFreezeRotationOnDrag = false;
        public bool isUseCenterOfMassAsDragPoint = true;
        private Vector2 _cacheCenterOfMass;

        [Header("Rotate to specific angle on drag")]
        public bool isRotateToSpecificAngleOnDrag = false;
        public float specificAngle = 0f;
        public float durationRotateToSpecificAngle = 1f;
        public AnimationCurve angularLerpEase = AnimationCurve.Linear(0, 0, 1, 1);

        [Header("On End Drag")]
        public float velocityMulOnEndRelease = 1f;
        public float clampVelocityOnEndRelease = -1f;

        [Header("Drag Event")]
        public UnityEngine.Events.UnityEvent OnStartDrag;
        public UnityEngine.Events.UnityEvent OnEndDrag;

        public BoolModifierWithRegisteredSource isPreventDrag;
        public bool IsDragging { get; private set; }

        private bool _isFreezeRotationOrigin;

        private void Awake()
        {
            IsDragging = false;
            isPreventDrag = new BoolModifierWithRegisteredSource(OnChangedCanDrag);
            Rb = GetComponent<Rigidbody2D>();
            _cacheCenterOfMass = Rb.centerOfMass;
            _isFreezeRotationOrigin = Rb.freezeRotation;
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
            IsDragging = true;

            OnStartDrag.Invoke();

            Vector2 mouseWorldPos = GetMouseWorldPos();
            _offset = Rb.position - mouseWorldPos;
            _cachedGravityScale = Rb.gravityScale;

            if (!Rb.isKinematic && Rb.simulated)
            {
                if (!_targetJoint)
                {
                    _targetJoint = this.gameObject.AddComponent<TargetJoint2D>();
                }

                Vector2 mouseLocalPoint = this.transform.InverseTransformPoint(mouseWorldPos);

                _targetJoint.target = mouseWorldPos;
                // Spring endpoint, set to the position of the hit object:
                _targetJoint.anchor = mouseLocalPoint;
                // Initially, both spring endpoints are the same point:
                _targetJoint.maxForce = this.maxForce;
                _targetJoint.dampingRatio = this.dampingRatio;
                _targetJoint.frequency = this.frequency;
                _targetJoint.enabled = true;

                Rb.freezeRotation = isFreezeRotationOnDrag;

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

        private Vector2 GetMouseWorldPos() => Camera.main.ScreenToWorldPoint(Input.mousePosition);

        private void OnMouseDrag()
        {
            if (!IsDragging) return;

            if (Rb.isKinematic)
            {
                Rb.position = GetMouseWorldPos() + _offset;
            }
        }

        private void OnMouseUp()
        {
            if (IsDragging)
            {
                //Rb.position = GetMouseWorldPos() + _offset;

                Rb.gravityScale = _cachedGravityScale;
                if (_targetJoint) _targetJoint.enabled = false;
                IsDragging = false;

                Rb.freezeRotation = _isFreezeRotationOrigin;
                Rb.velocity *= velocityMulOnEndRelease;
                if (clampVelocityOnEndRelease > 0f) Vector2.ClampMagnitude(Rb.velocity, clampVelocityOnEndRelease);

                OnEndDrag.Invoke();
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

            Rb.drag = drag;
            Rb.angularDrag = angularDrag;

            //
            // The spring joint's position becomes 
            //
            while (IsDragging)
            {
                //_targetJoint.attachedRigidbody.position = GetMouseWorldPos();
                _targetJoint.target = GetMouseWorldPos();
                yield return null;
            }

            Rb.centerOfMass = _cacheCenterOfMass;

            //
            // The player released the mouse button, so the spring joint is now
            // detached. The spring joint can be used again later.
            //
            if (_targetJoint.connectedBody)
            {
                Rb.drag = oldDrag;
                Rb.angularDrag = oldAngularDrag;
            }
        }
    }
}