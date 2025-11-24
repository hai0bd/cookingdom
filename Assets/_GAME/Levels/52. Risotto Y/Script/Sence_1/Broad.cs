using DG.Tweening;
using Link;
using Link.Cooking.Spageti;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LawNguyen.CookingGame.LRisottoY
{
    public class Broad : ItemIdleBase
    {
        ItemMovingBase item;
       public bool isHaveItem => item != null && Vector2.Distance(item.TF.position, TF.position) < 0.3f;

        public override bool OnTake(IItemMoving incoming)
        {
            // Nếu đã có item trên Broad
            if (isHaveItem)
            {
                // Nếu là dao thì đưa dao vào xử lý item hiện có
                if (incoming is Knift knife)
                {
                    knife.SetOrder(5);
                    return item.OnTake(knife);
                }

                // Nếu là item khác thì từ chối
                incoming.OnBack();
                return false;
            }

            // --- Nếu chưa có item nào --- //

            // Động vật biển
            if (incoming is Museel museel && museel.IsState(Museel.State.musselafterwash))
            {
                SetItem(museel, Museel.State.museelmoveinBroad, 0.5f);
                return true;
            }

            if (incoming is Clamm clam && clam.IsState(Clamm.State.clamafterwash))
            {
                SetItem(clam, Clamm.State.clammoveinbroad, 0.2f);
                return true;
            }

            if (incoming is Shrimp shrimp && shrimp.IsState(Shrimp.State.Shrimpafterwash))
            {
                SetItem(shrimp, Shrimp.State.Shrimpmoveinbroad, 0.2f);
                return true;
            }

            // Nguyên liệu thô
            if (incoming is Gralic gralic && gralic.IsState(Gralic.State.idle))
            {
                SetItem(gralic, Gralic.State.inbroad, 0.2f);
                return true;
            }

            if (incoming is Onion onion && onion.IsState(Onion.State.idle))
            {
                SetItem(onion, Onion.State.inbroad, 0.2f);
                return true;
            }

            if (incoming is Parsley parsley && parsley.IsState(Parsley.State.idle))
            {
                SetItem(parsley, Parsley.State.inbroad, 0.2f);
                return true;
            }

            // Không phù hợp → OnBack
            incoming.OnBack();
            return false;
        }

        private void SetItem(ItemMovingBase newItem, System.Enum state, float duration)
        {
            item = newItem;
            item.OnMove(TF.position + new Vector3(0.2f, 0, 0), Quaternion.identity, duration);
            item.OnSave(duration);
            item.ChangeState(state);
        }

        public override void OnClickDown()
        {
            base.OnClickDown();

            if (!isHaveItem) return;

            // Xử lý khi click vào Broad nếu có nguyên liệu được xử lý
            if (item.IsState(Onion.State.Sliced) ||
                item.IsState(Gralic.State.Sliced) ||
                item.IsState(Gralic.State.Peel) ||
                item.IsState(Gralic.State.Cuting) ||
                item.IsState(Parsley.State.inbroad))
            {
                if (item is Onion || item is Gralic || item is Parsley)
                {
                    item.OnClickDown();
                }
            }
        }
    }
}
