using DG.Tweening;
using Link;
using UnityEngine;
using VinhLB;

namespace HuyThanh.Cooking.Burrito
{
    public class Corn : ItemMovingBase
    {
        public enum State { OpenLeaf, Normal, Done }
        [SerializeField] State state;

        [SerializeField] Animation anim;
        [SerializeField] string animClean;

        [SerializeField] Transform targetDoneTF; //target TF khi ma rua xong
        [SerializeField] int targetOrderLayer;

        [SerializeField] private ModifiedMeshFoldDraggable meshFoldLeafOne, meshFoldLeafTwo;

        [SerializeField] HintText cornHintText;

        public override bool IsCanMove => IsState(State.Normal);
        public override bool IsState<T>(T t)
        {
            return state == (State)(object)t;
        }

        public override void ChangeState<T>(T t)
        {
            state = (State)(object)t;

            switch (state)
            {
                case State.OpenLeaf:

                    break;
                case State.Normal:
                    break;
                case State.Done:
                    cornHintText.OnActiveHint();
                    LevelControl.Ins.NextHint();
                    OnMove(targetDoneTF.position, targetDoneTF.rotation, 0.2f);
                    DOVirtual.DelayedCall(0.2f, () =>
                    {
                        anim.Play(animClean);
                        OrderLayer = targetOrderLayer;
                        collider.enabled = false;
                    });
                    break;

            }
        }

        private void OnEnable()
        {
            meshFoldLeafOne.onDetached += UnPackLeafOne;
            meshFoldLeafTwo.onDetached += UnPackLeafTwo;
        }

        private void OnDisable()
        {
            meshFoldLeafOne.onDetached -= UnPackLeafOne;
            meshFoldLeafTwo.onDetached -= UnPackLeafTwo;
        }

        public void UnPackLeafOne(ModifiedMeshFoldDraggable draggable)
        {
            meshFoldLeafTwo.enabled = true;
        }

        public void UnPackLeafTwo(ModifiedMeshFoldDraggable draggable)
        {
            ChangeState(State.Normal);
        }

        public override void OnClickDown()
        {
            base.OnClickDown();
            SoundControl.Ins.PlayFX(Fx.Click);
        }

        public override void OnClickTake()
        {
            base.OnClickTake();
            SoundControl.Ins.PlayFX(Fx.Take);
        }
    }

}