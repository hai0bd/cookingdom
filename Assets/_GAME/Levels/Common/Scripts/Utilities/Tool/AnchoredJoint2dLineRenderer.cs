using System.Collections;
using UnityEngine;

namespace Utilities
{
    public class AnchoredJoint2dLineRenderer : MonoBehaviour
    {
        public AnchoredJoint2D joint;
        public LineRenderer lineRenderer;
        public Vector3 offsetMainBody;
        public Vector3 offsetConnectedBody;

        private void OnEnable()
        {
            lineRenderer.enabled = true;
        }

        private void OnDisable()
        {
            if (lineRenderer) lineRenderer.enabled = false;
        }

        private void Update()
        {
            lineRenderer.SetPosition(0, joint.connectedBody.transform.position + offsetConnectedBody);
            lineRenderer.SetPosition(1, joint.transform.position + offsetMainBody);
        }

#if UNITY_EDITOR
        [Sirenix.OdinInspector.Button]
        private void UpdateRopeEditor()
        {
            lineRenderer.SetPosition(0, joint.connectedBody.transform.position + offsetConnectedBody);
            lineRenderer.SetPosition(1, joint.transform.position + offsetMainBody);
            UnityEditor.EditorUtility.SetDirty(lineRenderer);
        }
#endif
    }
}