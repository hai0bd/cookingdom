
using DG.Tweening;
using Link;
using Sirenix.OdinInspector;
using UnityEngine;
namespace HuyThanh.Cooking.SarmaleRomania
{
    public class ChoppingCleaver : ItemMovingBase
    {
        public enum State
        {
            Normal,
            Piceing,

            Done
        }

        public override bool IsCanMove => IsState(State.Normal);

        [SerializeField] State state;
        [SerializeField] Vector3 rotOnClick;
        [SerializeField] Animation anim;

        [SerializeField] string animChopingcut;



        [BoxGroup("Knife chop and knift")][SerializeField] GameObject knife, knifeChop;

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
                case State.Piceing:
                    ShowChop();

                    break;
                case State.Done:
                    HideChop();
                    OnBack();
                    ChangeState(State.Normal);
                    break;
            }
        }

        private void ShowChop()
        {
            knife.SetActive(false);
            knifeChop.SetActive(true);
        }
        private void HideChop()
        {
            knife.SetActive(true);
            knifeChop.SetActive(false);
        }
        public override void OnClickDown()
        {
            base.OnClickDown();
            TF.DORotate(rotOnClick, 0.1f);
        }

        public void ChangeAnim(string name)
        {
            anim.Stop(name);
            anim.Play(name);
        }
    }

}

