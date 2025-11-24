using DG.Tweening;
using Link;
using System.Collections;
using UnityEngine;


namespace HuyThanh.Cooking.Burrito
{
    public class Spoon : ItemMovingBase
    {
        public enum State
        {
            Normal,
            Pouring,
            Done
        }
        [SerializeField] State state;
        [SerializeField] SpiceSpoon spiceSpoon;
        [SerializeField] Transform spiceItemTF;
        [SerializeField] Animation anim;

        [SerializeField] SpriteRenderer spiceSprite;
        [SerializeField] Vector3 pouringPosition;

        private int numberOfPour = 0;
        private bool isHaveItem = false;
        private bool isMoving = false;

        public bool IsMoving => isMoving;

        public override bool IsCanMove => IsState(State.Normal);

        Tween dropTween;

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
                    break;
                case State.Pouring:
                    isHaveItem = false;
                    anim.Play("SpoonSpicePouring");
                    SoundControl.Ins.PlayFX(Fx.PouringSpice);
                    StartCoroutine(WaitForDonePouring());
                    break;
                case State.Done:
                    collider.enabled = false;
                    break;
            }
        }

        public override void OnClickDown()
        {
            SoundControl.Ins.PlayFX(Fx.Click);
            base.OnClickDown();
            isMoving = true;
            if (isHaveItem == false)
            {
                isHaveItem = true;
                if (spiceSprite.color.a < 0.1f)
                {
                    spiceSprite.color = Color.white;
                }
                spiceItemTF.DOScale(1, 0.05f);
                spiceSpoon.SpoonTake();
            }
        }

        public override void OnDrop()
        {
            SoundControl.Ins.PlayFX(Fx.PutDown);
            base.OnDrop();
            isMoving = false;

            isCanControl = false;
            collider.enabled = false;
            dropTween = DOVirtual.DelayedCall(1f, () =>
            {
                isCanControl = true;
                collider.enabled = true;
            });

            dropTween = DOVirtual.DelayedCall(0.3f, () =>
            {
                if (isHaveItem == true)
                {
                    isHaveItem = false;

                    spiceItemTF.DOScale(0, 0.1f);
                    spiceSpoon.SpoonUntake();
                }
            });
        }

        public override void OnBack()
        {
            base.OnBack();
            isMoving = false;
        }

        IEnumerator WaitForDonePouring()
        {
            yield return WaitForSecondCache.Get(1f);
            OnBack();
            numberOfPour++;
            if (numberOfPour >= 2)
            {
                ChangeState(State.Done);
            }
            else
            {
                ChangeState(State.Normal);
            }
        }

        public Vector3 GetPouringPosition()
        {
            return pouringPosition;
        }
    }

}
