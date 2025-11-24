using Link;
using UnityEngine;
using UnityEngine.Events;

namespace HuyThanh.Cooking.Burrito
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

            if (anim != null && !string.IsNullOrEmpty(animGetItem))
            {
                anim.Play(animGetItem);
            }
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
