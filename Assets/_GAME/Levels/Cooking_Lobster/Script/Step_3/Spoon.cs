using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link.Cooking.Lobster
{
    public class Spoon : ItemMovingBase
    {
        [SerializeField] PanSpice panSpice;
        [SerializeField] Transform pourPoint;
        Vector3 checkPoint;
        
        private void LateUpdate()
        {
            if (checkPoint != pourPoint.position && Vector2.Distance(checkPoint, pourPoint.position) > 0.1f && LevelControl.Ins.IsHaveObject<PanSpice>(pourPoint.position))
            {
                checkPoint = pourPoint.position;
                panSpice.SetSpice();
                if (!SoundControl.Ins.IsPlaying(LevelStep_1.Fx.Mix))
                {
                    SoundControl.Ins.PlayFX(LevelStep_1.Fx.Mix);
                }
            }
            else
            {
                SoundControl.Ins.StopFX(LevelStep_1.Fx.Mix);
            }
        }

        public override void OnClickDown()
        {
            base.OnClickDown();
            SoundControl.Ins.PlayFX(LevelStep_1.Fx.Click);
        }

        public override void OnDrop()
        {
            base.OnDrop();
            OnBack();
        }
    }
}