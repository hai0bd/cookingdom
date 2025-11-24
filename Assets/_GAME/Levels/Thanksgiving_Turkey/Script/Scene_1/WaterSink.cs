using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link.Turkey
{
    public class WaterSink : ItemIdleBase
    {
        [SerializeField] SpriteRenderer water;
        [SerializeField] GameObject waverFall;
        [SerializeField] ParticleSystem waveVFX;

        [SerializeField] Transform[] fruitPoints;

        WaterCover waterCover;
        bool waterTapOn = false;
        bool waterCoverClose => waterCover != null && Vector2.Distance(waterCover.TF.position, transform.position) < 0.35f;
        bool isWater = false;
        public override bool IsDone => !isWater && !waterTapOn;

        public void OnOpenWater(bool isOn)
        {
            waterTapOn = isOn;
            CheckWash();
            LevelControl.Ins.CheckStep();
            waverFall.SetActive(isOn);

            if (isOn)
            {
                SoundControl.Ins.PlayFX(LevelStep_1.FX.Water, true);
            }
            else
            {
                SoundControl.Ins.StopFX(LevelStep_1.FX.Water);
            }
        }

        public void OnCloseCover()
        {
            CheckWash();
            LevelControl.Ins.CheckStep();
        }

        public override bool OnTake(IItemMoving item)
        {
           
            if (item is Chicken)
            {
                //neu la item co the wash
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                DOVirtual.DelayedCall(0.2f, () => item.OrderLayer = 10);
                CheckWash();
                PlayWave(item.TF.position);
                return true;
            }
            
            if (item is Fruit)
            {
                //neu la item co the wash
                Vector2 point = GetFruitPoint(item);
                item.OnMove(point, Quaternion.identity, 0.2f);
                CheckWash();
                PlayWave(item.TF.position);
                return true;
            }
            else if (item is WaterCover)
            {
                waterCover = item as WaterCover;
                //neu la nap ong nuoc
                if (Vector2.Distance(item.TF.position, transform.position) < 0.35f)
                {
                    //neu o du gan thi dong lai
                    item.OnMove(TF.position, Quaternion.identity, 0.2f);
                    Invoke(nameof(OnCloseCover), 0.2f);
                    item.OrderLayer = 0;
                }
                else
                {
                    //neu xa qua tinh la mo
                    OnCloseCover();
                }
                return true;
            }
            return false;
        }

        private void CheckWash()
        {
            if (waterTapOn && waterCoverClose) isWater = true;
            if (!waterCoverClose) isWater = false;
            if (isWater)
            {
                water.DOFade(0.3f, 0.4f);
                Collider2D[] colliders = Physics2D.OverlapBoxAll(TF.position, new Vector2(2.2f, 1.8f), 0);
                IItemMoving it;

                foreach (var item in colliders)
                {
                    it = item.GetComponent<IItemMoving>();
                    if (it == null) continue;
                    if (it is Chicken && it.IsState(Chicken.State.NeedWash))
                    {
                        it.ChangeState(Chicken.State.Water);
                        it.OnSavePoint();
                    }
                    if (it is Fruit && it.IsState(Fruit.State.Dirty))
                    {
                        it.ChangeState(Fruit.State.Water);
                        it.OnSavePoint();
                    }
                }
            }
            else if(!waterCoverClose)
            {
                water.DOFade(0, 0.4f);
            }
        }

        public void PlayWave(Vector3 point)
        {
            if (isWater)
            {
                SoundControl.Ins.PlayFX(LevelStep_1.FX.Bubble);
                waveVFX.transform.position = point;
                waveVFX.Play();
            }
        }

        private Vector3 GetFruitPoint(IItemMoving fruit)
        {
            float distance = Mathf.Infinity;
            Vector3 target = Vector3.zero;
            for (int i = 0; i < fruitPoints.Length; i++)
            {
                if (!LevelControl.Ins.IsHaveObjectOther<Fruit>(fruit, fruitPoints[i].position, 0.01f) && distance > Vector2.Distance(fruit.TF.position, fruitPoints[i].position))
                {
                    distance = Vector2.Distance(fruit.TF.position, fruitPoints[i].position);
                    target = fruitPoints[i].position;
                }
            }
            return target;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(TF.position, new Vector3(2.2f, 1.8f, 1));
        }
    }
}