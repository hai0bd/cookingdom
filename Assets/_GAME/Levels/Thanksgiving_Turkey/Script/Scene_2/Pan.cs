using DG.Tweening;
using Satisgame;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Link.Turkey
{
    public class Pan : ItemMovingBase
    {
        public enum State { Empty, HeatPan, ReadyMix, Cooking, CookingDone, Clear, TurnOn, TurnOff}
        [SerializeField] private State state = State.Empty;

        public override bool IsCanMove => state == State.CookingDone;

        [SerializeField] GameObject activeGas;
        [SerializeField] Transform item;
        [SerializeField] SortingGroup[] sortingGroups;
        [SerializeField] int orderLayer;
        private int indexOrder = 0;
        float cookTime = 0;
        [SerializeField] SpriteRenderer oilRender;
        [SerializeField] ParticleSystem smokeVFX, sparkVfx;
        [SerializeField] EmojiControl emoji;
        [SerializeField] SpriteRenderer core;
        [SerializeField] List<SpriteRenderer> sprites;
        [SerializeField] Sprite spicesHint, spoonHint;

        public override bool IsDone => IsState(State.TurnOff);

        public bool IsActiveGas => activeGas.activeSelf;

        public void ActiveGas()
        {
            if (IsState(State.TurnOff))
            {
                return;
            }

            LevelControl.Ins.SetHintTextDone(2, 2);

            activeGas.SetActive(!IsActiveGas);
            if (IsAdd(ItemName.Oil) && IsActiveGas && IsState(State.Empty))
            {
                ChangeState(State.HeatPan);
            }
            if (indexOrder >= sortingGroups.Length - 1 && IsActiveGas && IsState(State.ReadyMix))
            {
                ChangeState(State.Cooking);
                
            }

            if (IsState(State.TurnOn) && !IsActiveGas)
            {
                ChangeState(State.TurnOff);
                LevelControl.Ins.SetHintTextDone(2, 9);
            }

            if (IsActiveGas)
            {
                SoundControl.Ins.PlayFX(LevelStep_1.FX.Fry, true);
                LevelControl.Ins.SetHintTextDone(2, 1);
            }
            else
            {
                SoundControl.Ins.StopFX(LevelStep_1.FX.Fry);
            }

            SoundControl.Ins.PlayFX(LevelStep_1.FX.Toggle);
            LevelControl.Ins.CheckStep();
        }

        public override bool OnTake(IItemMoving item)
        {
            if (IsState(State.Empty))
            {
                if (item is Spices && (item as Spices).spicesName == ItemName.Oil)
                {
                    AddItem((item as Spices).spicesName);
                    item.OnMove(TF.position, Quaternion.identity, 0.15f);
                    item.ChangeState(Spices.State.Used);
                    if (IsActiveGas)
                    {
                        ChangeState(State.HeatPan);
                    }
                    return true;
                }
                else if (item is Fruit)
                {
                    Fruit f = item as Fruit;
                    f.OnMove((f.TF.position - TF.position).normalized * 1.35f + TF.position, Quaternion.identity, 0.25f);
                } 
                emoji.ShowNegative();
                LevelControl.Ins.LoseHalfHeart(TF.position);
                return false;
            }

            if (item is Fruit)
            {
                AddItem((item as Fruit).fruitName);
                item.OnMove(TF.position, Quaternion.identity, 0.15f);
                item.ChangeState(Fruit.State.Done);
                return true;
            }
            if (item is Spices)
            {
                AddItem((item as Spices).spicesName);
                item.OnMove(TF.position, Quaternion.identity, 0.15f);
                item.ChangeState(Spices.State.Used);
                return true;
            }
            
            return false;
        }

        private void AddItem(ItemName itemName)
        {
            indexOrder++;
            sortingGroups[(int)itemName].sortingOrder = indexOrder;
            sortingGroups[(int)itemName].gameObject.SetActive(true);

            if (indexOrder >= sortingGroups.Length - 1 && IsActiveGas)
            {
                ChangeState(State.Cooking);
            }
        }

        private bool IsAdd(ItemName itemName)
        {
            return sortingGroups[(int)itemName].gameObject.activeSelf;
        }

        public void Cooking()
        {
            if (IsState(State.Cooking) && cookTime < 3f && IsActiveGas)
            {
                cookTime += Time.deltaTime;
                item.transform.Rotate(Vector3.back * 30 * Time.deltaTime, Space.Self);
                if (cookTime >= 2f)
                {
                    SetColor(cookTime - 2f);
                }
                if (cookTime >= 3f)
                {
                    ChangeState(State.CookingDone);
                }
            }
        }
        public override void ChangeState<T>(T t)
        {
            this.state = (State)(object)(t);
            switch (state)
            {
                case State.Empty:
                    break;
                case State.HeatPan:
                    oilRender.DOFade(0.5f, 0.8f);
                    DOVirtual.DelayedCall(1f, () =>
                    {
                        ChangeState(State.ReadyMix);
                        //them vfx
                        smokeVFX.Play();
                    });
                    LevelControl.Ins.SetHint(spicesHint);
                    break;
                case State.ReadyMix:
                    sparkVfx.Play();
                    LevelControl.Ins.SetHintTextDone(2, 3);
                    break;
                case State.Cooking:
                    emoji.ShowPositive();
                    LevelControl.Ins.SetHint(spoonHint);
                    LevelControl.Ins.SetHintTextDone(2, 4);
                    LevelControl.Ins.SetHintTextDone(2, 5);
                    break;
                case State.CookingDone:
                    LevelControl.Ins.SetHintTextDone(2, 6);
                    sparkVfx.Play();
                    break;
                case State.Clear:
                    core.DOFade(0, 1.5f).OnComplete(OnBack);
                    LevelControl.Ins.SetHintTextDone(2, 7);
                    break;
                case State.TurnOn:
                    break;
                case State.TurnOff:
                    TF.DOKill();
                    smokeVFX.gameObject.SetActive(false);
                    emoji.ShowPositive();
                    break;
                default:
                    break;
            }
            //Debug.Log("PanSpice : " + state);
        }

        public override void OnBack()
        {
            base.OnBack();
            DOVirtual.DelayedCall(0.3f, () =>
            {
                if (IsState(State.Clear))
                {
                    if (IsActiveGas)
                    {
                        ChangeState(State.TurnOn);
                        TF.DOShakePosition(100, 0.02f, 15).SetEase(Ease.Linear).SetDelay(0.5f);
                    }
                    else
                    {
                        ChangeState(State.TurnOff);
                    }
                }
            });
        }

        public override bool IsState<T>(T t)
        {
            return this.state == (State)(object)(t);
        }

        private void SetColor(float rate)
        {
            core.color = Color.white * rate;
            for (int i = 0; i < sprites.Count; i++)
            {
                sprites[i].color = Color.white * (1 - rate);
            }
        }

        public override void OnClickDown()
        {
            base.OnClickDown();
            OrderLayer = 100;
            SoundControl.Ins.PlayFX(LevelStep_1.FX.Hold);
        }

        public override void OnClickTake()
        {
            base.OnClickTake();
            OrderLayer = orderLayer;
            SoundControl.Ins.PlayFX(LevelStep_1.FX.Put);
        }

        public override void OnDrop()
        {
            base.OnDrop();
            OnClickTake();
        }
    }
}
