using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Link;
using Unity.VisualScripting;
using DG.Tweening;
namespace LawNguyen.CookingGame.LRisottoY
{
    public class Pasley3 : ItemMovingBase
    {
        [SerializeField] List<GameObject> pasley = new List<GameObject>();
        [SerializeField] Transform Tfback;
        int i=-1;
        public void Droppassley()
        {
            i++;
            if (i <= pasley.Count)
            {
                pasley[i].SetActive(false);
            }
        }
        public void ONmovePLate()
        {
            transform.DOMove(Tfback.position, 0.5f);
        }

        public override void OnDrop()
        {
            SetOrder(100);
            base.OnDrop();
        }
    }

}

