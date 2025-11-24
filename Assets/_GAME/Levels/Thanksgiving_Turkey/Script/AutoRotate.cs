using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link.Turkey
{
    public class AutoRotate : MonoBehaviour
    {

        private void LateUpdate()
        {
            transform.rotation = Quaternion.identity;      
        }
    }
}