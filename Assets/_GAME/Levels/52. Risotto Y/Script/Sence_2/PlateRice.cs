using UnityEngine;
using Link;

namespace LawNguyen.CookingGame.LRisottoY
{
    public class PlateRice : ItemMovingBase
    {
        [SerializeField] private Rice2 rice2;

        private void Start()
        {
            if (rice2 != null)
            {
                // Tắt SpriteRenderer và Collider của Rice2
                ToggleBox(false);
            }
        }

        public override void OnDrop()
        {
            base.OnDrop();

            
        }
        public override void OnBack()
        {
            base.OnBack();
        }

        public void ToggleBox(bool isOn)
        {
          

            var col = rice2.GetComponent<Collider2D>();
            if (col != null) col.enabled = isOn;

        
        }
    }
}
