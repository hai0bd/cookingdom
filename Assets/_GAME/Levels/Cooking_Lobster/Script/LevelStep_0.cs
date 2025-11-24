using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Link.Cooking.Lobster
{
    public class LevelStep_0 : LevelStepBase
    {
        [SerializeField] GameObject background;
        public override bool IsDone()
        {
            return true;
        }

        public override void OnFinish(Action action)
        {
            LevelControl.Ins.OnBlackSquare(1.5f, SetCam, null, 1);
            DOVirtual.DelayedCall(0.75f, () =>
            {
                base.OnFinish(action);
            });
        }

        public void SetCam()
        {
            CameraControl.Instance.OnMove(Vector3.forward * -10, .1f);
            CameraControl.Instance.OnSize(CameraControl.Instance.orthographicSize + 1, 0.1f);
            gameObject.SetActive(false);
            background.SetActive(true);
        }
    }
}