using Link;
using UnityEngine;


namespace HuyThanh.Cooking.TunaSaladFrench
{
    public class Scoop : ItemMovingBase
    {
        [SerializeField] Transform scoopTransform;

        public override bool IsCanMove => true;

        private ItemMovingBase item;


        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.TryGetComponent(out Bean bean))
            {
                if (bean.IsState(Bean.State.Spice) && item == null && !LevelControl.Ins.IsHaveObject<NapNoi>(Vector3.one * 0.2f))
                {
                    bean.TF.SetParent(scoopTransform);
                    bean.TF.localPosition = Vector3.zero;
                    bean.ChangeAnim("CookingItemScale");
                    item = bean;
                }
            }

            if (collision.TryGetComponent(out Egg egg))
            {
                if (egg.IsState(Egg.State.Cooked) && item == null && !LevelControl.Ins.IsHaveObject<NapNoi>(Vector3.one * 0.2f))
                {
                    egg.TF.SetParent(scoopTransform);
                    egg.TF.localPosition = Vector3.zero;
                    item = egg;
                }
            }

            if (collision.TryGetComponent(out Potato potato))
            {
                if (potato.IsState(Potato.State.Spice) && item == null && !LevelControl.Ins.IsHaveObject<NapNoi>(Vector3.one * 0.2f))
                {
                    potato.TF.SetParent(scoopTransform);
                    potato.TF.localPosition = Vector3.zero;
                    potato.ChangeAnim("CookingItemScale");
                    item = potato;
                }
            }

            if (collision.TryGetComponent(out Bowl bowl))
            {
                if (item != null && (item is Bean || item is Potato))
                {
                    if (bowl.OnTake(item))
                    {
                        item = null;
                    }
                }

            }

            if (collision.TryGetComponent(out CuttingBoard cuttingBoard))
            {
                if (item != null && item is Egg)
                {
                    if (cuttingBoard.OnTake(item))
                    {
                        item = null;
                    }
                }
            }
        }

        public override void OnClickDown()
        {
            base.OnClickDown();
            SoundControl.Ins.PlayFX(Fx.Click);
        }

        public override void OnClickTake()
        {
            base.OnClickTake();

            SoundControl.Ins.PlayFX(Fx.Take);
        }

        public override void OnDrop()
        {
            base.OnDrop();
            OnBack();
        }
    }

}
