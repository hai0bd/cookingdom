using System;
using System.Collections;
using UnityEngine;

namespace Utilities
{
    public class CreateWall2DAroundScreen : MonoBehaviour
    {
        public Camera _camera = null;
        public float wallThickness = 1f;
        public float wallLengthExtrude = 2f;
        public float wallSpaceExpand = 0f;
        public enum WallDirection { TOP, BOTTOM, LEFT, RIGHT }
        [EnumMask(typeof(WallDirection))] public int wallDirectionEnableMask = 0;

        private BoxCollider2D[] _wallColliders = new BoxCollider2D[4];
        private float _prevCameraOrthographicSize;
        private float _prevCameraAspectRatio;

        public bool isAutoSetUpWallOnStart = true;
        public bool isAutoUpdateWallOnCameraSizeChanged = true;

        private void Start()
        {
            if (isAutoSetUpWallOnStart)
            {
                SetUpWall();
            }
        }

        public void SetUpWall()
        {
            if (!_camera) _camera = Camera.main;
            _prevCameraAspectRatio = _camera.aspect;
            _prevCameraOrthographicSize = _camera.orthographicSize;
            CreateWall();
            UpdateWall();
        }

        private void Update()
        {
            if (isAutoUpdateWallOnCameraSizeChanged && (_camera.aspect != _prevCameraAspectRatio || _camera.orthographicSize != _prevCameraOrthographicSize))
            {
                UpdateWall();
                _prevCameraAspectRatio = _camera.aspect;
                _prevCameraOrthographicSize = _camera.orthographicSize;
            }
        }

        private void CreateWall()
        {
            Vector2 topLeft, topRight, bottomLeft, bottomRight;
            topLeft     = Camera.main.ScreenToWorldPoint(new Vector3(0, +Screen.height, 0));
            topRight    = Camera.main.ScreenToWorldPoint(new Vector3(+Screen.width, +Screen.height, 0));
            bottomLeft  = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
            bottomRight = Camera.main.ScreenToWorldPoint(new Vector3(+Screen.width, 0, 0));

            WallDirection[] wallDirections = System.Enum.GetValues(typeof(WallDirection)) as WallDirection[];
            foreach (var wallDirection in wallDirections)
            {
                if (EnumMaskUtility.IsEnumPositive(wallDirectionEnableMask, (int)wallDirection))
                {
                    GameObject wall = new GameObject("Wall-" + wallDirection);
                    wall.transform.parent = transform;
                    BoxCollider2D collider = wall.AddComponent<BoxCollider2D>();
                    _wallColliders[(int)wallDirection] = collider;

                    switch (wallDirection)
                    {
                        case WallDirection.TOP:
                            wall.transform.position = new Vector3((topLeft.x + topRight.x) / 2f, topRight.y + wallThickness / 2f + wallSpaceExpand, 0);
                            collider.size = new Vector2(topRight.x - topLeft.x + 2f * wallLengthExtrude, wallThickness);
                            break;

                        case WallDirection.BOTTOM:
                            wall.transform.position = new Vector3((bottomLeft.x + bottomRight.x) / 2f, bottomLeft.y - wallThickness / 2f - wallSpaceExpand, 0);
                            collider.size = new Vector2(bottomRight.x - bottomLeft.x + 2f * wallLengthExtrude, wallThickness);
                            break;

                        case WallDirection.LEFT:
                            wall.transform.position = new Vector3(bottomLeft.x - wallThickness / 2f - wallSpaceExpand, (bottomLeft.y + topLeft.y) / 2f, 0);
                            collider.size = new Vector2(wallThickness, topLeft.y - bottomLeft.y + 2f * wallLengthExtrude);
                            break;

                        case WallDirection.RIGHT:
                            wall.transform.position = new Vector3(topRight.x + wallThickness / 2f + wallSpaceExpand, (topRight.y + bottomRight.y) / 2f, 0);
                            collider.size = new Vector2(wallThickness, topRight.y - bottomRight.y + 2f * wallLengthExtrude);
                            break;
                    }
                }
            }
        }

        private void UpdateWall()
        {
            Vector2 topLeft, topRight, bottomLeft, bottomRight;
            topLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, +Screen.height, 0));
            topRight = Camera.main.ScreenToWorldPoint(new Vector3(+Screen.width, +Screen.height, 0));
            bottomLeft = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0));
            bottomRight = Camera.main.ScreenToWorldPoint(new Vector3(+Screen.width, 0, 0));

            WallDirection[] wallDirections = System.Enum.GetValues(typeof(WallDirection)) as WallDirection[];
            foreach (var wallDirection in wallDirections)
            {
                if (EnumMaskUtility.IsEnumPositive(wallDirectionEnableMask, (int)wallDirection))
                {
                    BoxCollider2D collider = _wallColliders[(int)wallDirection];
                    Transform wallTransform = collider.transform;

                    switch (wallDirection)
                    {
                        case WallDirection.TOP:
                            wallTransform.position = new Vector3((topLeft.x + topRight.x) / 2f, topRight.y + wallThickness / 2f + wallSpaceExpand, 0);
                            collider.size = new Vector2(topRight.x - topLeft.x + 2f * wallLengthExtrude, wallThickness);
                            break;

                        case WallDirection.BOTTOM:
                            wallTransform.position = new Vector3((bottomLeft.x + bottomRight.x) / 2f, bottomLeft.y - wallThickness / 2f - wallSpaceExpand, 0);
                            collider.size = new Vector2(bottomRight.x - bottomLeft.x + 2f * wallLengthExtrude, wallThickness);
                            break;

                        case WallDirection.LEFT:
                            wallTransform.position = new Vector3(bottomLeft.x - wallThickness / 2f - wallSpaceExpand, (bottomLeft.y + topLeft.y) / 2f, 0);
                            collider.size = new Vector2(wallThickness, topLeft.y - bottomLeft.y + 2f * wallLengthExtrude);
                            break;

                        case WallDirection.RIGHT:
                            wallTransform.position = new Vector3(topRight.x + wallThickness / 2f + wallSpaceExpand, (topRight.y + bottomRight.y) / 2f, 0);
                            collider.size = new Vector2(wallThickness, topRight.y - bottomRight.y + 2f * wallLengthExtrude);
                            break;
                    }
                }
            }
        }
    }
}