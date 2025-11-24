using DG.Tweening;
using Link;
using Satisgame;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HuyThanh.Cooking.Burrito
{
    public class Crust : ItemMovingBase
    {
        public enum State
        {
            Normal,
            OnCuttingBoard,
            DoneSpice,
            Pack1,
            Pack2,

            Scrolling,
            ChangeToDoneMix,
            DoneMix,
            OnPan,
            Cooking,
            DoneCook,
            Done
        }
        public enum ShakeState
        {
            NotShake,
            Light,
            Hard,
            SuperHard
        }
        [SerializeField] State state;
        [SerializeField] ShakeState shakeState;
        [SerializeField] List<Spoon> spoonList;
        [SerializeField] List<SpriteRenderer> spiceSprites;

        [SerializeField] CuttingBoardBurrito cuttingBoardBurrito;

        [SerializeField] Animation anim;
        [SerializeField] string takeAnimName;

        [SerializeField] GameObject origin, spice, pack1, pack2, scroll, doneMix;
        [FoldoutGroup("Burrito Cook Side")]
        [SerializeField] GameObject burritoRaw1, burritoRaw2, burritoDone1, burritoDone2;

        [FoldoutGroup("Burrito Cook Side Alpha")]
        [SerializeField] ItemAlpha burritoRaw1Alpha, burritoRaw2Alpha, burritoDone1Alpha, burritoDone2Alpha;

        [SerializeField] Transform startPoint, endPoint, burritoDoneMix;
        [SerializeField] float moveScrollSpeed;

        [SerializeField] ParticleSystem smokeVFX, blinkVFX;

        [SerializeField] bool IsCatHandTake;

        [SerializeField] PanBurrito panBurrito;

        [BoxGroup("Cat Hand TF")]
        [SerializeField] CatHandStealBurrito catHand;

        [SerializeField] EmojiControl emoji;

        [SerializeField] Animation redAnimation;

        private float currentHeatUp = 0f;
        private float currentHeatDown = 0f;
        private float maxHeat = 5f;

        private bool isUpSide = true; ///true - 1 ; false - 2
        private bool isCanFlip = false;
        private bool upSideDone = false;
        private bool downSideDone = false;
        private bool isFirstTimeFlip = true;
        private Tween currentBurritoShake;
        private Tween currentJumpTween;
        public override bool IsCanMove => IsState(State.Normal, State.DoneMix);

        public bool IsCanFlip => isCanFlip;
        [SerializeField] Sprite hintBase, hintOnPan, hintFlip;
        [SerializeField] HintText hint_cuttingBoard, hint_addSpice, hint_scroll, hint_flip;

        public override bool IsState<T>(T t)
        {
            return state == (State)(object)t;
        }

        public override void ChangeState<T>(T t)
        {
            state = (State)(object)t;

            switch (state)
            {
                case State.OnCuttingBoard:
                    hint_cuttingBoard.OnActiveHint();
                    break;
                case State.DoneSpice:
                    LevelControl.Ins.SetHint(hintOnPan);
                    hint_addSpice.OnActiveHint();
                    break;
                case State.Pack1:
                    origin.SetActive(false);
                    spice.SetActive(false);
                    pack1.SetActive(true);
                    anim.Play(takeAnimName);
                    break;
                case State.Pack2:
                    pack1.SetActive(false);
                    pack2.SetActive(true);
                    anim.Play(takeAnimName);
                    break;
                case State.Scrolling:
                    cuttingBoardBurrito.TurnCollider(false);
                    scroll.SetActive(true);
                    break;
                case State.ChangeToDoneMix:
                    hint_scroll.OnActiveHint();
                    StartCoroutine(WaitForChangeToDoneMix());
                    break;

                case State.OnPan:
                    LevelControl.Ins.SetHint(hintFlip);
                    SoundControl.Ins.PlayFX(Fx.SpoonFry, true);
                    isCanFlip = true;
                    anim.Play(takeAnimName);
                    doneMix.SetActive(false);
                    SetUpSide(isUpSide);

                    break;

                case State.Done:
                    SoundControl.Ins.StopFX(Fx.SpoonFry);
                    TF.DOKill();
                    burritoDoneMix.DOKill();
                    TF.SetParent(LevelControl.Ins.LevelStep.transform);

                    if (IsCatHandTake)
                    {
                        LevelControl.Ins.SetHint(hintBase);
                        CatHandTake();
                    }
                    else
                    {
                        OrderLayer = 51;
                        TF.DOJump(savePoint, 3, 1, 1f);
                        emoji.ShowPositive(1f);
                        panBurrito.ChangeState(PanBurrito.State.Cooked);
                        LevelControl.Ins.CheckStep(2f);
                    }
                    break;
            }
        }

        public bool CanTakeSpoon(Spoon spoon)
        {
            return spoonList.Contains(spoon) && spoonList[0] == spoon;
        }

        public void RemoveSpoon()
        {
            spoonList.RemoveAt(0);
            if (spoonList.Count == 0)
            {
                ChangeState(State.DoneSpice);
            }
        }

        public void SpiceActive()
        {
            spiceSprites[0].DOColor(Color.white, 0.5f);
            spiceSprites.RemoveAt(0);
        }

        Vector3 mousePosition;

        private void OnMouseDown()
        {
            mousePosition = Input.mousePosition;
        }

        private void OnMouseDrag()
        {
            if (IsState(State.Scrolling))
            {
                Vector3 mouseCurrentPosition = Input.mousePosition;
                float delta_Y = mouseCurrentPosition.y - mousePosition.y;
                delta_Y = Mathf.Max(0f, delta_Y);
                scroll.transform.position = Vector3.MoveTowards(scroll.transform.position, endPoint.position, delta_Y * moveScrollSpeed * Time.deltaTime);

                mousePosition = mouseCurrentPosition;
                if (Vector3.Distance(scroll.transform.position, endPoint.position) < 0.1f)
                {
                    cuttingBoardBurrito.TurnCollider(true);
                    ChangeState(State.ChangeToDoneMix);
                }
            }
        }

        IEnumerator WaitForChangeToDoneMix()
        {
            pack2.SetActive(false);
            scroll.transform.DOMove(startPoint.position, 0.2f);

            yield return WaitForSecondCache.Get(0.2f);

            anim.Play(takeAnimName);
            scroll.SetActive(false);
            doneMix.SetActive(true);
            ChangeState(State.DoneMix);
        }

        [Button]
        public void DoFlip()
        {
            isCanFlip = false;

            if (isFirstTimeFlip && IsCatHandTake)
            {
                hint_flip.OnActiveHint();
                isFirstTimeFlip = false;
            }

            /// code nay la xu ly tay meo goc
            if (downSideDone && upSideDone && IsState(State.OnPan) && IsCatHandTake)
            {

                ChangeState(State.Done);
                return;
            }
            burritoDoneMix.DORotate(burritoDoneMix.rotation.eulerAngles + Vector3.right * 180f, 0.5f);
            burritoDoneMix.DOLocalJump(Vector3.zero, 1f, 1, 0.5f);

            StartCoroutine(WaitForDoneFlip());
            SetUpSide(!isUpSide);
        }

        IEnumerator WaitForDoneFlip()
        {
            yield return WaitForSecondCache.Get(0.5f);
            blinkVFX.Play();
            SoundControl.Ins.PlayFX(Fx.DoneSomething);
            burritoDoneMix.DOKill();
            burritoDoneMix.DOLocalMove(Vector3.zero, 0.1f);

            while (panBurrito.IsDrop == false)
            {
                yield return null;
            }
            isCanFlip = true;
        }

        public void AddHeat(float heatAdd)
        {
            if (panBurrito.IsDrop == false)
            {
                return;
            }
            SoundControl.Ins.PlayFX(Fx.BurritoBoil);
            ///Add heat to current side
            if (!isUpSide)
            {
                currentHeatUp += heatAdd;
                currentHeatUp = Mathf.Clamp(currentHeatUp, 0, maxHeat);
                burritoRaw1Alpha.SetAlpha(Mathf.Clamp(1 - currentHeatUp / maxHeat, 0, 1f));
                burritoDone1Alpha.SetAlpha(Mathf.Clamp(currentHeatUp / maxHeat, 0, 1f));
                DoShakeFromHeat(currentHeatUp);

                if (currentHeatUp >= maxHeat && !upSideDone)
                {
                    upSideDone = true;
                    redAnimation.Play("RedBlink");
                    if (!IsCatHandTake)
                    {
                        blinkVFX.Play();
                        ChangeState(State.Done);
                    }
                    else
                    {
                        panBurrito.SetControl(true);
                        panBurrito.DoShake();
                    }
                }
            }
            else
            {
                currentHeatDown += heatAdd;
                currentHeatDown = Mathf.Clamp(currentHeatDown, 0, maxHeat);
                burritoRaw2Alpha.SetAlpha(Mathf.Clamp(1 - currentHeatDown / maxHeat, 0, 1f));
                burritoDone2Alpha.SetAlpha(Mathf.Clamp(currentHeatDown / maxHeat, 0, 1f));
                DoShakeFromHeat(currentHeatDown);

                if (currentHeatDown >= maxHeat && !downSideDone)
                {
                    downSideDone = true;
                    panBurrito.SetControl(true);
                    panBurrito.DoShake();
                    redAnimation.Play("RedBlink");
                }
            }
        }

        private void DoShakeFromHeat(float heat)
        {
            if (1f <= heat && heat < 2.5f && (shakeState != ShakeState.Light || currentBurritoShake == null))
            {
                shakeState = ShakeState.Light;
                if (currentBurritoShake != null)
                {
                    currentBurritoShake.Kill();
                    currentBurritoShake = null;
                    burritoDoneMix.localPosition = Vector3.zero;
                }

                currentBurritoShake = TF.DOShakePosition(5, 0.005f, 10, 10, fadeOut: false).SetLoops(-1);
            }
            else if (2.5f <= heat && heat < 4f && (shakeState != ShakeState.Hard || currentBurritoShake == null))
            {
                if (smokeVFX.isPlaying == false)
                    smokeVFX.Play();
                shakeState = ShakeState.Hard;
                if (currentBurritoShake != null)
                {
                    currentBurritoShake.Kill();
                    currentBurritoShake = null;
                    burritoDoneMix.localPosition = Vector3.zero;
                }

                currentBurritoShake = TF.DOShakePosition(5, 0.01f, 10, 10, fadeOut: false).SetLoops(-1);
            }
            else if (4f <= heat && heat < 5f && (shakeState != ShakeState.SuperHard || currentBurritoShake == null))
            {
                shakeState = ShakeState.SuperHard;
                if (currentBurritoShake != null)
                {
                    currentBurritoShake.Kill();
                    currentBurritoShake = null;
                    burritoDoneMix.localPosition = Vector3.zero;
                }

                currentBurritoShake = TF.DOShakePosition(5, 0.02f, 10, 10, fadeOut: false).SetLoops(-1);
            }
        }

        public void SetUpSide(bool isUp)
        {
            isUpSide = isUp;
            burritoRaw1.SetActive(isUpSide);
            burritoRaw2.SetActive(!isUpSide);
            burritoDone1.SetActive(isUpSide);
            burritoDone2.SetActive(!isUpSide);
        }

        [Button]
        public void CatHandTake()
        {
            OrderLayer = 51;
            TF.DOJump(new Vector3(TF.position.x, 5f, 0), 1, 1, 1.5f);
            StartCoroutine(CatCoroutine());
        }

        IEnumerator CatCoroutine()
        {
            yield return WaitForSecondCache.Get(.3f);
            catHand.StealBurrito();
        }

        public override void OnClickDown()
        {
            base.OnClickDown();
            SoundControl.Ins.PlayFX(Fx.Click);
        }

        public override void OnDrop()
        {
            base.OnDrop();
            SoundControl.Ins.PlayFX(Fx.PutDown);
        }

        public override void OnClickTake()
        {
            base.OnClickTake();
            SoundControl.Ins.PlayFX(Fx.PutDown);
        }
    }
}