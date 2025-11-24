using Link;

namespace HuyThanh.Cooking.TunaSaladFrench
{

    public class MixItemMove : ItemMovingBase
    {
        public override void OnDone()
        {
            base.OnDone();
        }

        public override void OnClickDown()
        {
            base.OnClickDown();
            SoundControl.Ins.PlayFX(Fx.Click);
        }

        public override void OnClickTake()
        {
            base.OnClickTake();
            SoundControl.Ins.PlayFX(Fx.Take);
        }
    }

}