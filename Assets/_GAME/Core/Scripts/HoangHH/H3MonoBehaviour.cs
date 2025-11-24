using UnityEngine;

namespace HoangHH
{
    public class H3MonoBehaviour : MonoBehaviour
    {
        private Transform _tf;
        
        public Transform Tf => _tf ? _tf : _tf = transform;

        protected float DistanceToInSqr(Transform target)
        {
            // using sqrMagnitude instead of magnitude for performance
            return (Tf.position - target.position).sqrMagnitude;
        }
        
        protected float DistanceToInSqr(Vector3 target)
        {
            // using sqrMagnitude instead of magnitude for performance
            return (Tf.position - target).sqrMagnitude;
        }

        protected float DistanceToInSqrVec2(Transform target)
        {
            Vector2 targetPos = target.position;
            Vector2 myPos = Tf.position;
            return (myPos - targetPos).sqrMagnitude;
        }

        protected float DistanceToInSqrVec2(Vector2 point1)
        {
            Vector2 myPos = Tf.position;
            return (myPos - point1).sqrMagnitude;
        }
    }
}
