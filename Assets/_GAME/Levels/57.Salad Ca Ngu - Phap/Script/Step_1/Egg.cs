using DG.Tweening;
using Link;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace HuyThanh.Cooking.TunaSaladFrench
{
    public class Egg : ItemMovingBase
    {
        public enum State
        {
            Normal,
            Cooking,
            Cooked,
            Breaking,
            Cutting,
            Spice,
            Done
        }

        [SerializeField] private State state;

        [SerializeField] private int countingClickToFragile;
        [SerializeField] private ParticleSystem vfxBlink;
        [SerializeField] private ParticleSystem vfxSmoke;

        [BoxGroup("Animation")][SerializeField] private Animation anim;
        [BoxGroup("Animation")][SerializeField] private string animMoveOut;

        [BoxGroup("Breaking and Cutting State")][SerializeField] private List<GameObject> originEggs;
        [BoxGroup("Breaking and Cutting State")][SerializeField] private List<GameObject> breakingEggs;
        [BoxGroup("Breaking and Cutting State")][SerializeField] private List<GameObject> cuttingEggs;

        [SerializeField] Sprite hint;

        [SerializeField] Vector3 positionOnBoard;

        public override bool IsCanMove => IsState(Egg.State.Normal);

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
                case State.Cooking:
                    OrderLayer = 3;
                    DOVirtual.DelayedCall(4f, () => ChangeState(Egg.State.Cooked));
                    break;
                case State.Cooked:
                    vfxSmoke.Play();
                    vfxBlink.Play();
                    break;
                case State.Breaking:

                    countingClickToFragile = 0;
                    originEggs[0].transform.DOMove(positionOnBoard, 0.4f);
                    originEggs[0].transform.DORotate(Vector3.zero, 0.4f);

                    DOVirtual.DelayedCall(.4f, () =>
                    {
                        if (breakingEggs.Count > 0)
                        {
                            breakingEggs[0].SetActive(true);
                            originEggs[0].SetActive(false);
                        }

                    });
                    break;
                case State.Cutting:
                    originEggs.RemoveAt(0);
                    breakingEggs.RemoveAt(0);
                    cuttingEggs[0].SetActive(true);
                    break;

                case State.Done:
                    vfxSmoke.Stop();
                    this.OnBack();
                    break;
            }
        }

        public override void OnDrop()
        {
            if (IsState(State.Cooked))
            {
                return;
            }
            base.OnDrop();
        }

        public override void OnBack()
        {
            if (IsState(State.Cooked))
            {
                return;
            }
            base.OnBack();
        }

        public void PreBreaking()
        {
            anim.Play(animMoveOut);
        }

        public void DoneCutting()
        {
            cuttingEggs.RemoveAt(0);
            if (cuttingEggs.Count != 0)
            {
                ChangeState(Egg.State.Breaking);
            }
            else
            {
                ChangeState(Egg.State.Done);
            }
        }

        public void OnBreaking()
        {
            if (IsState(State.Cutting))
            {
                return;
            }

            SoundControl.Ins.PlayFX(Fx.EggCracking);
            countingClickToFragile++;

            if (breakingEggs.Count > 0)
            {
                breakingEggs[0].transform.DOKill();
                breakingEggs[0].transform.DOScale(1.2f, .1f).OnComplete(() =>
                {
                    if (breakingEggs.Count > 0)
                    {
                        breakingEggs[0].transform.DOScale(1, 0.1f);
                    }

                });
            }

            if (countingClickToFragile == 3)
            {
                breakingEggs[0].GetComponent<EggFragile>().ChangeState(EggFragile.State.Fragile);
                ChangeState(Egg.State.Cutting);
            }
        }

        public override void OnClickDown()
        {
            base.OnClickDown();
            LevelControl.Ins.SetHint(hint);
            SoundControl.Ins.PlayFX(Fx.Click);
        }

        public override void OnClickTake()
        {
            base.OnClickTake();
            SoundControl.Ins.PlayFX(Fx.Take);
        }
    }

}
