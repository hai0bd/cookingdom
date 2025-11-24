using DG.Tweening;
using Link;
using UnityEngine;

namespace HuyThanh.Cooking.Burrito
{
    public class BowlRedBean : ItemIdleBase
    {
        [SerializeField] ItemAlpha beanAlpha;
        [SerializeField] ParticleSystem vfxBlink;

        [SerializeField] HintText redBeanHintText;
        public override bool OnTake(IItemMoving item)
        {
            if (item is Colander && item.IsState(Colander.State.DoneWash))
            {
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                item.ChangeState(Colander.State.Pouring);

                redBeanHintText.OnActiveHint();

                DOVirtual.DelayedCall(1f, () =>
                {
                    vfxBlink.Play();
                    SoundControl.Ins.PlayFX(Fx.DoneSomething);
                    beanAlpha.DoAlpha(1f, 0.1f);
                });
                return true;
            }
            return base.OnTake(item);
        }
    }

}
