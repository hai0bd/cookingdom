using DG.Tweening;
using Link;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

namespace HuyThanh.Cooking.SarmaleRomania
{
    public class BaseCornBowl : ItemIdleBase
    {
        [SerializeField] Vector3 moveInPos;

        [SerializeField] Animation anim;
        [SerializeField] string getItemAnim;
        [SerializeField] ParticleSystem smokeVFX;


        [Button]
        public void MoveOut(Vector3 vec)
        {
            TF.position = TF.position + vec;
        }

        [Button]
        public void MoveIn()
        {
            TF.DOMove(moveInPos, .5f);
        }

        public override bool OnTake(IItemMoving item)
        {
            if (item is PotCorn && item.IsState(PotCorn.State.DoneMixing))
            {
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                item.ChangeState(PotCorn.State.Pouring);
                StartCoroutine(WaitForDonePouring());
                return true;
            }
            return base.OnTake(item);
        }

        IEnumerator WaitForDonePouring()
        {
            yield return WaitForSecondCache.Get(1f);
            smokeVFX.Play();
            anim.Play(getItemAnim);
        }
    }
}