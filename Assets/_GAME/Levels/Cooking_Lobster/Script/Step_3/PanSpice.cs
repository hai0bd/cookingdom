using DG.Tweening;
using MoreMountains.NiceVibrations;
using Satisgame;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Link.Cooking.Lobster
{
    public class PanSpice : ItemMovingBase
    {
        public enum State { Normal, Mix, Pour, Pouring, Fire, Done }
        float time = 0;
        [SerializeField] State state;
        [SerializeField] SpriteRenderer mix;
        [SerializeField] SpriteRenderer wineColor;
        [SerializeField] SortingGroup wine;
        [SerializeField] ParticleSystem smokeVFX, sparkVFX;
        [SerializeField] ElectricStove stove;
        [SerializeField] HintText hintText;

        bool isFire => stove.IsOn;

        int isWine = 0;
        public override bool IsCanMove => IsState(State.Pour);
        List<ItemSpice> mixes = new List<ItemSpice>();
        ItemSpice coriander; 

        [SerializeField] List<ItemMovingBase> orders;

        public override bool OnTake(IItemMoving item)
        {
            if(!isFire) return false;
            
            if (item is ItemSpice spice && orders[0].Equals(item))
            {
                orders.RemoveAt(0);
                mixes.Add(spice);
                item.TF.SetParent(TF);
                item.TF.localPosition = new Vector3(0.078f, -0.123f, 1);
                mixes.Last().OnActive(mixes.Count + isWine);
                CheckMix();

                if (coriander == null && mixes.Last().ItemName == DecoreItem.NameType.Cucumber)
                {
                    coriander = spice;
                    LevelControl.Ins.SetHintTextDone(4, 6);
                }
                LevelControl.Ins.SetStep(LevelName.Lobster, Step.SpiceInPan);

                if(spice.ItemName == DecoreItem.NameType.Piece_1) LevelControl.Ins.SetHintTextDone(4, 2);
                if(spice.ItemName == DecoreItem.NameType.Piece_2) LevelControl.Ins.SetHintTextDone(4, 3);
                if(spice.ItemName == DecoreItem.NameType.Tomato) LevelControl.Ins.SetHintTextDone(4, 4);
                LevelControl.Ins.SetHintTextDone(3, 5);

                return true;
            }
            else if(item is Wine && orders[0].Equals(item))
            {
                orders.RemoveAt(0);
                item.ChangeState(Wine.State.Pouring);
                item.OnMove(TF.position + Vector3.right * 0.1f, Quaternion.identity, 0.2f);
                wine.transform.DOScale(1, 0.5f).SetDelay(0.4f);
                wine.sortingOrder = mixes.Count + 1;
                isWine = 1;
                CheckMix();
                LevelControl.Ins.SetStep(LevelName.Lobster, Step.WineInPan);
                LevelControl.Ins.SetHintTextDone(4, 5);

                return true;
            }

            return base.OnTake(item);
        }


        public override void ChangeState<T>(T t)
        {
            state = (State)(object)t;
            switch (state)
            {
                case State.Normal:
                    break;
                case State.Mix:
                    LevelControl.Ins.SetStep(LevelName.Lobster, Step.MixInStart);
                    break;
                case State.Pour:
                    sparkVFX.Play();
                    SoundControl.Ins.PlayFX(LevelStep_1.Fx.Blink);
                    LevelControl.Ins.SetHintTextDone(4, 7);
                    break;
                case State.Pouring:
                    DOVirtual.DelayedCall(1.5f, () => ChangeState(State.Fire));
                    time = 0;
                    mix.DOFade(0, 0.5f).SetDelay(0.5f).OnUpdate(
                        ()=> 
                        {
                            time += Time.deltaTime * 2;
                            coriander.SetAlpha(time);
                        });
                    mix.transform.parent.DOScale(0.5f, 1.5f).SetDelay(0.5f);
                    wine.gameObject.SetActive(false);
                    LevelControl.Ins.NextHint();
                    LevelControl.Ins.SetHintTextDone(4, 8);
                    break;
                case State.Fire:
                    OnBack();
                    if (!isFire)
                    {
                        smokeVFX.Clear();
                        smokeVFX.Stop();
                    }
                    LevelControl.Ins.SetStep(LevelName.Lobster, Step.MixInBowl);

                    break;
                case State.Done:
                    LevelControl.Ins.SetStep(LevelName.Lobster, Step.TurnOffStove_2);
                    break;
                default:
                    break;
            }
        }

        public override bool IsState<T>(T t)
        {
            return state == (State)(object)t;
        }

        public void CheckMix()
        {
            if (mixes.Count >= 4 && isWine > 0)
            {
                ChangeState(State.Mix);
            }
        }

        public void SetSpice()
        {
            if (IsState(State.Mix) && isFire)
            {
                time += Time.deltaTime;
                MMVibrationManager.Haptic(HapticTypes.SoftImpact); // Vibrate
                
                //tron
                for (int i = 0; i < mixes.Count; i++)
                {
                    mixes[i].SetTime(time);
                }

                if (time >= 4f)
                {
                    ChangeState(State.Pour);
                }

                if (time > 2f && !smokeVFX.isPlaying)
                {
                    smokeVFX.Play();
                    if (!SoundControl.Ins.IsPlaying(LevelStep_1.Fx.Fry))
                    {
                        SoundControl.Ins.PlayFX(LevelStep_1.Fx.Fry, true);
                    }
                }

                if (time > 3f)
                {
                    //sang dan len
                    Color color = mix.color;
                    color.a = time - 3;
                    mix.color = color;

                    //mo dan di
                    color.a = Mathf.Lerp(color.a, 0, time - 3 * 0.5f);
                    wineColor.color = color;
                    for (int i = 0; i < mixes.Count; i++)
                    {
                        if (mixes[i].ItemName == DecoreItem.NameType.Cucumber)
                        {
                            mixes[i].SetScale(Utilities.GetMapValue(time - 3, 0, 1, 0.3f, 0.15f));
                        }
                        else
                        {
                            mixes[i].SetAlpha(time - 3);
                        }

                    }
                }
            }
        }

        public void SetFire()
        {
            if (time > 2f && isFire)
            {
                smokeVFX.Play();
                LevelControl.Ins.SetStep(LevelName.Lobster, Step.TurnOnStove_2);
                LevelControl.Ins.SetHintTextDone(4, 10);
            }
            else if (time < 2f || IsState(State.Done, State.Fire))
            {
                smokeVFX.Stop();
                smokeVFX.Clear();
                SoundControl.Ins.StopFX(LevelStep_1.Fx.Fry);
            }

            if(!isFire) hintText.OnActiveHint();
        }

        public override void OnClickDown()
        {
            base.OnClickDown();
            SoundControl.Ins.PlayFX(LevelStep_1.Fx.Click);
            SoundControl.Ins.StopFX(LevelStep_1.Fx.Fry);
        }

        public override void OnClickTake()
        {
            base.OnClickTake();
            SoundControl.Ins.PlayFX(LevelStep_1.Fx.Take);
        }
    }
}