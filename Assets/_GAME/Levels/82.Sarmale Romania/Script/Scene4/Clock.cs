using DG.Tweening;
using Link;
using Satisgame;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace HuyThanh.Cooking.SarmaleRomania
{
    public class Clock : ItemIdleBase
    {
        [SerializeField] MinuteHand minuteHand;
        [SerializeField] SortingGroup sortingGroup;
        [SerializeField] EmojiControl emoji;
        [SerializeField] Collider2D col;
        bool isShow, isStart;
        Tween tween;
        Vector3 savePoint;

        private void OnEnable()
        {
            minuteHand.onTime += OffCollition;
            minuteHand.onDone += OnDoneTime;
        }
        private void OnDisable()
        {
            minuteHand.onTime -= OffCollition;
            minuteHand.onDone -= OnDoneTime;
        }
        private void OnSavePoint()
        {
            savePoint = TF.position;
        }

        public override void OnClickDown()
        {
            base.OnClickDown();
            isShow = !isShow;
            if (isShow)
            {
                // phong to dong ho va dong ho di chuyen vao giua
                CameraControl.Instance.OnMove(TF.position + new Vector3(0, 0, -10));
                CameraControl.Instance.OnSize(1f);

                ///tat collider cua clock
                col.enabled = false;
                ///bat collider cua kim phut
                minuteHand.OnCollition(true);
                // tang layer cua dong ho len 100
                sortingGroup.sortingOrder = 100;
            }
            else
            {
                // scale ve lai binh thuong va xong back lai vi tri ban dau
                CameraControl.Instance.OnMove(new Vector3(0, 0, -10));
                float target = Link.Utilities.GetMapValue((float)Screen.height / (float)Screen.width, 1920f / 1080f, 1600f / 720f, 5, 6.25f);
                CameraControl.Instance.OnSize(target);
                // tat collider cua kim phut
                minuteHand.OnCollition(false);
                // tang layer cua dong ho len 0
                sortingGroup.sortingOrder = 0;
            }
        }
        public void OffCollition()
        {
            col.enabled = false;
            StartCoroutine(WaitForCameraMove());
            OnClickDown();
        }
        public void OnCollition()
        {
            col.enabled = true;
        }
        public void OnDoneTime()
        {
            isStart = false;
            emoji.ShowPositive();
        }
        public void OnActive()
        {
            isStart = true;
        }
        private void LateUpdate()
        {
            if (isStart)
            {
                minuteHand.OnRotation();
            }
        }

        IEnumerator WaitForCameraMove()
        {
            yield return WaitForSecondCache.Get(0.5f);
            isStart = true;
        }
    }

}