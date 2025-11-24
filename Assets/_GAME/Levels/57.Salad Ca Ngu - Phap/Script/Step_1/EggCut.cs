using DG.Tweening;
using Link;
using UnityEngine;

namespace HuyThanh.Cooking.TunaSaladFrench
{
    public class EggCut : ItemMovingBase
    {
        public enum State
        {
            Normal,
            Cutting,
            Spice,
            Done
        }
        [SerializeField] private State state;
        [SerializeField] private GameObject origin, cutting, done;
        [SerializeField] private Egg egg;
        public override bool IsCanMove => IsState(EggCut.State.Spice);

        public override bool IsState<T>(T t)
        {
            return state == (State)(object)t;
        }

        public override void ChangeState<T>(T t)
        {
            state = (State)(object)t;

            switch (state)
            {
                case State.Cutting:
                    DOVirtual.DelayedCall(0.2f, () =>
                    {
                        origin.SetActive(false);
                        cutting.SetActive(true);
                    });
                    DOVirtual.DelayedCall(.8f, () =>
                    {
                        cutting.SetActive(false);
                        done.SetActive(true);
                    });
                    DOVirtual.DelayedCall(.8f, () =>
                    {
                        ChangeState(EggCut.State.Spice);
                    });
                    break;
                case State.Spice:

                    break;
                case State.Done:
                    egg.DoneCutting();
                    done.SetActive(false);
                    collider.enabled = false;
                    break;
            }
        }

        public override bool OnTake(IItemMoving item)
        {
            if (item is Knife && this.IsState(EggCut.State.Normal))
            {
                this.ChangeState(EggCut.State.Cutting);
                item.ChangeState(Knife.State.CuttingEgg);
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                return true;
            }
            return base.OnTake(item);
        }

        public override void OnClickDown()
        {
            base.OnClickDown();
            SoundControl.Ins.PlayFX(Fx.Click);
        }
        public override void OnClickTake()
        {
            base.OnClickTake();
            SoundControl.Ins.PlayFX(Fx.Take);
        }
    }
}