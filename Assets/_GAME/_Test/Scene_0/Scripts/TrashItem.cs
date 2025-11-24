using UnityEngine;
using Link;
using UnityEngine.Events;

namespace HoangLinh.Cooking.Test
{
    public class TrashItem : ItemMovingBase
    {
        [SerializeField] TrashBin trashBin;
        [SerializeField] Animation anim;
        [SerializeField] string animGetItem;
        [SerializeField] UnityEvent throwEvent;

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
            throwEvent?.Invoke();
        }
    }
}