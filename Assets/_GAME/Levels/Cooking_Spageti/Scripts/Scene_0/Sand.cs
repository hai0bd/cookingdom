using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link.Cooking.Spageti
{
    public class Sand : ItemMovingBase
    {
        float y;
        
        protected override void Start()
        {
            base.Start();

            y = transform.position.y;
        }

        public override void OnClickDown()
        {
            
        }

        public override void OnDrop()
        {
            
        }

        void Update()
        {
            transform.position = new Vector3(transform.position.x, y, transform.position.z);
        }
        void LateUpdate()
        {
            transform.position = new Vector3(transform.position.x, y, transform.position.z);
        }
    }
}
