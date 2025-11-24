using Link;
using UnityEngine;
namespace HuyThanh.Cooking.SarmaleRomania
{
    public class Core : ItemMovingBase
    {
        [SerializeField] private TrashBin trashBin;
        [SerializeField] private Cabbage cabage;
        [SerializeField] Animation anim;
        public override bool IsCanMove => true;
        public void OnActive()
        {
            anim.Play("Bounce");
            collider.enabled = true;
            sprite.enabled = true;
        }
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
            cabage.OnDoneThrowRottenCore();
        }
    }
}



