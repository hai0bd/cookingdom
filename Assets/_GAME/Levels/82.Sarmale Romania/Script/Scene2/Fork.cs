using DG.Tweening;
using Link;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HuyThanh.Cooking.SarmaleRomania
{
    public class Fork : ItemMovingBase
    {
        public enum State
        {
            Normal,
            StickingCabbage,
            HoldingCabbage,
            HoldingCabbageOnBowl,
            DoneRemoveLeaf,
            Done,
        }

        [SerializeField] State state;
        [SerializeField] Transform cabbageTF;
        [SerializeField] CabbageCook cabbageCook;
        [SerializeField] CabbageBowlLeaf cabbageBowl;
        [SerializeField] BoxCollider2D boxCollider2D;
        [SerializeField] Animation anim;

        [SerializeField] List<CabbageLeafMove> cabbageLeafMoves;
        [SerializeField] List<CabbageLeaf> cabbageLeaves;

        [SerializeField] Vector3 startPos;

        private List<CabbageLeaf> cabbageLeavesActive = new List<CabbageLeaf>();
        public override bool IsCanMove => IsState(State.Normal, State.HoldingCabbage);
        public override bool IsState<T>(T t)
        {
            return state == (State)(object)t;
        }

        public override void ChangeState<T>(T t)
        {
            state = (State)(object)t;

            switch (state)
            {
                case State.StickingCabbage:
                    saveRot = TF.rotation;
                    anim.Play("ForkStickingCabbage");
                    StartCoroutine(WaitForStickingCabbage());
                    break;
                case State.DoneRemoveLeaf:

                    foreach (CabbageLeaf cabbageLeaf in cabbageLeavesActive)
                    {
                        cabbageLeaf.enabled = true;
                    }
                    cabbageCook.DoneRemoveLeaf();

                    anim.Play("ForkDoneRemoveLeaf");
                    StartCoroutine(WaitForTurnBack());
                    break;
            }
        }

        public override void OnClickDown()
        {
            if (IsState(State.HoldingCabbageOnBowl))
            {
                RemoveLeaf();
                ShowCabbageLeaf();

                if (cabbageLeafMoves.Count == 0)
                {
                    cabbageBowl.ReleaseFork();
                    ChangeState(State.DoneRemoveLeaf);
                }
                return;
            }
            base.OnClickDown();
            TF.DORotate(Vector3.forward * -130f, 0.1f);
        }

        private void RemoveLeaf()
        {
            cabbageLeafMoves[0].OnMove();
            cabbageLeafMoves.RemoveAt(0);
        }

        private void ShowCabbageLeaf()
        {
            cabbageLeavesActive.Add(cabbageLeaves[0]);

            cabbageLeaves[0].OnShowUp();
            cabbageLeaves.RemoveAt(0);
        }

        IEnumerator WaitForStickingCabbage()
        {
            yield return WaitForSecondCache.Get(0.5f);
            OnSavePoint();
            yield return WaitForSecondCache.Get(0.2f);
            cabbageCook.PlayAnim();
            cabbageCook.TF.SetParent(cabbageTF);
            ChangeState(State.HoldingCabbage);
        }

        IEnumerator WaitForTurnBack()
        {
            yield return WaitForSecondCache.Get(0.5f);
            OnMove(startPos, Quaternion.Euler(Vector3.forward * 90f), 0.2f);
        }
    }
}