using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link.Cooking.Lobster
{
    public class Fish : MonoBehaviour
    {
        [SerializeField] CapyCatchFish capy;
        [SerializeField] int index;
        [SerializeField] Vector2 range = new Vector2(1.5f, -4f);

        private Transform tf;
        public Transform Tf => tf ? tf : tf = transform;

        public float speed = 2f;


        private Vector3 minScreenBounds;
        private Vector3 maxScreenBounds;

        private void Start()
        {
            minScreenBounds = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane));
            maxScreenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.nearClipPlane));
        }

        private void Update()
        {
            Tf.position += Vector3.left * speed * Time.deltaTime;

            if(Tf.position.x < minScreenBounds.x - 2f)
            {
                Tf.position = new Vector2(maxScreenBounds.x + Random.Range(0, 5f), Random.Range(range.x, range.y));
            }
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (capy.IsHaveFish) return;
            capy.OnCatchFish(index);
            gameObject.SetActive(false);
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(Vector3.up * range.x, .1f);
            Gizmos.DrawWireSphere(Vector3.up * range.y, .1f);
        }
    }
}

