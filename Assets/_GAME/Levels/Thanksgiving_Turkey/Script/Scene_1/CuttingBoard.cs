using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link.Turkey
{
    public class CuttingBoard : ItemIdleBase
    {
        IItemMoving itemMoving;
        [SerializeField] ItemMovingBase startItem;
        //[SerializeField] Collider2D collider;

        private void Start()
        {
            if (startItem != null)
            {
                OnTake(startItem);
            }
        }

        public override bool OnTake(IItemMoving item)
        {
            if (item is Tissue) return base.OnTake(item);
            if (item is Knife or Scraper && itemMoving != null && itemMoving is not Chicken)
            {
                //neu la dao thi cat got
                return itemMoving.OnTake(item);
            }

            //xoa thang cu dang o day
            if (itemMoving != null && Vector2.Distance(TF.position, itemMoving.TF.position) > 0.5f) itemMoving = null;
            if (itemMoving != null && itemMoving != item)
            {
                //neu dang co thang khac thi back ve
                item.OnBack();
                return false;
            }
            if (itemMoving == null && item is Fruit || item is Chicken)
            {
                //neu la fruid hoac chicken thi vao
                itemMoving = item;
                itemMoving.OnMove(TF.position, Quaternion.identity, 0.2f);

                if (item is Chicken && item.IsState(Chicken.State.Water))
                {
                    item.ChangeState(Chicken.State.NeedClean);
                }
                if (item is Chicken && item.IsState(Chicken.State.Prepare))
                {
                    item.ChangeState(Chicken.State.Marinate_1);
                }
                return true;
            }
            //if (item is Chicken && item.IsState(Chicken.State.Washed))
            //{
            //    itemMoving = item;
            //    itemMoving.OnMove(transform.position, Quaternion.identity, 0.2f);
            //}

            return false;
        }

        //public bool CheckItemContain()
        //{
        //    Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, Vector2.one * 2, 0);
        //    for (int i = 0; i < hits.Length; i++)
        //    {
        //        if (collider != hits[i])
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        //private void OnDrawGizmos()
        //{
        //    Gizmos.DrawCube(transform.position, Vector2.one * 2);
        //}

    }

}