using Link;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
namespace HuyThanh.Cooking.SarmaleRomania
{


    public class CabbageBowlLeaf : ItemIdleBase
    {
        public enum State
        {
            Normal,
            Mixing,
            Done,
        }
        [SerializeField] State state;
        [SerializeField] ItemAlpha squareCabbageCuttingAlpha, squareDillAlpha, squareMix1, squareMix2;
        [SerializeField] Drop2D pepperDrop2D;
        [SerializeField] CircleCollider2D circleCollider2D;

        [SerializeField] Transform spoonTF;
        [SerializeField] ParticleSystem blinkVFX;

        [SerializeField] Animation animSquareDill, animSquareMix1;


        private SpoonCabbageMix spoon;
        private Ellipse2D ellipse2D;
        private Vector3 mousePos; /// use for mixing

        private float ellipseTimer = 0;
        private float timer = 0;

        private bool isWaitingDill = false;
        private bool isWaitingPepper = false;
        private bool isWaitingSpoon = false;
        private bool isClick = false;

        private Fork fork;

        public override bool OnTake(IItemMoving item)
        {
            if (item is Fork && item.IsState(Fork.State.HoldingCabbage))
            {
                fork = item as Fork;
                fork.ChangeState(Fork.State.HoldingCabbageOnBowl);
                fork.OnMove(TF.position + Vector3.up * 1f, fork.TF.rotation, 0.2f);
                circleCollider2D.offset = Vector2.up;

                return true;
            }

            if (item is CabbageLeafCutting cabbageLeafCutting && item.IsState(CabbageLeafCutting.State.DoneCut))
            {
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                item.ChangeState(CabbageLeafCutting.State.InBowl);
                cabbageLeafCutting.PourIntoBowl();
                squareCabbageCuttingAlpha.DoAlpha(1, 0.5f);
                isWaitingDill = true;
                return true;
            }

            if (item is ItemPouring dill && dill.IsPouringType(ItemPouring.PouringType.DillMix) && item.IsState(ItemPouring.State.Normal) && isWaitingDill)
            {
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                item.ChangeState(ItemPouring.State.Pouring);

                StartCoroutine(WaitForPouringDill());
                return true;
            }

            if (item is ItemPouring pepper && pepper.IsPouringType(ItemPouring.PouringType.Pepper) && item.IsState(ItemPouring.State.Normal) && isWaitingPepper)
            {
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                item.ChangeState(ItemPouring.State.Pouring);

                StartCoroutine(WaitForPouringPepper());

                return true;
            }

            if (item is SpoonCabbageMix spoon && isWaitingSpoon)
            {
                this.spoon = spoon;
                isWaitingSpoon = false;
                spoon.TF.SetParent(TF.transform);
                item.OnMove(spoonTF.position + Vector3.up * 0.3f + Vector3.right * 0.25f, Quaternion.Euler(Vector3.forward * -130f), 0.2f);
                item.ChangeState(SpoonCabbageMix.State.Mixing);

                ChangeState(State.Mixing);

                return true;
            }
            return base.OnTake(item);
        }

        IEnumerator WaitForPouringDill()
        {
            yield return WaitForSecondCache.Get(0.5f);
            squareDillAlpha.DoAlpha(1, 0.5f);
            yield return WaitForSecondCache.Get(0.5f);
            isWaitingPepper = true;
        }

        IEnumerator WaitForPouringPepper()
        {
            yield return WaitForSecondCache.Get(0.5f);
            pepperDrop2D.OnActive();
            yield return WaitForSecondCache.Get(0.5f);
            isWaitingSpoon = true;
        }

        public override void OnClickDown()
        {
            base.OnClickDown();
            if (IsState(State.Mixing))
            {
                isClick = true;
                return;
            }
            if (fork != null) ///co the loi o day, chua set fork = null khi bam xong
            {
                fork.OnClickDown();
                return;
            }
        }

        public override bool IsState<T>(T t)
        {
            return state == (State)(object)t;
        }

        [Button]
        public override void ChangeState<T>(T t)
        {
            state = (State)(object)t;

            switch (state)
            {
                case State.Mixing:
                    ellipse2D = new Ellipse2D(0.05f, 0.05f, spoonTF.localPosition);
                    break;
            }
        }

        [Button]

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
                angle = Mathf.Clamp(angle, -20, 20);
                ellipseTimer += Time.deltaTime * (-angle / 20f);
                timer += Time.deltaTime * (Mathf.Abs(angle) / 20f);
                spoon.TF.localPosition = ellipse2D.Evaluate(ellipseTimer) + Vector2.up * 0.3f + Vector2.right * 0.2f;


                if (timer >= 0 && timer < 2)
                {
                    squareDillAlpha.SetAlpha(1 - timer / 2);
                    squareCabbageCuttingAlpha.SetAlpha(1 - timer / 2);
                    squareMix1.SetAlpha(timer / 2);

                    if (animSquareDill.isPlaying == false)
                    {
                        animSquareDill.Play();
                    }
                    if (animSquareMix1.isPlaying == false)
                    {
                        animSquareMix1.Play();
                    }
                }
                else if (timer >= 2 && timer < 4)
                {
                    squareMix1.SetAlpha(1 - (timer - 2) / 2);
                    squareMix2.SetAlpha((timer - 2) / 2);

                    if (animSquareMix1.isPlaying == false)
                    {
                        animSquareMix1.Play();
                    }
                }
                else if (timer >= 4)
                {
                    ChangeState(State.Done);
                    spoon.ChangeState(SpoonCabbageMix.State.Done);
                    blinkVFX.Play();
                }
                mousePos = newMousePos;
            }
        }

        public void ReleaseFork()
        {
            fork = null;
        }

        private void OnDrawGizmos()
        {
            if (ellipse2D != null)
                ellipse2D.OnDrawGizmos(TF.position);
        }
    }
}