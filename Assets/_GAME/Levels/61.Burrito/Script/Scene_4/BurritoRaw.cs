using Link;
using System.Collections;
using UnityEngine;

namespace HuyThanh.Cooking.Burrito
{
    public class BurritoRaw : ItemIdleBase
    {
        [SerializeField] Collider2D collider;

        [SerializeField] BurritoDoneCut burritoPieceLeft, burritoPieceRight;

        [SerializeField] ItemAlpha burritoAlpha;

        [SerializeField] HintText hintCutBurrito;

        private bool isDone = false;

        public override bool OnTake(IItemMoving item)
        {
            if (item is KnifeBurrito)
            {
                collider.enabled = false;
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                item.ChangeState(KnifeBurrito.State.Cutting);
                StartCoroutine(WaitToShowDoneBurrito());
                return true;
            }
            return base.OnTake(item);
        }

        IEnumerator WaitToShowDoneBurrito()
        {
            yield return WaitForSecondCache.Get(0.8f);
            burritoAlpha.DoAlpha(0, 0.1f);
            burritoPieceLeft.gameObject.SetActive(true);
            burritoPieceLeft.OnMove(TF.position + Vector3.left * 0.35f, TF.rotation, 0.2f);
            burritoPieceLeft.OnSave(0.2f);

            burritoPieceRight.gameObject.SetActive(true);
            burritoPieceRight.OnMove(TF.position + Vector3.right * 0.35f, TF.rotation, 0.2f);
            burritoPieceRight.OnSave(0.2f);

            hintCutBurrito.OnActiveHint();
            isDone = true;
            LevelControl.Ins.NextHint();
        }

        public bool CheckDone()
        {
            return isDone;
        }
    }
}