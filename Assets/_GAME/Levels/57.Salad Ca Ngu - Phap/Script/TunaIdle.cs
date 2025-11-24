using DG.Tweening;
using Link;
using UnityEngine;

namespace HuyThanh.Cooking.TunaSaladFrench
{
    public class TunaIdle : ItemIdleBase
    {
        [SerializeField] Tuna itemTuna; // lay hop ca hoi vao day

        [SerializeField] Collider2D collider;

        private bool IsEmpty => itemTuna == null || Vector2.Distance(itemTuna.TF.position, TF.position) > 0.2f;


        private float countingClickDown = 0; ///dem so lan da cham vao hop de mo nap

        public override void OnClickDown()
        {
            countingClickDown++;

            if (countingClickDown > 6)
            {
                return;
            }
            SoundControl.Ins.PlayFX(Fx.Click);

            itemTuna.OnClickIdle();

            if (countingClickDown == 3)
            {
                itemTuna.ChangeState(Tuna.State.OpenCap);

                collider.enabled = false;

                DOVirtual.DelayedCall(1.2f, () =>
                {
                    collider.enabled = true;
                });
                return;
            }

            else if (countingClickDown == 6)
            {
                itemTuna.ChangeState(Tuna.State.RemoveCap);
                collider.enabled = false; /// tat box khi da su dung xong
                return;
            }

            base.OnClickDown();
        }

    }

}
