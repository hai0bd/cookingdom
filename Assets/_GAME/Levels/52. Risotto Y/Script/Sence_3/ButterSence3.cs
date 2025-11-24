using UnityEngine;
using DG.Tweening;
using System;

namespace LawNguyen.CookingGame.LRisottoY
{
    public class ButterSence3 : MonoBehaviour
    {
        [Header("Melt Effects")]
        [SerializeField] private SpriteRenderer butterSR;
        [SerializeField] private SpriteRenderer buttermeltel;

        [Header("Liên kết")]
        [SerializeField] private StoveTapOn stoveTapOn;
   
        [SerializeField] private PlateButter plateButter; // Tham chiếu tới đối tượng PlateButter

        private bool hasMelted = false;

        private void OnEnable()
        {
            stoveTapOn.PotisTurnedOn += Buttermetel;
            plateButter.buterInpot += Buttermetel;
        }

        private void OnDisable()
        {
            stoveTapOn.PotisTurnedOn -= Buttermetel;
            plateButter.buterInpot -= Buttermetel;
        }

        public void setlayerButter()
        {
            butterSR.sortingOrder=-25;
        }
        private void Buttermetel()
        {
            if (hasMelted) return;
            if (!plateButter.hasMovedToPot || !stoveTapOn.Ison) return;

            hasMelted = true;

            // Ẩn bơ gốc

            DOVirtual.DelayedCall(1f, () =>
            {
                butterSR.DOFade(0f, 2f);
                butterSR.gameObject.transform.DOLocalRotate(new Vector3(45f, 0, 0), 1f);
                butterSR.gameObject.transform.DOScale(new Vector3(0.5f, 0.5f, 0.5f), 2f);


            });

          
          
               
             DOVirtual.DelayedCall(1.5f, () =>
                {

                    buttermeltel.gameObject.transform.DOScale( Vector3.one *0.7f, 2f);
                    buttermeltel.DOFade(1f, 2f);


                });
              DOVirtual.DelayedCall(3.5f, () =>
            {
                buttermeltel.DOKill();



            });

        }
    }
}
