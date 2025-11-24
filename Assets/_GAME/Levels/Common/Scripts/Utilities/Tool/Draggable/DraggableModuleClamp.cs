using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

namespace Utilities
{
    public class DraggableModuleClamp : MonoBehaviour
    {
        public bool isClampRelativeToViewport = false;
        public Rect areaClampPosition = new Rect(-1, -1, 2, 2);

        public Vector3 Clamp(Vector3 position)
        {
            if (isClampRelativeToViewport)
            {
                Vector2 min = Camera.main.ViewportToWorldPoint(new Vector3(areaClampPosition.xMin, areaClampPosition.yMin, 0));
                Vector2 max = Camera.main.ViewportToWorldPoint(new Vector3(areaClampPosition.xMax, areaClampPosition.yMax, 0));
                position.x = Mathf.Clamp(position.x, min.x, max.x);
                position.y = Mathf.Clamp(position.y, min.y, max.y);
            }
            else
            {
                position.x = Mathf.Clamp(position.x, areaClampPosition.xMin, areaClampPosition.xMax);
                position.y = Mathf.Clamp(position.y, areaClampPosition.yMin, areaClampPosition.yMax);
            }
            return position;
        }

        private void OnDrawGizmosSelected()
        {
            if (isClampRelativeToViewport)
            {
                Vector2 min = Camera.main.ViewportToWorldPoint(new Vector3(areaClampPosition.xMin, areaClampPosition.yMin, 0));
                Vector2 max = Camera.main.ViewportToWorldPoint(new Vector3(areaClampPosition.xMax, areaClampPosition.yMax, 0));
                GizmoUtility.DrawRect(new Rect(min, max - min), Color.green);
            }
            else
            {
                GizmoUtility.DrawRect(areaClampPosition, Color.green);
            }
        }
    }
}