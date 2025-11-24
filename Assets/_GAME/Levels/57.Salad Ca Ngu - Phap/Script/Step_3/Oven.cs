using DG.Tweening;
using Link;
using Satisgame;
using UnityEngine;

namespace HuyThanh.Cooking.TunaSaladFrench
{
    public class Oven : ItemIdleBase
    {
        public enum State
        {
            Normal,
            Open,
            Baking,
            Baked,
            Done
        }
        [SerializeField] State state;

        [SerializeField] Animation anim;
        [SerializeField] string animOpen;
        [SerializeField] string animClose;
        [SerializeField] ItemAlpha ovenBakingSprite;
        [SerializeField] EmojiControl _emojiControl;
        [SerializeField] ClockTimer _clockTimer;

        [SerializeField] Sprite hint;
        private PanOvenMoving pan;


        private bool isOpen = false;

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
                    this.GetComponent<BoxCollider2D>().size = new Vector2(2.8f, 2f);
                    break;
                case State.Open:
                    this.GetComponent<BoxCollider2D>().size = new Vector2(2.8f, 3.7f);
                    break;
                case State.Baking:
                    ovenBakingSprite.DoAlpha(1f, 0.5f);

                    _clockTimer.Show(4f);
                    SoundControl.Ins.PlayFX(Fx.OvenBaking);
                    DOVirtual.DelayedCall(4f, () => ChangeState(State.Baked));
                    break;
                case State.Baked:
                    ovenBakingSprite.DoAlpha(0f, 0.5f);
                    LevelControl.Ins.SetHint(hint); /// hint dong nap va tat bep
                    OvenButton(false); /// mo nap
                    break;
                case State.Done:
                    this.GetComponent<BoxCollider2D>().size = new Vector2(2.8f, 2f);
                    _emojiControl.ShowPositive();
                    MiniGame5.Instance.CheckDone(0.2f);
                    LevelControl.Ins.CheckStep(1f);
                    break;
            }
        }

        public override bool OnTake(IItemMoving item)
        {
            if (item is PanOvenMoving && item.IsState(PanOvenMoving.State.Normal) && IsState(Oven.State.Open))
            {
                //item.OnMove(TF.position + Vector3.left * 0.2f + Vector3.down * 0.4f, Quaternion.identity, 0f);/// cho no di xuong 1 ti de lam anim
                //item.OnMove(TF.position + Vector3.left * 0.2f, Quaternion.identity, 0.2f);

                this.pan = (PanOvenMoving)item;
                item.OnMove(TF.position + Vector3.down * 0.4f, Quaternion.identity, 0f);/// cho no di xuong 1 ti de lam anim
                item.OnMove(TF.position, Quaternion.identity, 0.2f);

                OvenButton(true); /// dong nap
                item.OnSave(0.2f);
                item.ChangeState(PanOvenMoving.State.Baking);
                ChangeState(State.Baking);
                return true;
            }

            return base.OnTake(item);
        }

        public override void OnClickDown()
        {
            base.OnClickDown();
            if (IsState(Oven.State.Normal))
            {
                OvenButton(false);/// mo nap
                ChangeState(Oven.State.Open);
                return;
            }
            if (IsState(Oven.State.Baked) && pan != null && Vector2.Distance(pan.TF.position, TF.position) > 0.5f && pan.IsState(PanOvenMoving.State.Done))
            {
                OvenButton(true); ///dong nap
                ChangeState(Oven.State.Done);
                return;
            }
            if (IsState(Oven.State.Open))
            {
                OvenButton(true);/// dong nap
                ChangeState(Oven.State.Normal);
                return;
            }
        }

        public void OvenButton(bool isOn)
        {
            SoundControl.Ins.PlayFX(Fx.OvenOpen);
            if (isOn)
            {
                anim.Play(animOpen);
            }
            else
            {
                anim.Play(animClose);
            }
        }
    }

}