using HuyThanh.Cooking.Burrito;
using Link;
using System.Collections;
using UnityEngine;

namespace Hai.Cooking.Burrito {
    public class BowlPotItem : ItemMovingBase
    {
        public enum State
        {
            Normal,
            Pouring,
            WaitingCookSpice,
            Done
        }

        [SerializeField ]private State state;

        [SerializeField] private GameObject activeItem;
        [SerializeField] private Animation anim;
        [SerializeField] private string animPouring;
        [SerializeField] private string animTakeItem;

        [SerializeField] private Punctured.SpiceType spiceType;
        [SerializeField] private GameObject[] activeItemSprite;
        [SerializeField] private HintText hintText;

        public override bool IsCanMove => IsState(State.Normal);
        public Punctured.SpiceType SpiceType => spiceType;

        private int currentTake = 1;

        public override bool IsState<T>(T t)
        {
            return state == (State)(object) t;
        }

        public override void ChangeState<T>(T t)
        {
            state = (State)(object) t;

            switch (state)
            {
                case State.Normal:
                    break;
                case State.Pouring:
                    anim.Play(animPouring);
                    SoundControl.Ins.PlayFX(Fx.BeanPouring);
                    StartCoroutine(WaitForPouring());
                    break;
                case State.WaitingCookSpice:
                    activeItem.SetActive(true);
                    OnBack();
                    break;
                case State.Done:
                    SoundControl.Ins.PlayFX(Fx.DoneSomething);
                    hintText.OnActiveHint();
                    LevelControl.Ins.NextHint();
                    break;

            }
        }

        IEnumerator WaitForPouring()
        {
            yield return WaitForSecondCache.Get(1.2f);
            ChangeState(State.WaitingCookSpice);
        }

        public void ActiveSprite()
        {
            if (currentTake >= activeItemSprite.Length) return;
            SoundControl.Ins.PlayFX(Fx.Click);

            currentTake++;
            activeItemSprite[currentTake].SetActive(true);
            anim.Play(animTakeItem);
            if(currentTake == activeItemSprite.Length - 1)
            {
                ChangeState(State.Done);
            }
        }
    }
}
