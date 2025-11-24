using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Link;
using DG.Tweening;

namespace HoangLinh.Cooking.Test
{
    public class BeanBowl : ItemIdleBase
    {
        [SerializeField] ItemAlpha beanAlpha;
        [SerializeField] ParticleSystem vfxBlink;

        public override bool OnTake(IItemMoving item)
        {
            if (item is BeanBasket && item.IsState(BeanBasket.State.DoneWashing))
            {
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                item.ChangeState(BeanBasket.State.Pouring);

                DOVirtual.DelayedCall(1f, () =>
                {
                    vfxBlink.Play();
                    beanAlpha.DoAlpha(1f, 0.1f);
                });
                return true;
            }

            return base.OnTake(item);
        }


        
    }
}