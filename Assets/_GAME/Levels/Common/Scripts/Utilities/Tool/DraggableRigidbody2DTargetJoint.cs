using System.Collections;
using UnityEngine;

namespace Utilities
{
    public class DraggableRigidbody2DTargetJoint : DraggableRigidbody2DJoint
    {
        [Header("Target Joint")]
        public float maxForce = 100.0f;
        public float dampingRatio = 5.0f;
        public float frequency = 2.5f;
        private TargetJoint2D joint;

        protected override Joint2D SetUpJoint()
        {
            if (!joint)
            {
                joint = this.gameObject.AddComponent<TargetJoint2D>();
            }

            Vector2 mouseWorldPos = GetMouseWorldPos();
            Vector2 mouseLocalPoint = this.transform.InverseTransformPoint(mouseWorldPos);
            joint.target = mouseWorldPos;
            // Spring endpoint, set to the position of the hit object:
            joint.anchor = mouseLocalPoint;
            // Initially, both spring endpoints are the same point:
            joint.maxForce = this.maxForce;
            joint.dampingRatio = this.dampingRatio;
            joint.frequency = this.frequency;
            joint.enabled = true;

            return joint;
        }

        protected override void OnUpdateDraggingDynamic()
        {
            joint.target = GetMouseWorldPos();
        }

        protected override void UpdateJointOnEndDrag()
        {
            // do nothing
        }
    }
}