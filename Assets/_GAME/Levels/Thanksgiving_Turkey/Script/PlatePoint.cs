using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link.Turkey
{
    public class PlatePoint : ItemIdleBase
    {
        [SerializeField] ItemName[] suitables;
        [SerializeField] ItemMovingBase itemStart;
        [SerializeField] Collider2D collider;

        private void Start()
        {
            if (itemStart != null)
            {
                OnTake(itemStart);
            }
        }

        //them dieu kien xem plaet nao o dau
        public int GetLayer => 0;

        private bool isFull = false;
        public override bool IsDone => isFull;

        public override bool OnTake(IItemMoving item)
        {
            //Debug.Log(IsSuitable(item as Plate));
            if (item is Plate && IsSuitable(item as Plate) && !IsDone)
            {
                item.OnMove(transform.position, Quaternion.identity, 0.2f);
                item.ChangeState(Plate.State.Ready);
                isFull = true;
                collider.enabled = false;
                return true;
            }
            return false;
        }

        private bool IsSuitable(Plate plate)
        {
            for (int i = 0; i < suitables.Length; i++)
            {
                for (int j = 0; j < plate.ItemNames.Length; j++)
                {
                    //Debug.Log($"{j} {plate.ItemNames.Length}");
                    if (suitables[i] == plate.ItemNames[j])
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        protected override void Editor()
        {
            base.Editor();
            collider = GetComponent<Collider2D>();
        }
    }

}