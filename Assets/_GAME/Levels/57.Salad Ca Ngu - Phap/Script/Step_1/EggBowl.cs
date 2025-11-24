using Link;
using System.Collections.Generic;
using UnityEngine;

namespace HuyThanh.Cooking.TunaSaladFrench
{
    public class EggBowl : ItemIdleBase
    {
        [SerializeField] ItemMovingBase item;

        [SerializeField] private List<GameObject> activeSpriteEgg;

        [SerializeField] private ParticleSystem vfxBlink;
        [SerializeField] Animation anim;
        [SerializeField] string catHandSteal;

        public override bool OnTake(IItemMoving item)
        {
            if (item is EggCut && item.IsState(EggCut.State.Spice))
            {
                item.ChangeState(EggCut.State.Done);
                ActiveSprite();
                if (activeSpriteEgg.Count == 0)
                {
                    vfxBlink.Play();
                    anim.Play(catHandSteal);
                }

                LevelControl.Ins.CheckStep(1f);
                return true;
            }
            item.OnBack();
            return false;
        }

        private void ActiveSprite()
        {
            activeSpriteEgg[0].SetActive(true);
            activeSpriteEgg[1].SetActive(true);

            activeSpriteEgg.RemoveAt(1);
            activeSpriteEgg.RemoveAt(0);
        }
    }
}