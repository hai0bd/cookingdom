using DG.Tweening;
using Link;
using Satisgame;
using System.Collections.Generic;
using UnityEngine;

namespace HuyThanh.Cooking.TunaSaladFrench

{
    public class SaladBowl : ItemIdleBase
    {
        [SerializeField] List<ItemMovingBase> order;
        [SerializeField] private List<GameObject> spiceRender;
        [SerializeField] EmojiControl _emoji;

        [SerializeField] Sprite hint;

        public override bool OnTake(IItemMoving item)
        {
            if (order.Count == 0)
            {
                return false;
            }

            if (item is Bean && order[0].Equals(item))
            {
                order.RemoveAt(0);
                SoundControl.Ins.PlayFX(Fx.Take);
                item.ChangeState(Bean.State.Done);
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                DOVirtual.DelayedCall(0.2f, () =>
                {
                    ActiveRender();
                    item.TF.gameObject.SetActive(false);
                });
                return true;
            }

            if (item is Potato && order[0].Equals(item))
            {
                order.RemoveAt(0);
                SoundControl.Ins.PlayFX(Fx.Take);
                item.ChangeState(Potato.State.Done);
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                DOVirtual.DelayedCall(0.2f, () =>
                {
                    ActiveRender();
                    item.TF.gameObject.SetActive(false);
                });
                return true;
            }

            if (item is SugarSpoon && order[0].Equals(item))
            {
                order.RemoveAt(0);
                SoundControl.Ins.PlayFX(Fx.SaltPouring, 0.1f);
                item.ChangeState(SugarSpoon.State.Pouring);
                item.OnMove(TF.position + Vector3.right * 0.4f, item.TF.rotation, 0.2f);

                DOVirtual.DelayedCall(1.2f, () =>
                {
                    ActiveRender();
                    // item.TF.gameObject.SetActive(false);
                });
                return true;
            }

            if (item is Wine && order[0].Equals(item))
            {
                order.RemoveAt(0);
                _emoji.ShowPositive();
                SoundControl.Ins.PlayFX(Fx.OilPour);
                item.ChangeState(Wine.State.Pouring);
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                DOVirtual.DelayedCall(1.2f, () =>
                {
                    ActiveRender();
                    // item.TF.gameObject.SetActive(false);

                });

                DOVirtual.DelayedCall(2f, () =>
                {
                    if (order.Count == 0)
                    {
                        LevelControl.Ins.SetHint(hint); /// set hint khi choi step sau
                        MiniGame1.Instance.CheckDone();
                    }

                });

                return true;
            }

            return base.OnTake(item);
        }

        public bool CheckDone()
        {
            return order.Count == 0;
        }
        private void ActiveRender()
        {
            spiceRender[0].SetActive(true);
            spiceRender.RemoveAt(0);
            LevelControl.Ins.CheckStep(0.5f);
            return;
        }
    }
}