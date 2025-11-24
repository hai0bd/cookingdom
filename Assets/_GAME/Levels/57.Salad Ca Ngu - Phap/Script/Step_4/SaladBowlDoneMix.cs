using DG.Tweening;
using Link;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HuyThanh.Cooking.TunaSaladFrench
{
    public class SaladBowlDoneMix : ItemMovingBase
    {
        // Start is called before the first frame update

        public enum State { Normal, Pouring, Done }
        public enum Type { Pepper, Wine, Oil }

        [SerializeField] State state;
        [SerializeField] Type type;
        [SerializeField] GameObject itemActive;
        [BoxGroup("Anim Name")][SerializeField] Animation anim;
        [BoxGroup("Anim Name")][SerializeField] string animName;

        public override bool IsCanMove => IsState(Wine.State.Normal);

        public override void ChangeState<T>(T t)
        {
            state = (State)(object)t;
            switch (state)
            {
                case State.Normal:
                    break;
                case State.Pouring:
                    anim.Play(animName);
                    //SoundControl.Ins.PlayFX(LevelStep_1.Fx.Pour);
                    DOVirtual.DelayedCall(0.2f, () => OrderLayer = 50);
                    DOVirtual.DelayedCall(0.5f, () => itemActive.SetActive(true));
                    DOVirtual.DelayedCall(2.5f, () => ChangeState(State.Done));
                    break;
                case State.Done:
                    OnBack();
                    break;
                default:
                    break;
            }
        }

        public override bool IsState<T>(T t)
        {
            return state == (State)(object)t;
        }

        public override void OnClickDown()
        {
            base.OnClickDown();
            SoundControl.Ins.PlayFX(Fx.Click);
        }

        public override void OnClickTake()
        {
            base.OnClickTake();
            //SoundControl.Ins.PlayFX(LevelStep_1.Fx.Take);
        }

        public override void OnDrop()
        {
            base.OnDrop();
            OnBack();
        }

    }

}


