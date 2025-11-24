using DG.Tweening;
using Link;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

namespace HuyThanh.Cooking.SarmaleRomania
{


    public class PotCorn : ItemMovingBase
    {
        public enum State
        {
            Water,
            Salt,
            WaitForTurnOn,
            Boiling,
            CornStarch,
            WaitForSpatula,
            Mixing,
            WaitForTurnOff,
            DoneMixing,
            Pouring,
            Done
        }
        [SerializeField] State state;
        [SerializeField] Transform napNoiTF, spatulaTF;

        [SerializeField] ItemAlpha waterAlpha, waterBoilAlpha, saltAlpha, cornStarchAlpha;

        [SerializeField] Transform squareWater;

        [SerializeField] Drop2D saltDrop2D;

        [SerializeField] ParticleSystem smokeVFX;

        [FoldoutGroup("Water Boil Anim")]
        [SerializeField] Animation waterBoilAnim;

        [FoldoutGroup("Base Alpha")][SerializeField] ItemAlpha base1, base2, base3, base4, base5;

        [SerializeField] BaseCornBowl baseCornBowl;
        [SerializeField] Transform waterBowlTF;

        [SerializeField] Animation anim;
        [SerializeField] string potCornPouringAnim;

        private SpatulaCorn spatulaCorn;
        private Ellipse2D ellipse2D;
        private Vector3 mousePos; /// use for mixing

        private float timer = 0;

        private bool isClick = false;
        public override bool IsCanMove => IsState(State.DoneMixing);

        public override bool IsState<T>(T t)
        {
            return state == (State)(object)t;
        }

        public override void ChangeState<T>(T t)
        {
            state = (State)(object)t;

            switch (state)
            {
                case State.Boiling:
                    StartCoroutine(WaitForBoiling());
                    break;
                case State.Mixing:
                    ellipse2D = new Ellipse2D(0.15f, 0.1f, spatulaTF.localPosition);
                    spatulaCorn.TF.SetParent(TF);
                    break;
                case State.WaitForTurnOff:
                    spatulaCorn.TF.SetParent(LevelControl.Ins.LevelStep.transform);
                    spatulaCorn.ChangeState(SpatulaCorn.State.Done);
                    waterBowlTF.DOMove(waterBowlTF.position + Vector3.right * 4f, .5f);
                    baseCornBowl.MoveIn();
                    break;
                case State.Pouring:
                    anim.Play(potCornPouringAnim);
                    StartCoroutine(WaitForDonePouring());
                    break;
                case State.Done:
                    smokeVFX.Stop();
                    OnBack();
                    break;
            }
        }


        public override bool OnTake(IItemMoving item)
        {

            if (item is ItemPouringCorn waterBowl && IsState(State.Water) && waterBowl.IsPouringType(ItemPouringCorn.PouringType.Water))
            {
                StartCoroutine(WaitForPouringWater());
                item.OnMove(napNoiTF.position, Quaternion.identity, 0.2f);
                item.ChangeState(ItemPouringCorn.State.Pouring);
                return true;
            }

            if (item is ItemPouringCorn salt && IsState(State.Salt) && salt.IsPouringType(ItemPouringCorn.PouringType.Salt))
            {
                StartCoroutine(WaitForPouringSalt());
                item.OnMove(napNoiTF.position, Quaternion.identity, 0.2f);
                item.ChangeState(ItemPouringCorn.State.Pouring);
                return true;
            }

            if (item is ItemPouringCorn cornStarch && IsState(State.CornStarch) && cornStarch.IsPouringType(ItemPouringCorn.PouringType.CornStarch))
            {
                StartCoroutine(WaitForCornStarch());
                item.OnMove(napNoiTF.position, Quaternion.identity, 0.2f);
                item.ChangeState(ItemPouringCorn.State.Pouring);
                return true;
            }

            if (item is SpatulaCorn spatula && IsState(State.WaitForSpatula) && spatula.IsState(SpatulaCorn.State.Normal))
            {
                StartCoroutine(WaitForMixingAnim());
                spatulaCorn = spatula;
                spatulaCorn.SetCollider(false);
                item.OnMove(spatulaTF.position, spatula.TF.rotation, 0.2f);
                item.ChangeState(SpatulaCorn.State.Mixing);
                return true;
            }
            return base.OnTake(item);
        }
        #region Coroutines For State    
        IEnumerator WaitForPouringWater()
        {
            yield return WaitForSecondCache.Get(0.5f);
            squareWater.DOScale(1, 0.5f);
            waterAlpha.DoAlpha(1, 0.5f);
            yield return WaitForSecondCache.Get(0.5f);
            ChangeState(State.Salt);
        }
        IEnumerator WaitForPouringSalt()
        {
            yield return WaitForSecondCache.Get(1f);
            saltDrop2D.OnActive();
            ChangeState(State.WaitForTurnOn);
        }

        IEnumerator WaitForBoiling()
        {
            waterAlpha.DoAlpha(0, 2f);
            waterBoilAlpha.DoAlpha(1, 2f);
            saltAlpha.DoAlpha(0, 2f);
            yield return WaitForSecondCache.Get(1f);
            smokeVFX.Play();
            waterBoilAnim.Play();
            yield return WaitForSecondCache.Get(1f);
            ChangeState(State.CornStarch);
        }

        IEnumerator WaitForCornStarch()
        {
            yield return WaitForSecondCache.Get(0.5f);
            cornStarchAlpha.DoAlpha(1, 0.5f);
            yield return WaitForSecondCache.Get(0.5f);
            ChangeState(State.WaitForSpatula);
        }

        IEnumerator WaitForMixingAnim()
        {
            yield return WaitForSecondCache.Get(0.8f);
            ChangeState(State.Mixing);
        }

        IEnumerator WaitForDonePouring()
        {
            yield return WaitForSecondCache.Get(1.2f);
            ChangeState(State.Done);
        }
        #endregion

        #region ButtonClick
        public bool CanUseButton()
        {
            return IsState(State.WaitForTurnOn) || IsState(State.WaitForTurnOff);
        }

        public void OnButtonClick()
        {
            if (IsState(State.WaitForTurnOn))
            {
                ChangeState(State.Boiling);
            }

            if (IsState(State.WaitForTurnOff))
            {
                ChangeState(State.DoneMixing);
            }
        }
        #endregion

        private void OnMouseDown()
        {
            if (IsState(State.Mixing))
            {
                isClick = true;
            }
        }

        private void LateUpdate()
        {
            if (isClick && Input.GetMouseButtonUp(0))
            {
                isClick = false;
            }

            if (isClick && IsState(State.Mixing))
            {
                Vector3 newMousePos = LevelControl.Ins.GetPoint();
                Vector3 currentDir = newMousePos - TF.position;
                Vector3 previousDir = mousePos - TF.position;

                float angle = Vector3.SignedAngle(previousDir, currentDir, Vector3.forward);
                angle = Mathf.Clamp(angle, -20, 0);
                timer += Time.deltaTime * (Mathf.Abs(angle) / 20f);
                spatulaCorn.TF.localPosition = ellipse2D.Evaluate(timer);

                mousePos = newMousePos;


                if (timer > 0 && timer <= 2)
                {
                    cornStarchAlpha.SetAlpha(1 - timer / 2);
                    waterBoilAlpha.SetAlpha(1 - timer / 2);
                    base1.SetAlpha(timer / 2);
                }
                else if (timer > 2 && timer <= 4)
                {
                    base1.SetAlpha(1 - (timer - 2) / 2);
                    base2.SetAlpha((timer - 2) / 2);
                }
                else if (timer > 4 && timer <= 6)
                {
                    base2.SetAlpha(1 - (timer - 4) / 2);
                    base3.SetAlpha((timer - 4) / 2);
                }
                else if (timer > 6 && timer <= 8)
                {
                    base3.SetAlpha(1 - (timer - 6) / 2);
                    base4.SetAlpha((timer - 6) / 2);
                }
                else if (timer > 8 && timer <= 10)
                {
                    base4.SetAlpha(1 - (timer - 8) / 2);
                    base5.SetAlpha((timer - 8) / 2);
                }
                else if (timer > 10)
                {
                    ChangeState(State.WaitForTurnOff);
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (ellipse2D != null)
                ellipse2D.OnDrawGizmos(TF.position);
        }
    }
}