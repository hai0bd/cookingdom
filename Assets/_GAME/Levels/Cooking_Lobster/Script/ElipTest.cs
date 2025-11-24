using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link
{
    public class ElipTest : MonoBehaviour
    {
        [SerializeField] Transform[] targets;
        [SerializeField] Ellipse2D[] elips;

        private void Start()
        {
            elips = new Ellipse2D[targets.Length];
            for (int i = 0; i < targets.Length; i++)
            {
                elips[i] = new Ellipse2D(1, 0.5f, targets[i].position);
            }
        }

        float time;

        private void Update()
        {
            time += Time.deltaTime;
            if (time > 2) time -= 2;

            for (int i = 0; i < targets.Length; i++)
            {
                targets[i].position = elips[i].Evaluate(time / 2);
            }
        }

        private void OnDrawGizmos()
        {
            //new Ellipse2D(1, 0.5f).OnDrawGizmos();
            //new Ellipse2D(1, 0.5f, new Vector2(0,1)).OnDrawGizmos();
            //new Ellipse2D(1, 0.5f, new Vector2(0,4)).OnDrawGizmos();
            //new Ellipse2D(1, 0.5f, new Vector2(1,4)).OnDrawGizmos();

            //elips = new Ellipse2D[targets.Length];
            //for (int i = 0; i < targets.Length; i++)
            //{
            //    elips[i] = new Ellipse2D(1, 0.5f, targets[i].position);
            //}
            foreach (var item in elips)
            {
                item.OnDrawGizmos(transform.position);
            }
        }
    }
}