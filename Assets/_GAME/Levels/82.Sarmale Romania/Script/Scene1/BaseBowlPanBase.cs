using DG.Tweening;
using Link;
using System.Collections;
using UnityEngine;

namespace HuyThanh.Cooking.SarmaleRomania
{
    public class BaseBowlPanBase : ItemIdleBase
    {
        [SerializeField] ItemAlpha squareSpice;
        public override bool OnTake(IItemMoving item)
        {
            if (item is PanBase panBase && panBase.IsState(PanBase.State.DoneMixing))
            {
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                item.ChangeState(PanBase.State.Pouring);
                StartCoroutine(WaitForDonePouring());
                return true;
            }
            return base.OnTake(item);
        }

        IEnumerator WaitForDonePouring()
        {
            squareSpice.transform.localScale = Vector3.zero;
            yield return WaitForSecondCache.Get(0.75f);
            squareSpice.DoAlpha(1, 1f);
            squareSpice.transform.DOScale(1f, 1f);
            yield return WaitForSecondCache.Get(0.5f);
            LevelControl.Ins.CheckStep(1f);
        }
    }

}