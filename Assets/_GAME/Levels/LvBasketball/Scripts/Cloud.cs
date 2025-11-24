using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnhPD
{
    public class Cloud : MonoBehaviour
    {
        [SerializeField] private float offsetX;
        [SerializeField] private float initSpeed = 1f, minY;
        [SerializeField] private int dir = 1;
        [SerializeField] private bool isRandomY = true;

        private float speed;

        Vector3 minScreenBounds;
        Vector3 maxScreenBounds;
        private void Start()
        {
            minScreenBounds = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane));
            maxScreenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.nearClipPlane));
            RandomSpeed();
        }
        public void OnInit(float initSpeed, int dir, float minY)
        {
            this.initSpeed = initSpeed;
            this.dir = dir;
            this.minY = minY;

            RandomSpeed();
        }
        private void Update()
        {
            transform.position += transform.right * speed * dir * Time.deltaTime;
            float y = isRandomY ? Random.Range(minY, maxScreenBounds.y) : transform.position.y;
            if (dir > 0 && transform.position.x - offsetX > maxScreenBounds.x)
            {
                transform.position = new Vector2(minScreenBounds.x - offsetX, y);
                RandomSpeed();
            }
            if (dir < 0 && transform.position.x + offsetX < minScreenBounds.x)
            {
                transform.position = new Vector2(maxScreenBounds.x + offsetX, y);
                RandomSpeed();
            }
        }

        private void RandomSpeed()
        {
            speed = initSpeed * Random.Range(0.75f, 1.25f);
        }
    }

}
