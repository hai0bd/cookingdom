using System.Collections;
using UnityEngine;
using DG.Tweening;
using Link;
using System;

namespace LawNguyen.CookingGame.LRisottoY
{
    public class PlateButter : ItemMovingBase
    {
        [SerializeField] private GameObject butterSR;
        public event Action buterInpot;
      

        public bool hasMovedToPot = false;

        public override void OnBack()
        {
            base.OnBack();
            SetOrder(-25);
        }

        public override void OnDrop()
        {
            base.OnDrop();
        }

        public void MoveButterInPot(Vector3 positionPot, Transform parent)
        {
            hasMovedToPot = true;
            butterSR.transform.DOMove(positionPot, 1f).OnComplete(() =>
            {
                butterSR.GetComponent<ButterSence3>().setlayerButter();

            });
            butterSR.transform.SetParent(parent);
            buterInpot?.Invoke();  
        }
    }
}
