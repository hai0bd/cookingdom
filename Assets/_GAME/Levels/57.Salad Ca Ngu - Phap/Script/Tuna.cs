using DG.Tweening;
using Link;
using Sirenix.OdinInspector;
using UnityEngine;
using VinhLB;

namespace HuyThanh.Cooking.TunaSaladFrench
{
    public class Tuna : ItemMovingBase
    {
        public enum State { Normal, OpenCap, RemoveCap, Spice, Pouring, Trash, Done }
        [SerializeField] State state;
        [SerializeField] TunaCap tunaCap;
        [SerializeField] GameObject itemActive;
        [BoxGroup("Anim Name")][SerializeField] Animation anim;
        [BoxGroup("Anim Name")][SerializeField] string animOpenCap, animRemoveCap, animPouring;

        [SerializeField] private ModifiedMeshFoldDraggable meshFold;


        public override bool IsCanMove => IsState(Tuna.State.Spice, Tuna.State.Trash);

        private void OnEnable()
        {
            meshFold.onDetached += UnPack;
        }

        private void OnDisable()
        {
            meshFold.onDetached -= UnPack;
        }

        public override void ChangeState<T>(T t)
        {
            state = (State)(object)t;

            switch (state)
            {
                case State.OpenCap:

                    anim.Play(animOpenCap);
                    DOVirtual.DelayedCall(0.5f, () => SoundControl.Ins.PlayFX(Fx.Click));
                    break;
                case State.RemoveCap:

                    anim.Play(animRemoveCap);
                    DOVirtual.DelayedCall(1f, () =>
                    {
                        SoundControl.Ins.PlayFX(Fx.Click);
                        tunaCap.ChangeState(TunaCap.State.DropCap);///chuyen state cho nap
                    });

                    DOVirtual.DelayedCall(1.2f, () =>
                    {
                        ChangeState(Tuna.State.Spice);
                    });

                    break;
                case State.Spice:
                    collider.enabled = true;
                    break;
                case State.Pouring:

                    anim.Play(animPouring);

                    DOVirtual.DelayedCall(.6f, () =>
                    {
                        itemActive.SetActive(true);
                        SoundControl.Ins.PlayFX(Fx.Click);
                    });
                    DOVirtual.DelayedCall(1.2f, () => ChangeState(Tuna.State.Trash));

                    break;
                case State.Trash:
                    OnBack();
                    break;
                case State.Done:
                    LevelControl.Ins.CheckStep(1f);
                    break;
                default:
                    break;
            }
        }

        public void UnPack(ModifiedMeshFoldDraggable draggable)
        {
            ChangeState(Tuna.State.Spice);
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

        public void OnClickIdle()
        {
            TF.DOKill();
            TF.DOScale(1.2f, 0.1f).OnComplete(() =>
            {
                TF.DOScale(1f, 0.1f);
            });
        }

        public override void OnDrop()
        {
            base.OnDrop();
            if (!IsState(Tuna.State.Done))
                OnBack();
        }

        public override bool IsState<T>(T t)
        {
            return state == (State)(object)t;
        }
    }
}

