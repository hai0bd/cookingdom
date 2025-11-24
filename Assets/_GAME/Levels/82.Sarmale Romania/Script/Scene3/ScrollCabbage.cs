using DG.Tweening;
using Link;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HuyThanh.Cooking.SarmaleRomania
{
    public class ScrollCabbage : ItemIdleBase
    {
        public enum State
        {
            Normal,
            HaveNhan,
            Scroll1,
            Scroll2,
            DoneScroll,
            DoneCabbage,
        }

        [SerializeField] State state;
        [SerializeField] Transform cabbage1TF, cabbage2TF;
        [SerializeField] Transform itemScroll, startPointScroll, endPointScroll;
        [SerializeField] GameObject scroll1, scroll2, scroll3;
        [SerializeField] GameObject leaf1, leaf2, leaf3;
        [SerializeField] ItemAlpha nhanBanh;
        [SerializeField] Animation anim;
        [SerializeField] List<DoneCabbage> doneCabbages;
        [SerializeField] string scrollAnimName;
        [SerializeField] float moveSpeed;

        private CabbageRaw cabbage1, cabbage2;
        private DoneCabbage currentDoneCabbage;
        private bool haveNhan = false;

        private Vector3 startMousePos;


        public override bool IsState<T>(T t)
        {
            return state == (State)(object)t;
        }

        public override void ChangeState<T>(T t)
        {
            state = (State)(object)t;

            switch (state)
            {
                case State.Normal:
                    cabbage1 = cabbage2 = null;
                    break;
                case State.HaveNhan:

                    break;
                case State.Scroll1:
                    itemScroll.gameObject.SetActive(true);
                    leaf1.SetActive(false);
                    leaf2.SetActive(true);
                    scroll1.SetActive(true);
                    break;
                case State.Scroll2:

                    nhanBanh.DoAlpha(0, 0.1f);
                    scroll1.SetActive(false);
                    scroll2.SetActive(true);
                    leaf2.SetActive(false);
                    leaf3.SetActive(true);
                    break;
                case State.DoneScroll:
                    anim.Play(scrollAnimName);
                    scroll2.SetActive(false);
                    scroll3.SetActive(true);
                    leaf3.SetActive(false);
                    itemScroll.DOMove(startPointScroll.position, 0.2f);
                    StartCoroutine(WaitForDoneCabbage());
                    break;
                case State.DoneCabbage:
                    break;
            }
        }

        public override bool OnTake(IItemMoving item)
        {
            if (IsState(State.DoneCabbage) && currentDoneCabbage.IsOnDonePlate)
            {
                ChangeState(State.Normal);
            }

            if (IsState(State.Normal) && item is CabbageRaw && (cabbage1 == null || cabbage2 == null))
            {
                if (cabbage1 == null)
                {
                    cabbage1 = item as CabbageRaw;
                    item.OnMove(cabbage1TF.position, TF.rotation, 0.2f);
                    item.OnDone();
                    return true;
                }

                if (cabbage2 == null)
                {
                    cabbage2 = item as CabbageRaw;
                    item.OnMove(cabbage2TF.position, TF.rotation, 0.2f);
                    item.OnDone();
                    return true;
                }
            }

            if (item is SpoonBase spoon && spoon.CanTake())
            {
                cabbage1.gameObject.SetActive(false);
                cabbage2.gameObject.SetActive(false);
                leaf1.SetActive(true);

                item.OnMove(TF.position, spoon.TF.rotation, 0.2f);
                item.ChangeState(SpoonBase.State.Pouring);

                StartCoroutine(WaitForDonePouring());

                return true;
            }
            return base.OnTake(item);
        }

        public override void OnClickDown()
        {
            base.OnClickDown();

            if (IsState(State.HaveNhan))
            {
                ChangeState(State.Scroll1);
                return;
            }
        }

        private void OnMouseDown()
        {
            startMousePos = Input.mousePosition;
        }

        private void OnMouseDrag()
        {
            if (!IsState(State.Scroll1) && !IsState(State.Scroll2))
            {
                return;
            }

            Vector3 mousePos = Input.mousePosition;

            float deltaY = mousePos.y - startMousePos.y;

            startMousePos = mousePos;

            if (deltaY <= 0)
            {
                return;
            }

            anim.Play(scrollAnimName);

            itemScroll.position = Vector3.MoveTowards(itemScroll.position, endPointScroll.position, deltaY * moveSpeed * Time.deltaTime);

            if (IsState(State.Scroll1) && Vector3.Distance(itemScroll.position, startPointScroll.position) > 0.05f)
            {
                ChangeState(State.Scroll2);
            }

            if (IsState(State.Scroll2) && Vector3.Distance(itemScroll.position, startPointScroll.position) >= 0.45f)
            {
                ChangeState(State.DoneScroll);
            }
        }

        IEnumerator WaitForDoneCabbage()
        {
            yield return WaitForSecondCache.Get(0.2f);
            currentDoneCabbage = doneCabbages[0];
            doneCabbages[0].gameObject.SetActive(true);
            doneCabbages.RemoveAt(0);

            leaf1.SetActive(false);
            leaf2.SetActive(false);
            leaf3.SetActive(false);
            scroll1.SetActive(false);
            scroll2.SetActive(false);
            scroll3.SetActive(false);
            itemScroll.gameObject.SetActive(false);
            ChangeState(State.DoneCabbage);
        }

        IEnumerator WaitForDonePouring()
        {
            nhanBanh.DoAlpha(1, 0.2f);
            anim.Play(scrollAnimName);
            yield return WaitForSecondCache.Get(0.5f);
            ChangeState(State.HaveNhan);
        }
    }

}
