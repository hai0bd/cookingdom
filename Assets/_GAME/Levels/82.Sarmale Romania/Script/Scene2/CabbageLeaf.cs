using DG.Tweening;
using Link;
using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

namespace HuyThanh.Cooking.SarmaleRomania
{

    public class CabbageLeaf : ItemMovingBase
    {
        public enum State
        {
            WaitingShow,
            Normal,
            ReadyCutting,
            Cutting,
            Done,
        }

        [SerializeField] State state;
        [SerializeField] Animation anim;
        [SerializeField] ItemAlpha squareItemAlpha, squareLeafMiddle;

        [SerializeField] CabbageLeafMoveToBowl leafLeft, leafRight;
        [SerializeField] Transform leafMiddle, square;

        private Knife knife;
        private CuttingBoard cuttingBoard;
        private int countDoneLeaf = 0;
        private bool isCanCut = true;
        private bool cutLeaf, cutRight;
        public override bool IsCanMove => IsState(State.Normal);

        override public bool IsState<T>(T t)
        {
            return state == (State)(object)t;
        }

        override public void ChangeState<T>(T t)
        {
            state = (State)(object)t;
            switch (state)
            {
                case State.ReadyCutting:
                    anim.Play();
                    square.gameObject.SetActive(false);
                    leafRight.gameObject.SetActive(true);
                    leafLeft.gameObject.SetActive(true);
                    leafMiddle.gameObject.SetActive(true);
                    break;
                case State.Cutting:
                    knife.OnMove(leafLeft.TF.position + Vector3.right * 0.2f, Quaternion.identity, 0.2f);
                    break;
                case State.Done:
                    ///set collider cho may thang la, rieng thang middle thi cho bay sang
                    leafLeft.enabled = true;
                    leafRight.enabled = true;

                    squareLeafMiddle.DoAlpha(0, 0.1f);

                    ///cuttingboard 

                    break;
            }
        }

        public override void OnClickDown()
        {
            if (IsState(State.Cutting))
            {
                if (isCanCut == false)
                {
                    return;
                }
                StartCoroutine(WaitForKnifeCut());
                knife.ChangeAnim("KnifeCut");

                if (cutLeaf == false)
                {
                    cutLeaf = true;
                    leafLeft.PlayAnim();
                    leafLeft.TF.DOMove(leafLeft.TF.position + Vector3.left * 0.1f, 0.1f);
                    knife.OnMove(leafRight.TF.position, Quaternion.identity, 0.2f);
                }
                else if (cutRight == false)
                {
                    cutRight = true;
                    leafRight.PlayAnim();
                    leafRight.TF.DOMove(leafRight.TF.position + Vector3.right * 0.1f, 0.1f);
                }

                if (cutLeaf && cutRight)
                {
                    StartCoroutine(WaitForChangeStateDone());
                    knife.ChangeState(Knife.State.Done);
                    cuttingBoard.DoMiddleLeavesMove();
                }
                return;
            }
            base.OnClickDown();
        }

        IEnumerator WaitForChangeStateDone()
        {
            yield return WaitForSecondCache.Get(0.2f);
            ChangeState(State.Done);
        }

        IEnumerator WaitForKnifeCut()
        {
            isCanCut = false;
            yield return WaitForSecondCache.Get(0.2f);
            isCanCut = true;
        }

        public void OnShowUp()
        {
            StartCoroutine(WaitForShowUp());
        }

        IEnumerator WaitForShowUp()
        {
            squareItemAlpha.DoAlpha(1, 0.5f);
            OnSavePoint();
            yield return WaitForSecondCache.Get(0.5f);
            anim.Play("Bounce");
            ChangeState(State.Normal);
        }

        [Button]
        public void ItemAlphaSetUp()
        {
            anim = GetComponent<Animation>();
            squareItemAlpha = GetComponent<ItemAlpha>();
        }

        public void PlayAnim()
        {
            anim.Play("Bounce");
        }

        public void SetKnife(Knife knife)
        {
            this.knife = knife;
        }

        public void SetCuttingBoard(CuttingBoard cuttingBoard)
        {
            this.cuttingBoard = cuttingBoard;
        }

        public void ReleaseCuttingBoard()
        {
            countDoneLeaf++;
            if (countDoneLeaf == 2)
            {
                cuttingBoard.ReleaseCabbageLeaf();
            }
        }

    }
}