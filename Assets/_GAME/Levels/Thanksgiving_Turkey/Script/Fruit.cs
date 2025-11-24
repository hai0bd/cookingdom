using DG.Tweening;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link.Turkey
{
    public class Fruit : ItemMovingBase
    {
        public ItemName fruitName;
        public enum State { Dirty, Water, Cutting, Piece, Prepare, Mix, Done }
        [SerializeField] GameObject[] fruits;
        [SerializeField] State state = State.Dirty;
        [SerializeField] ParticleSystem sparkVFX;
        private bool IsAboveCuttingBoard => LevelControl.Ins.IsHaveObject<CuttingBoard>(TF.position) != null;
        public override bool IsCanMove => state != State.Prepare && state != State.Cutting;
        public override void ChangeState<T>(T t)
        {
            this.state = (State)(object)t;
            //Debug.Log("Fruit State: " + state);

            fruits[(int)state - 1].SetActive(false);
            fruits[(int)state].SetActive(true);

            switch (state)
            {
                case State.Dirty:
                    break;
                case State.Water:
                    //if (fruitName == ItemName.Leaf) { ChangeState(State.Piece); return; }
                    OnSavePoint();
                    sparkVFX.Play();
                    break;
                case State.Piece:
                    sparkVFX.Play();
                    break;
                case State.Prepare:
                    break;
                case State.Mix:
                    break;
                case State.Cutting:
                    break;
                case State.Done:
                    DOVirtual.DelayedCall(0.12f, () => gameObject.SetActive(false));
                    break;
                default:
                    break;
            }
        }

        public override bool IsState<T>(T t)
        {
            return this.state == (State)(object)t;
        }

        public override void OnClickDown()
        {
            base.OnClickDown();
            sprite.sortingOrder = 70;
            if (IsState(State.Water))
            {
                WaterSink w = LevelControl.Ins.IsHaveObject<WaterSink>(TF.position);
                if (w != null) w.PlayWave(TF.position);
            }
            SoundControl.Ins.PlayFX(LevelStep_1.FX.Hold);
        }

        public override void OnClickTake()
        {
            base.OnClickTake();
            SoundControl.Ins.PlayFX(LevelStep_1.FX.Put);
        }

        public override void OnDrop()
        {
            base.OnDrop();
            OnClickTake();
        }

        public override bool OnTake(IItemMoving item)
        {
            if (item is Knife && item.IsState(Knife.State.Blunt))
            {
                item.OnBack();
                return true;
            }

            if (item is Scraper && fruitName == ItemName.Lemon && IsState(State.Water) && IsAboveCuttingBoard)
            {
                //nao chanh
                item.OnMove(TF.position, Quaternion.identity, 0.1f);
                item.ChangeState(Scraper.State.Cutting);
                ChangeState(State.Cutting);

                DOVirtual.DelayedCall(2f, () =>
                {
                    ChangeState(State.Piece);
                });
                return true;
            }
            else
            if (item is Knife && IsState(State.Water) && item.IsState(Knife.State.Sharp) && IsAboveCuttingBoard && fruitName != ItemName.Lemon)
            {
                //cat hoa qua
                //if (fruitName != ItemName.Leaf)
                //{
                item.OnMove(TF.position, Quaternion.identity, 0.1f);
                item.ChangeState(Knife.State.Cutting);
                ChangeState(State.Cutting);
                SoundControl.Ins.PlayFX(LevelStep_1.FX.Cut, true);

                DOVirtual.DelayedCall(2.5f, () =>
                    {
                        SoundControl.Ins.StopFX(LevelStep_1.FX.Cut);
                        ChangeState(State.Piece);
                        item.ChangeState(Knife.State.Sharp);
                        item.OnBack();
                    });
                return true;
                //}
            }
            //if (item is Knife && IsState(State.Water) && fruitName == ItemName.Leaf)
            //{
            //    ChangeState(State.Piece);
            //    item.OnBack();
            //    return true;
            //}
            if(item is not Fruit) LevelControl.Ins.LoseHalfHeart(TF.position);
            return base.OnTake(item);
        }


    }


}