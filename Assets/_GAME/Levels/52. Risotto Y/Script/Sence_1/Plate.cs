using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Link;
using UnityEngine.Experimental.GlobalIllumination;
using DG.Tweening;
using Satisgame;
using System;

namespace LawNguyen.CookingGame.LRisottoY
{
    public class Plate : ItemIdleBase
    {

        public enum PlateType {musell,shrimp,clam,onion,parsley,safron,gralic };
        public event Action<Plate> OnCompleteEvent;
      
        [SerializeField] PlateType type;

        private bool hasItem = false; // CHẶN các item tiếp theo

        public override bool OnTake(IItemMoving item)
        {
            if (hasItem) return false; // Nếu đã có item thì không nhận nữa

            // Động vật biển
            if ((item is Museel museel && museel.IsState(Museel.State.movingplate))&& type==PlateType.musell  ||
                (item is Clamm clam && clam.IsState(Clamm.State.clammovinginplate)) && type == PlateType.clam ||
                (item is Shrimp shrimp && shrimp.IsState(Shrimp.State.Shrimpmovinginplate) && type == PlateType.shrimp))
              
            {
                HandleItemDrop(item);
                return true;
            }

            // Gia vị đã cắt
            if ((item is Onion onion && onion.IsState(Onion.State.Pieced)) && type == PlateType.onion ||
                (item is Gralic gralic && gralic.IsState(Gralic.State.Pieced)) && type == PlateType.gralic ||
                  (item is Parsley parsley && parsley.IsState(Parsley.State.minece) && type == PlateType.parsley)
                )
            {
                HandleItemDrop(item);
                return true;
            }

            return false;
        }

        private void HandleItemDrop(IItemMoving item)
        {  

            hasItem = true; // Đánh dấu đã nhận item đầu tiên
            item.OnMove(TF.position, Quaternion.identity, 0.2f);
            DOVirtual.DelayedCall(0.2f, () => item.OrderLayer = 0);
         
            item.OnDone();
   
            OnCompleteEvent?.Invoke(this);
        }

        public void Clear()
        {
            hasItem = false; // Dùng khi muốn cho phép nhận item mới (sau khi plate được reset chẳng hạn)
        }
    }

}
