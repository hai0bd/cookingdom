using DG.Tweening;
using Link;
using Sirenix.OdinInspector;
using UnityEngine;


namespace HuyThanh.Cooking.Burrito
{


    public class Knife : ItemMovingBase
    {
        public enum State
        {
            Normal,
            Cutting,
            KatanaCutting,
            Peelonion,
            CornCutting,
            Done
        }

        public override bool IsCanMove => IsState(State.Normal);

        [SerializeField] State state;
        [SerializeField] Vector3 rotOnClick;
        [SerializeField] Animation anim;

        [SerializeField] string animKatanaCut;
        [SerializeField] string animPeelOnion;

        [SerializeField] ParticleSystem katanaVFX;

        [BoxGroup("Knife Front and Back")][SerializeField] GameObject knifeFront, knifeBack;

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
                case State.Cutting:
                    ShowFront(isFront: false);
                    break;
                case State.Peelonion:
                    ShowFront(isFront: false);
                    anim.Play(animPeelOnion);
                    DOVirtual.DelayedCall(1f, () => ChangeState(State.Done));
                    break;
                case State.KatanaCutting:
                    ShowFront(isFront: true);
                    katanaVFX.Play();
                    anim.Play(animKatanaCut);
                    DOVirtual.DelayedCall(1f, () => ChangeState(State.Done));
                    break;
                case State.CornCutting:
                    ShowFront(isFront: true);
                    break;

                case State.Done:
                    ShowFront(isFront: true);
                    OnBack();
                    ChangeState(State.Normal);
                    break;
            }
        }

        private void ShowFront(bool isFront)
        {
            knifeFront.SetActive(isFront);
            knifeBack.SetActive(!isFront);
        }

        public override void OnClickDown()
        {
            SoundControl.Ins.PlayFX(Fx.Click);
            base.OnClickDown();
            TF.DORotate(rotOnClick, 0.1f);
        }

        public void ChangeAnim(string name)
        {
            anim.Stop(name);
            anim.Play(name);
        }

        public override void OnClickTake()
        {
            base.OnClickTake();
            OrderLayer = 40;
        }
    }

}