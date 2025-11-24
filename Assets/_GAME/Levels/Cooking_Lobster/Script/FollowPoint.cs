using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link.Cooking
{
    public class FollowPoint : MonoBehaviour
    {
        [SerializeField] Transform target;

        // Update is called once per frame
        void LateUpdate()
        {
            transform.position = target.position;
        }
    }
}
