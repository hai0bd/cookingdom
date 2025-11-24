using Link;

using UnityEngine;

namespace HuyThanh.Cooking.TunaSaladFrench
{
    public class ScoopScene3 : ItemMovingBase
    {
        [SerializeField] private Transform scoopTransform;

        private ItemMovingBase item;

        public override bool IsCanMove => true;


        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out Nut nut))
            {
                if (nut.IsState(Nut.State.Cooked) && item == null)
                {
                    nut.OrderLayer = 2;
                    nut.TF.SetParent(scoopTransform);
                    nut.TF.localPosition = Vector3.zero;
                    nut.ChangeAnim("NutItemScale");
                    item = nut;
                }
            }


            if (collision.TryGetComponent(out PanOvenIdle panOven))
            {
                if (item != null && item is Nut)
                {
                    if (panOven.OnTake(item))
                    {
                        item = null;
                    }

                }

            }
        }
    }
}
