using Link;
using UnityEngine;

namespace HuyThanh.Cooking.SarmaleRomania
{
    public class CabbageLeafMoveToBowl : ItemMovingBase
    {
        public enum State
        {
            Normal,
            Done,
        }

        [SerializeField] State state;
        [SerializeField] Animation anim;
        [SerializeField] CabbageLeaf cabbageLeaf; ///use for release cuttingboard item
        [SerializeField] ItemAlpha mainSpriteAlpha, leafOnDishAlpha;

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
                case State.Done:
                    cabbageLeaf.ReleaseCuttingBoard();
                    break;
            }
        }

        public void PlayAnim()
        {
            anim.Play("Bounce");
        }

        public void ShowOnDish()
        {
            mainSpriteAlpha.DoAlpha(0, 0.5f);
            leafOnDishAlpha.DoAlpha(1, 0.5f);
            PlayAnim();
        }
    }
}