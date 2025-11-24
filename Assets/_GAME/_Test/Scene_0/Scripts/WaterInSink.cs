using DG.Tweening;
using Link;
using UnityEngine;

namespace HoangLinh.Cooking.Test
{
    public class WaterInSink : ItemIdleBase
    {
        [SerializeField] ItemAlpha waterAlpha;
        [SerializeField] ItemAlpha waterFlowAlpha;

        [SerializeField] SinkCover sinkCover;
        [SerializeField] ParticleSystem vfxSplashWater;
        ItemMovingBase item;

        public bool isWater = false;
        bool isWaterFlow = false;
        bool isWaterClose => sinkCover != null && Vector2.Distance(sinkCover.TF.position, TF.position) < 0.75f;
        public bool haveItem => item != null && Vector2.Distance(item.TF.position, TF.position) < 0.2f;
        [SerializeField] bool haveItemDebug;

        protected virtual void Update()
        {
            haveItemDebug = haveItem;

        }

        public override bool OnTake(IItemMoving item)
        {
            if (item is SinkCover)
            {
                this.sinkCover = item as SinkCover;
                if (Vector2.Distance(item.TF.position, TF.position) < 0.75f)
                {
                    item.OnMove(TF.position + Vector3.down * (0.44f + 0.075f) + Vector3.right * (0.55f - 0.075f), Quaternion.identity, 0.2f);
                    DOVirtual.DelayedCall(0.2f, () => OnCloseWater());
                    item.OrderLayer = -1;
                }
                else
                {
                    OnCloseWater();
                }
                return true;
            }

            if (item is DirtyItems && item.IsState(DirtyItems.State.Dirty) && isWater && !haveItem)
            {
                this.item = item as ItemMovingBase;
                item.SetOrder(-7);
                item.OnMove(TF.position + Vector3.left * 0.1f, Quaternion.identity, 0.2f);
                item.OnSave(0.2f);
                item.ChangeState(DirtyItems.State.Cleaning);
                vfxSplashWater.transform.position = item.TF.position;
                vfxSplashWater.Play();
                return true;
            }

            if(item is BeanBasket && item.IsState(BeanBasket.State.WithBeans) && isWater && !haveItem)
            {
                this.item = item as BeanBasket;
                item.OnMove(TF.position + Vector3.left * 0.1f, Quaternion.identity, 0.2f);
                item.OnSave(0.2f);
                item.ChangeState(BeanBasket.State.Washing);
                return true;
            }

            return base.OnTake(item);

        }


        public void OnCloseWater()
        {
            CheckWash();
            Debug.Log("Dong nap roi day");
        }

        public void OnOpenWater(bool isFlowing)
        {
            isWaterFlow = isFlowing;

            if (isFlowing) waterFlowAlpha.DoAlpha(1f, 0.2f);
            else waterFlowAlpha.DoAlpha(0f, 0.2f);

            CheckWash();
        }

        private void CheckWash()
        {
            if (isWaterFlow && isWaterClose) isWater = true;
            if (!isWaterClose) isWater = false;

            if (isWater)
            {
                waterAlpha.DoAlpha(1f, 0.2f);
            }
            else if (!isWaterClose)
            {
                sinkCover?.OnBack();
                waterAlpha.DoAlpha(0f, 0.2f);
            }
        }

        public override void OnClickDown()
        {
            base.OnClickDown();

            if (item != null && haveItem)
            {
                item.OnClickDown();
                return;
            }
        }
    }
}