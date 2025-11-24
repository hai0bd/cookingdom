using Link;
using Sirenix.OdinInspector;
using UnityEngine;
namespace HuyThanh.Cooking.SarmaleRomania
{
    public class CabbageMoveTrash : ItemMovingBase
    {
        [SerializeField] private TrashBin trashBin;
        [SerializeField] private Cabbage cabbage;
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
            cabbage.OnDoneThrowRotten();
        }

        public void OnActive()
        {
            this.enabled = true;
            this.sprite.enabled = true;
            OnSavePoint();
        }

        [Button("Find Trash Bin and Cabbage")]
        public void Find()
        {
            trashBin = FindObjectOfType<TrashBin>();
            cabbage = FindObjectOfType<Cabbage>();
        }
    }
}

