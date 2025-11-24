using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Link.Cooking.Spageti
{
    public class ScallopSand : ItemDecor
    {
        public override bool IsCanMove => base.IsCanMove && IsControl();

        bool isActive = false;

        public override void OnClickDown()
        {
            base.OnClickDown();
            isActive = true;
        }

        bool IsControl()
        {
            if (isActive) return true;
            return LevelControl.Ins.IsHaveObject<Sand>(TF.position) == null;
        }

        public override void OnMove(Vector3 pos, Quaternion rot, float time)
        {
            TF.DOJump(pos, 0.5f, 1, time * 2);
            TF.DORotateQuaternion(rot, time * 2);
        }
    }
}