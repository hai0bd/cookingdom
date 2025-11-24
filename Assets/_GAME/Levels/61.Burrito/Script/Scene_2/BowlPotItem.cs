using Link;
using System.Collections;
using UnityEngine;

namespace HuyThanh.Cooking.Burrito
{
    public class BowlPotItem : ItemMovingBase
    {
        public enum State
        {
            Normal,
            Pouring,
            WaitingCookSpice,
            Done
        }
        [SerializeField] private State state;

        [SerializeField] GameObject activeItem;
        [SerializeField] Animation anim;
        [SerializeField] string animPouring;
        [SerializeField] string animTakeItem;

        [SerializeField] Punctured.SpiceType spiceType;
        [SerializeField] GameObject[] activeItemSprite;

        [SerializeField] HintText hintText;

        public Punctured.SpiceType SpiceType => spiceType;
        public override bool IsCanMove => IsState(State.Normal);

        private int currentTake = -1;

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
                    // Handle normal state
                    break;
                case State.Pouring:
                    anim.Play(animPouring);
                    SoundControl.Ins.PlayFX(Fx.BeanPouring);
                    StartCoroutine(WaitForEndPouring());
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

        IEnumerator WaitForEndPouring()
        {
            yield return WaitForSecondCache.Get(1.2f);
            ChangeState(State.WaitingCookSpice);
        }

        public void ActiveSpirte()
        {

            if (currentTake >= activeItemSprite.Length)
            {
                return;
            }
            SoundControl.Ins.PlayFX(Fx.Click);

            currentTake++;
            activeItemSprite[currentTake].SetActive(true);
            anim.Play(animTakeItem);
            if (currentTake == activeItemSprite.Length - 1)
            {
                ChangeState(State.Done);
            }
        }
    }
}
