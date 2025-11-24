using Link;
using UnityEngine;
namespace HuyThanh.Cooking.SarmaleRomania
{
    public class DillMoveTrash : ItemMovingBase
    {
        public enum DillType { dill, dillpice }
        [SerializeField] private DillType dillType;
        [SerializeField] private TrashBin trashBin;
        [SerializeField] private Dill dill;
        [SerializeField] private DillPice dillPice;
        public override bool IsCanMove => true;



        public override void OnClickDown()
        {
            base.OnClickDown();
            trashBin.OnNeedTrashBin();
        }

        public override void OnDrop()
        {
            base.OnDrop();
            trashBin.OnNoNeedTrashBin();
        }

        public void OnThrow()
        {
            if (dillType == DillType.dill)
            {
                dill.OnDoneThrowRotten();
            }
            if (dillType == DillType.dillpice)
            {
                dillPice.OnDoneThrowRotten();
            }
        }
    }
}
