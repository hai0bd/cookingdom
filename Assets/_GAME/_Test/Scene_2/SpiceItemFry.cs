using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HoangLinh.Cooking.Test
{
    public class SpiceItemFry : MonoBehaviour
    {
        [SerializeField] Vector3 center;
        [SerializeField] float radius, force = 1f;
        [SerializeField] SpriteRenderer[] rens;
        [SerializeField] CircleCollider2D col;
        [SerializeField] float itemRadius;

        Spatula spatula;

        bool isCollapse = false;
        bool isActive = true;

        float rate;
        Color color = Color.white;

        public void OnInit(Vector3 center, float radius)
        {
            this.center = center;
            this.radius = radius;
            col.enabled = true;
        }
        public void SetRipeRate(float rate)
        {
            this.rate = rate;

            if (rate < 1)
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

        public void SetAlpha(float alpha)
        {
            color.a = alpha;
            rens[Mathf.FloorToInt(rate)].color = color;
        }

        public void SetRadius(float radius)
        {
            this.radius = radius;
        }

        public void SetControl(bool active)
        {
            this.isActive = active;
            col.enabled = active;
        }

        public void AddForce(Spatula spatula, Vector2 force)
        {
            this.spatula = spatula;
            if (isCollapse)
            {
                isCollapse = false;
                return;
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
                    force = Vector2.Reflect(force, (center - newPoint).normalized);
                    newPoint = speed * Time.deltaTime * force + transform.position;
                }
                transform.position = newPoint;
                yield return null;
            }

            //isCollapse = spoon.IsCollapse(newPoint, itemRadius);
            col.enabled = isActive;
        }

        [Button]
        public void Setup()
        {
            col = GetComponent<CircleCollider2D>();
            if (col == null) col = gameObject.AddComponent<CircleCollider2D>();

            rens = new SpriteRenderer[transform.childCount];
            for (int i = 0; i < transform.childCount; i++)
            {
                rens[i] = transform.GetChild(i).GetComponent<SpriteRenderer>();
            }

            rens[1].color = new Color(1, 1, 1, 0);
            rens[2].color = new Color(1, 1, 1, 0);
            itemRadius = col.radius * transform.localScale.x;
        }

        void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(center, radius);
        }
    }
}