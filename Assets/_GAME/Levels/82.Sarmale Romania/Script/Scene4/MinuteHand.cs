using Link;
using System;
using UnityEngine;

namespace HuyThanh.Cooking.SarmaleRomania
{
    public class MinuteHand : ItemIdleBase
    {
        [SerializeField] float min = 0, max = 100;
        [SerializeField] Collider2D col;
        [SerializeField] Transform kimGio;

        public Action onTime, onDone;

        Vector3 mousePos;

        bool isClick;

        float eulerKimPhut;
        float eulerKimGio;
        float time;

        float currentAngle = 0;
        public override void OnClickDown()
        {
            base.OnClickDown();
            mousePos = LevelControl.Ins.GetPoint();
            if (Input.GetMouseButtonDown(0))
            {
                isClick = true;
            }
        }

        private void LateUpdate()
        {
            if (isClick && Input.GetMouseButtonUp(0))
            {
                isClick = false;
                CheckTime();
            }

            if (isClick)
            {
                Vector3 newMousePos = LevelControl.Ins.GetPoint();
                Vector3 currentDir = newMousePos - TF.position;
                Vector3 previousDir = mousePos - TF.position;

                float angle = Vector3.SignedAngle(previousDir, currentDir, Vector3.forward);
                angle = Mathf.Clamp(angle, -10, 0);
                if (angle < 0 && currentAngle + angle >= -360f)
                {
                    currentAngle += angle;
                    TF.rotation = Quaternion.Euler(0, 0, currentAngle);
                    kimGio.rotation = Quaternion.Euler(0, 0, currentAngle / 12);
                }

                mousePos = newMousePos;
            }
        }

        public void OnRotation()
        {
            time += Time.deltaTime;
            TF.eulerAngles = Vector3.Lerp(new Vector3(0, 0, eulerKimPhut), new Vector3(0, 0, 360), time / 10f);
            kimGio.eulerAngles = Vector3.Lerp(new Vector3(0, 0, eulerKimGio), new Vector3(0, 0, 360), time / 10f);
            if (time >= 10)
            {
                onDone?.Invoke();
            }
        }

        public void OnCollition(bool isOn)
        {
            col.enabled = isOn;
        }
        private void CheckTime()
        {
            if (currentAngle >= min && currentAngle <= max)
            {
                onTime?.Invoke();
                eulerKimPhut = TF.rotation.eulerAngles.z;
                eulerKimGio = kimGio.rotation.eulerAngles.z;
            }
        }
    }
}