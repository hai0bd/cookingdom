using Hai.Cooking.Burrito;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hai.Cooking.Burrito 
{
    public class SpiceItemFry : MonoBehaviour
    {
        [SerializeField] private Vector3 center;
        [SerializeField] private float radius, force = 1f;
        [SerializeField] private SpriteRenderer[] rens;
        [SerializeField] private CircleCollider2D col;
        [SerializeField] private float itemRadius;

        Spatula spatula;

        private bool isCollapse = false;
        private bool isActive = true;

        private float rate;
        private Color color;
        public void OnInit(Vector3 center, float radius)
        {
            this.center = center;
            this.radius = radius;
            col.enabled = true;
        }

        public void SetRadius(float radius)
        {
            this.radius = radius;
        }

        public void SetRipeRate(float rate)
        {
            this.rate = rate;

            if (rate < 1f)
            {
                color.a = 1 - rate;
                rens[0].color = color;
                color.a = rate;
                rens[1].color = color;
                rens[2].color = Color.clear;
            }
            else if (rate >= 1 && rate < 2)
            {
                rens[0].color = Color.clear;
                color.a = 2 - rate;
                rens[1].color = color;
                color.a = rate - 1;
                rens[2].color = color;
            }
            else if (rate >= 2 && rate < 3)
            {
                rens[0].color = Color.clear;
                rens[1].color = Color.clear;
                color.a = 3 - rate;
                rens[2].color = color;
            }

        }

        public void AddForce(Spatula spatula, Vector2 force)
        {
            this.spatula = spatula;
            if (isCollapse)
            {
                isCollapse = false;
            }
            StopAllCoroutines();
            StartCoroutine(IEForce(force * this.force));
        }

        private IEnumerator IEForce(Vector3 force)
        {
            float time = 0.2f;
            float t = 0;
            float speed = 2.5f;
            Vector3 newPoint = transform.position;
            col.enabled = false;
            while (t < time)
            {
                t += Time.deltaTime;
                newPoint = speed * Time.deltaTime * force + transform.position;
                if (Vector3.Distance(newPoint, center) > radius)
                {
                    force = Vector2.Reflect(force, center - newPoint).normalized;
                    newPoint = speed * Time.deltaTime * force + transform.position;
                }
                transform.position = newPoint;
                yield return null;
            }
            col.enabled = true;
        }

        public void SetAlpha(float alpha)
        {
            color.a = alpha;
            rens[Mathf.FloorToInt(rate)].color = color;
        }

        public void SetControl(bool active)
        {
            this.isActive = active;
            col.enabled = active;
        }

        public void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(center, radius);
        }

    }
}
