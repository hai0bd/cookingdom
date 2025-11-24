using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Link;
using System.ComponentModel;

namespace LawNguyen.CookingGame.LRisottoY
{
    public class PlateSeefood : ItemMovingBase
    {
        public enum Platetype { Shrimp, Mussel, Clam }

        [SerializeField] public Platetype platetype;
        [SerializeField] private List<Transform> seafoodItems;
         private List<Transform> ListtranformMove = new List<Transform>();
        int layeritemcurrent = 0;
       
    
       

        public void MoveAndDropToPot(List<Transform> listpointshrimp, List<Transform> listpointmuseel, List<Transform> listpointClam, List<Transform> listallseafood)
        {
            switch (platetype)
            {
                case Platetype.Shrimp:  
                ListtranformMove = listpointshrimp;
                    layeritemcurrent = -5;
                    break;
                case Platetype.Mussel:  
                  ListtranformMove= listpointmuseel;  
                    layeritemcurrent = -15;
                  break;
                  case Platetype.Clam:
                  ListtranformMove=listpointClam;
                    layeritemcurrent = -10;
                    break;
                    default:
                    break;
            }




            for (int i = 0; i < seafoodItems.Count; i++)
            {
                float delay = i * 0.2f;
                layeritemcurrent++;

                Transform item = seafoodItems[i];
                Transform target = ListtranformMove[i];
                int currentLayer = layeritemcurrent;

                item.SetParent(null);
                item.DOLocalRotate(target.rotation.eulerAngles, 0.5f).SetDelay(delay);

                // Gọi SetLayer khi move hoàn tất (đúng thời điểm)
                item.DOMove(target.position, 0.5f)
                    .SetDelay(delay)
                    .OnComplete(() =>
                    {
                        item.gameObject.GetComponent<SeafoodStirEffect>().SetLayer(currentLayer);
                       
                    });

                listallseafood.Add(item);
            }


        }




        public override void OnBack() => base.OnBack();
        public override void OnDrop() => base.OnDrop();
    }
}
