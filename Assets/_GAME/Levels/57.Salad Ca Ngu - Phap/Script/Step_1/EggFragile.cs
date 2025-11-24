using Link;
using UnityEngine;

namespace HuyThanh.Cooking.TunaSaladFrench
{
    public class EggFragile : ItemMovingBase
    {

        public enum State
        {
            Normal,
            Fragile,
            Done
        }
        [SerializeField] private State state;
        [SerializeField] private GameObject fragile, breaking;
        [SerializeField] private Sprite hint;
        public override bool IsCanMove => IsState(EggFragile.State.Fragile);

        public override bool IsState<T>(T t)
        {
            return state == (State)(object)t;
        }

        public override void ChangeState<T>(T t)
        {
            state = (State)(object)t;

            switch (state)
            {
                case State.Fragile:
                    breaking.SetActive(false);
                    fragile.SetActive(true);
                    break;
                case State.Done:
                    LevelControl.Ins.CheckStep();
                    break;
            }
        }



        public override void OnDrop()
        {
            base.OnDrop();
        }

        public override void OnClickDown()
        {
            base.OnClickDown();
            LevelControl.Ins.SetHint(hint);
            ///sound nhat vo trung
        }
    }
}