using UnityEngine;
using Link;
using Satisgame;

namespace LawNguyen.CookingGame.LRisottoY
{
    public class StoveTap : ItemIdleBase
    {
        [SerializeField] private Pot pot;
        private bool isOn = false;
        [SerializeField] Bowlrice bowlrice;
        [SerializeField] EmojiControl emoji;
        bool isdone =false; 

        // Chỉ hoàn thành khi đã tắt bếp
        public override bool IsDone => !isOn  && bowlrice.IsDone;

        public override void OnClickDown()
        {
            // Đảo trạng thái
            isOn = !isOn;

            // Gọi bếp nếu đã gán
            if (pot != null)
            {
                pot.StoveTapON(isOn);
                if (IsDone) emoji.ShowPositive();
            }
            else
            {
                Debug.LogWarning("StoveTap: Pot not assigned.");
            }

            // Kiểm tra bước nếu hệ thống sẵn sàng
            if (LevelControl.Ins != null && !isdone && IsDone)
            {
                LevelControl.Ins.CheckStep();
               
                isdone = true;  
            }
           
        }
    }
}
