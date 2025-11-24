using DG.Tweening;
using Satisgame;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VinhLB;

namespace Link.Turkey
{
    public class Chicken : ItemMovingBase
    {
        public enum State 
        { 
            Plastic,//boc nilong
            NeedWash,//can rua
            Water,//da rua
            NeedClean,//da rua sach can lau don
            Clean,//da lau xong
            Dish,//cho len dia
            Prepare,//cho len thot
            Marinate_1,//cho len thot
            Marinate_2,//uop gia vi ben trong
            Marinated,//nuong
            Grill,//di chuyen de nuong
            Grilling,//dang nuong
            Ripe,//k di chuyen
            Decore,//chin mang ra ngoai
            Done,
        }

        //public override bool IsCanMove => state == State.NeedWash || state == State.Water || state == State.Clean || state == State.Grill || state == State.Prepare || state == State.Decore;
        public override bool IsCanMove => IsState(State.NeedWash, State.Water, State.Clean, State.Grill, State.Prepare, State.Decore);

        [SerializeField] ModifiedMeshFoldDraggable[] meshFold;
        [ShowInInspector] State state = State.Plastic;
        [SerializeField] GameObject[] chickens;
        [SerializeField] Transform[] waters;
        [SerializeField] ParticleSystem sparkVFX;
        [SerializeField] EmojiControl emoji;
        [SerializeField] GameObject waterVFX;

        [SerializeField] Sprite hint_12;

        [field:SerializeField] public Collider2D Collider { get; private set; }
        private float cleanRate = 0;


        int plastic = 2;

        private void Start()
        {
            meshFold[0].onDetached += UnPack;
            meshFold[1].onDetached += UnPack;
        }


        private void OnDestroy()
        {
            meshFold[0].onDetached -= UnPack;
            meshFold[1].onDetached -= UnPack;
        }

        public override void OnClickDown()
        {
            base.OnClickDown();
            if (IsState(State.Water))
            {
                waterVFX.SetActive(true);
                WaterSink w = LevelControl.Ins.IsHaveObject<WaterSink>(TF.position);
                if (w != null) w.PlayWave(TF.position);
            }
            SoundControl.Ins.PlayFX(LevelStep_1.FX.Hold);
        }

        public override void OnClickTake()
        {
            base.OnClickTake();
            OnSavePoint();
            SoundControl.Ins.PlayFX(LevelStep_1.FX.Put);
        }

        public override void OnDrop()
        {
            base.OnDrop();
            base.OnClickTake();
        }

        public override void ChangeState<T>(T t) 
        {
            this.state = (State)(object)(t);
            chickens[(int)state - 1].SetActive(false);
            chickens[(int)state].SetActive(true);

            switch (state)
            {
                case State.Plastic:
                    break;
                case State.NeedWash:
                    break;
                case State.Water:
                    sparkVFX.Play();
                    break;
                case State.NeedClean:
                    waterVFX.SetActive(false);
                    for (int i = 0; i < waters.Length; i++)
                    {
                        waters[i].localScale = Vector3.zero;
                        waters[i].DOScale(1, 0.3f);
                    }
                    break;
                case State.Clean:
                    sparkVFX.Play();
                    break;
                case State.Dish:
                    LevelControl.Ins.SetHint(hint_12);
                    LevelControl.Ins.SetHintTextDone(1, 3);
                    break;
                case State.Marinated:
                    emoji.ShowPositive();
                    LevelControl.Ins.SetHintTextDone(2, 8);
                    break;
                case State.Ripe:
                    OrderLayer = 15;
                    LevelControl.Ins.SetHintTextDone(3, 1);
                    break;
                case State.Marinate_1:
                    LevelControl.Ins.SetHintTextDone(2, 1);
                    break;
                case State.Marinate_2:
                    break;
                case State.Done:
                    OrderLayer = 100;
                    emoji.ShowPositive();
                    break;
                case State.Grill:
                    break;
                case State.Prepare:
                    break;
                case State.Grilling:
                    break;
                case State.Decore:
                    OrderLayer = 25;
                    break;
            }
            //Debug.Log("Chicken : " + state);
        }

        public override bool IsState<T>(T t)
        {
            return this.state == (State)(object)(t);
        }

        public override bool OnTake(IItemMoving item)
        {
            if (item is not Tissue && item is not Decor)
            {
                if (!(item is Fruit && (item.IsState(Fruit.State.Dirty) || item.IsState(Fruit.State.Water))))
                {
                    item.OnBack();
                }
            }
            //lay sot
            //lay ruou
            if( !(item is Tissue || item is Spoon || item is Decor || item is WaterCover)) LevelControl.Ins.LoseHalfHeart(TF.position);
            return false;
        }

        private void UnPack(ModifiedMeshFoldDraggable draggable)
        {
            plastic--;
            if (plastic <= 0)
            {
                ChangeState(State.NeedWash);
                LevelControl.Ins.SetHintTextDone(1, 2);
            }
        }

        public void AddClean()
        {
            cleanRate += Time.deltaTime;
            if (cleanRate >= 1)
            {
                ChangeState(State.Clean);
            }
            for (int i = 0; i < waters.Length; i++)
            {
                waters[i].localScale = Vector3.one * (1 - cleanRate);
            }
        }

    }
}