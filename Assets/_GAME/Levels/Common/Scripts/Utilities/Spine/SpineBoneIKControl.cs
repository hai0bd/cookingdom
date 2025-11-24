using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UIElements;

namespace Utilities
{
    public class SpineBoneIKControl : MonoBehaviour
    {
        public SkeletonAnimation skeletonAnimation;
        [SpineBone(dataField: "skeletonAnimation")]
        public string boneName;
        private Bone bone;
        public Transform target;
        public Vector3 positionOffset;
        public float scalingX = 1;
        public float scalingY = 1;
        public bool isBoneRotateControlByTarget;

        private void Start()
        {
            bone = skeletonAnimation.Skeleton.FindBone(boneName);
            skeletonAnimation.UpdateWorld += UpdateIK;
        }

        private void UpdateIK(ISkeletonAnimation animated)
        {
            Vector2 targetWorldPos = (target.position + positionOffset);
            targetWorldPos.x *= scalingX;
            targetWorldPos.y *= scalingY;
            Vector2 targetLocalPos = skeletonAnimation.transform.InverseTransformPoint(targetWorldPos);
            bone.SetPositionSkeletonSpace(targetLocalPos);
            if (isBoneRotateControlByTarget) bone.Rotation = target.localRotation.eulerAngles.z;
        }
    }
}