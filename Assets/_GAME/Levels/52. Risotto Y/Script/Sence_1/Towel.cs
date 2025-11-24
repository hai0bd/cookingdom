using Link;
using Link.Turkey;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LawNguyen.CookingGame.LRisottoY
{
    public class Towel : ItemMovingBase
    {
        Museel museel1;
        Clamm Clamm;
        Shrimp shrimp;

        public bool ismoving = false;
        bool colisionON = false;
        Vector3 point;

        public override void OnDrop()
        {
            base.OnDrop();
        }

        private void LateUpdate()
        {
            if (ismoving && museel1 != null && museel1.IsState(Museel.State.museelmoveinBroad) && colisionON)
            {
                if (Vector3.Distance(TF.position, point) > 0.05f)
                {
                    point = TF.position;
                    museel1.AddClean();
                }
            }

            if (ismoving && Clamm != null && Clamm.IsState(Clamm.State.clammoveinbroad) && colisionON)
            {
                if (Vector3.Distance(TF.position, point) > 0.05f)
                {
                    point = TF.position;
                    Clamm.AddClean();
                }
            }

            if (ismoving && shrimp != null && shrimp.IsState(Shrimp.State.Shrimpmoveinbroad) && colisionON)
            {
                if (Vector3.Distance(TF.position, point) > 0.05f)
                {
                    point = TF.position;
                    shrimp.AddClean();
                }
            }
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            // Gán đối tượng đang chạm nếu chưa có
            if (museel1 == null)
            {
                museel1 = collision.GetComponent<Museel>();
                if (museel1 != null) colisionON = true;
            }

            if (Clamm == null)
            {
                Clamm = collision.GetComponent<Clamm>();
                if (Clamm != null) colisionON = true;
            }

            if (shrimp == null)
            {
                shrimp = collision.GetComponent<Shrimp>();
                if (shrimp != null) colisionON = true;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.GetComponent<Museel>() != null && museel1 != null)
                museel1 = null;

            if (collision.GetComponent<Clamm>() != null && Clamm != null)
                Clamm = null;

            if (collision.GetComponent<Shrimp>() != null && shrimp != null)
                shrimp = null;

            // Nếu tất cả đều null thì tắt cờ
            if (museel1 == null && Clamm == null && shrimp == null)
                colisionON = false;
        }
    }
}
