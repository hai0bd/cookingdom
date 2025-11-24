using System.Collections;
using UnityEngine;

namespace Utilities
{
    public class DraggableRigidbody2DSpringJoint : DraggableRigidbody2DJoint
    {
        [Header("Spring Joint")]
        public float dampingRatio = 5.0f;
        public float frequency = 2.5f;

        private SpringJoint2D joint;
        private Rigidbody2D jointBody;

        protected override Joint2D SetUpJoint()
        {
            if (!joint)
            {
                GameObject obj = new GameObject("Rigidbody2D dragger");
                jointBody = obj.AddComponent<Rigidbody2D>();
                jointBody.isKinematic = true;
                joint = obj.AddComponent<SpringJoint2D>();
            }

            Vector2 mouseWorldPos = GetMouseWorldPos();

            joint.transform.position = mouseWorldPos;
            // Spring endpoint, set to the position of the hit object:
            joint.anchor = Vector2.zero;
            // Initially, both spring endpoints are the same point:
            joint.connectedAnchor = transform.InverseTransformPoint(mouseWorldPos);
            joint.dampingRatio = dampingRatio;
            joint.frequency = frequency;
            // Don't want our invisible "Rigidbody2D dragger" to collide!
            joint.enableCollision = false;
            joint.connectedBody = Rb;
            joint.autoConfigureDistance = false;

            joint.enabled = true;

            return joint;
        }

        protected override void OnUpdateDraggingDynamic()
        {
            jointBody.MovePosition(GetMouseWorldPos());
        }

        protected override void UpdateJointOnEndDrag()
        {
            joint.connectedBody = null;
        }
    }
}