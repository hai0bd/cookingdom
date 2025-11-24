using Link;
using System.Collections;
using UnityEngine;

namespace HuyThanh.Cooking.Burrito
{
    public enum BottleItemType
    {
        Mayo,
        Chili
    }
    public class BottleItem : ItemMovingBase
    {
        public enum State
        {
            Normal,
            Pouring,
            Done
        }
        [SerializeField] State state;
        [SerializeField] BottleItemType type;

        [SerializeField] Animation anim;
        [SerializeField] string animPouringName;


        [SerializeField] HintText hintText;

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
                case State.Pouring:
                    anim.Play(animPouringName);
                    StartCoroutine(WaitForDonePouring());
                    break;
                case State.Done:
                    hintText.OnActiveHint();
                    LevelControl.Ins.CheckStep(1f);
                    OnBack();
                    break;
            }
        }

        IEnumerator WaitForDonePouring()
        {
            yield return new WaitForSeconds(1.6f);
            ChangeState(State.Done);
            OnDrop();
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

        public bool IsType(BottleItemType t)
        {
            return type == t;
        }
    }
}