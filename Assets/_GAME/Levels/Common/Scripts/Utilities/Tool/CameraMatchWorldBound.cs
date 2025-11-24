using UnityEngine;
using Utilities;

public class CameraMatchWorldBound : MonoBehaviour
{
    public enum ScaleMode { Fit, Fill }
    public Transform quadTransform;  // The quad transform
    public ScaleMode scaleMode;
    public float scaleMul = 1f;
    [Tooltip("The anchor position to match camera with quad. For example, set anchor = (0,0) for camera always at center of quad, or set anchor = (0,-1) to make bottom edge of camera match with bottom edge of quad, or set anchor = (1,1) to make top right of camera match with top right of quad")]
    public Vector2 anchor = Vector2.zero;

    void Start()
    {
        AdjustCamera();
    }

    public Rect GetScreenWorldBoundary()
    {
        Camera camera = GetComponent<Camera>();
        Vector2 cameraSize = new Vector2(camera.orthographicSize * camera.aspect, camera.orthographicSize);
        return new Rect(camera.transform.position.x - cameraSize.x, camera.transform.position.y - cameraSize.y, cameraSize.x * 2, cameraSize.y * 2);
    }

    [Sirenix.OdinInspector.Button("Adjust Camera")]
    public void AdjustCamera()
    {
        Camera camera = GetComponent<Camera>();
        float quadHeight = quadTransform.localScale.y;
        float quadWidth = quadTransform.localScale.x;

        if (camera.orthographic)
        {
            // Calculate the orthographic size based on the quad size and the aspect ratio
            //float aspectRatio = (float)Screen.width / (float)Screen.height;
            float aspectRatio = camera.aspect;
            float orthographicSize = 1f;

            switch (scaleMode)
            {
                case ScaleMode.Fit:
                    if (quadWidth / quadHeight > aspectRatio)
                    {
                        AdjustOrthographicSizeByQuadWidth();
                    }
                    else
                    {
                        AdjustOrthographicSizeByQuadHeight();
                    }
                    break;
                case ScaleMode.Fill:
                    if (quadWidth / quadHeight > aspectRatio)
                    {
                        AdjustOrthographicSizeByQuadHeight();
                    }
                    else
                    {
                        AdjustOrthographicSizeByQuadWidth();
                    }
                    break;
            }

            void AdjustOrthographicSizeByQuadWidth()
            {
                orthographicSize = quadWidth / aspectRatio / 2;
            }
            void AdjustOrthographicSizeByQuadHeight()
            {
                orthographicSize = quadHeight / 2;
            }

            camera.orthographicSize = orthographicSize * scaleMul;

            Vector3 quadAnchorLocalPos = new Vector3(quadTransform.localScale.x * anchor.x / 2f, quadTransform.localScale.y * anchor.y / 2f);
            Vector3 cameraAnchorLocalPos = new Vector3(orthographicSize * aspectRatio * anchor.x, orthographicSize * anchor.y);
            camera.transform.position = quadTransform.position + quadAnchorLocalPos - cameraAnchorLocalPos + Vector3.back * 10f;
        }
        else // Perspective
        {
            float aspectRatio = camera.aspect;

            float distance = Mathf.Abs(camera.transform.position.z - quadTransform.position.z);

            switch (scaleMode)
            {
                case ScaleMode.Fit:
                    if (quadWidth / quadHeight > aspectRatio)
                    {
                        AdjustPerspectivePosByQuadWidth();
                    }
                    else
                    {
                        AdjustPerspectivePosByQuadHeight();
                    }
                    break;
                case ScaleMode.Fill:
                    if (quadWidth / quadHeight > aspectRatio)
                    {
                        AdjustPerspectivePosByQuadHeight();
                    }
                    else
                    {
                        AdjustPerspectivePosByQuadWidth();
                    }
                    break;
            }

            void AdjustPerspectivePosByQuadWidth()
            {
                float currentViewWidth = distance * Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad) * 2f * camera.aspect;
                distance = quadTransform.localScale.x / currentViewWidth * distance;
            }

            void AdjustPerspectivePosByQuadHeight()
            {
                float currentViewHeight = distance * Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad) * 2f;
                distance = quadTransform.localScale.y / currentViewHeight * distance;
            }

            Vector3 camPosition = camera.transform.position;
            camPosition.z = quadTransform.position.z - distance;
            camera.transform.position = camPosition;
        }

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(camera);
#endif
    }

    private void OnDrawGizmosSelected()
    {
        if (quadTransform)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(quadTransform.position, quadTransform.localScale);
            GizmoUtility.DrawPoint(quadTransform.position + new Vector3(quadTransform.localScale.x * anchor.x / 2f, quadTransform.localScale.y * anchor.y / 2f));
        }
    }

#if UNITY_EDITOR
    [Header("Editor Tool")]
    [SerializeField] private bool isUpdateOnValidateEditor = false;
    [SerializeField] private Vector2 quadCenterEditor = Vector2.zero;
    [SerializeField] private Vector2 quadSizeEditor = Vector2.one;
    private void OnValidate()
    {
        if (isUpdateOnValidateEditor && quadTransform)
        {
            quadTransform.position = new Vector3(quadCenterEditor.x, quadCenterEditor.y);
            quadTransform.localScale = new Vector3(quadSizeEditor.x, quadSizeEditor.y);
            UnityEditor.EditorUtility.SetDirty(quadTransform);
            AdjustCamera();
        }
    }

    [Sirenix.OdinInspector.Button("Sync Quad Center And Size")]
    private void SyncQuadCenterAndSizeEditor()
    {
        if (quadTransform)
        {
            quadCenterEditor = new Vector2(quadTransform.position.x, quadTransform.position.y);
            quadSizeEditor = new Vector2(quadTransform.localScale.x, quadTransform.localScale.y);
        }
    }
#endif
}
