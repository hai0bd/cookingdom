using Link;

namespace HuyThanh.Cooking.TunaSaladFrench
{
    public class Grater : ItemMovingBase
    {
        public override bool IsCanMove => true;

        public override void OnClickDown()
        {
            base.OnClickDown();
            SoundControl.Ins.PlayFX(Fx.Click);
        }
    }
}