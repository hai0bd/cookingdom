using DG.Tweening;
using Link;
using UnityEngine;

namespace HuyThanh.Cooking.SarmaleRomania
{


    public class Tong : ItemMovingBase
    {
        [SerializeField] Transform takeTF;
        [SerializeField] PotCabbage potCabbage;
        private bool isHaveItem = false;
        private PotCabbageRoll currentPotCabbageRoll;
        private PotRib currentPotRib;

        private void OnTriggerEnter2D(UnityEngine.Collider2D collision)
        {
            if (!isHaveItem && collision.TryGetComponent(out PotCabbageRoll potCabbageRoll) && potCabbage.IsState(PotCabbage.State.CabbageRolls))
            {
                isHaveItem = true;
                currentPotCabbageRoll = potCabbageRoll;
                potCabbageRoll.TF.SetParent(this.TF);
                potCabbageRoll.TF.DOLocalMove(takeTF.localPosition, 0.2f);
                potCabbageRoll.TF.DORotate(Vector3.zero, 0.2f);
                potCabbageRoll.TF.DOScale(1f, 0.2f);
                potCabbageRoll.ChangeOnTong();
                return;
            }

            if (isHaveItem && collision.TryGetComponent(out PotCabbageRollTarget potCabbageRollTarget) && potCabbageRollTarget.IsHaveItem == false && currentPotCabbageRoll != null)
            {
                isHaveItem = false;
                potCabbageRollTarget.SetHaveItem(true);
                currentPotCabbageRoll.TF.SetParent(potCabbageRollTarget.TF);
                currentPotCabbageRoll.TF.DOMove(potCabbageRollTarget.TF.position, 0.2f);
                currentPotCabbageRoll.TF.DORotateQuaternion(potCabbageRollTarget.TF.rotation, 0.2f);
                currentPotCabbageRoll.TF.DOScale(1f, 0.2f);
                currentPotCabbageRoll.OrderLayer = potCabbageRollTarget.TargetLayer;
                currentPotCabbageRoll.ChangeOnDish(potCabbageRollTarget.Type);

                currentPotCabbageRoll = null;
                return;
            }

            if (!isHaveItem && collision.TryGetComponent(out PotRib potRib) && potCabbage.IsState(PotCabbage.State.Ribs))
            {
                isHaveItem = true;
                currentPotRib = potRib;
                potRib.TF.SetParent(this.TF);
                potRib.TF.DOLocalMove(takeTF.localPosition, 0.2f);
                potRib.TF.DORotate(Vector3.zero, 0.2f);
                potRib.TF.DOScale(1f, 0.2f);
                potRib.ChangeOnTong();
                return;
            }

            if (isHaveItem && collision.TryGetComponent(out PotRibTarget potRibTarget) && potRibTarget.IsHaveItem == false && currentPotRib != null)
            {
                isHaveItem = false;
                potRibTarget.SetHaveItem(true);
                currentPotRib.TF.SetParent(potRibTarget.TF);
                currentPotRib.TF.DOJump(potRibTarget.TF.position, 1, 1, 0.2f);
                currentPotRib.TF.DORotateQuaternion(potRibTarget.TF.rotation, 0.2f);
                currentPotRib.TF.DOScale(1f, 0.2f);
                currentPotRib.ChangeOnDish();

                currentPotRib = null;
                return;
            }
        }
    }
}