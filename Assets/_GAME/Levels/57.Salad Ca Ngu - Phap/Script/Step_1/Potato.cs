using DG.Tweening;
using Link;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace HuyThanh.Cooking.TunaSaladFrench
{
    public class Potato : ItemMovingBase
    {
        public const float CUT_STEP = 10;
        public enum State
        {
            Normal,
            Grater,
            DoneGrater,
            Cutting,
            Raw,
            Cooking,
            Spice,
            Done
        }

        [SerializeField] State state;
        [BoxGroup("Anim Name")][SerializeField] Animation anim;
        [BoxGroup("Anim Name")][SerializeField] string animPotatoMove, animeDoneGraterMove;

        [BoxGroup("Grater")][SerializeField] private List<GameObject> graterPotato;
        [BoxGroup("Grater")][SerializeField] private List<GameObject> originPotato;
        [BoxGroup("Grater")][SerializeField] private ParticleSystem vfxSplash;

        [BoxGroup("Cutting")][SerializeField] private GameObject cutPotato, doneCutPotato, spicePotato;
        [BoxGroup("Cutting")][SerializeField] private Knife knife;
        [BoxGroup("Cutting")][SerializeField] GameObject slice;
        [BoxGroup("Cutting")][SerializeField] Transform maskSlide, maskPiece, startPoint, finishPoint;
        [SerializeField] bool blockCutting = false;
        [SerializeField] Animation animScale;

        [BoxGroup("Cooked")][SerializeField] private string animCooking, animCutting;
        [SerializeField] private ParticleSystem vfxSmoke;
        [SerializeField] private ParticleSystem vfxChop;

        [SerializeField] Sprite hint;
        public override bool IsCanMove => IsState(Potato.State.Normal, Potato.State.Raw);


        private float step;


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
                case State.Grater:
                    originPotato[0].SetActive(false);
                    graterPotato[0].SetActive(true);
                    break;
                case State.DoneGrater:
                    cutPotato.SetActive(true);
                    break;
                case State.Cutting:
                    anim.Play(animCutting);
                    blockCutting = true;
                    DOVirtual.DelayedCall(0.5f, () => blockCutting = false);
                    break;
                case State.Raw:
                    spicePotato.SetActive(true);
                    break;
                case State.Cooking:
                    OrderLayer = 0;
                    DOVirtual.DelayedCall(4f, () => ChangeState(Potato.State.Spice));
                    anim.Play(animCooking);
                    break;
                case State.Spice:
                    vfxSmoke.Play();
                    vfxSplash.Play();
                    break;
                case State.Done:
                    ChangeAnim("CookingItemScale");
                    break;
            }
        }

        public override void OnClickDown()
        {
            if (IsState(State.Cutting))
            {
                if (blockCutting == true)
                {
                    return;
                }
                OrderLayer = 1;
                step += 1 / CUT_STEP;
                Vector2 point = Vector3.Lerp(startPoint.position, finishPoint.position, step);
                maskSlide.position = point;
                maskPiece.position = point;
                knife.TF.position = point;
                vfxChop.transform.position = point;
                vfxChop.Play();

                knife.ChangeAnim("cut");
                SoundControl.Ins.PlayFX(Fx.KnifeCutEgg);
                if (step >= 1f)
                {
                    animScale.Play();
                    slice.SetActive(false);
                    ChangeState(State.Raw);
                    knife.ChangeState(Knife.State.Ready);
                    knife.OnDrop();
                    //DOVirtual.DelayedCall(0.15f, () =>
                    //knife.OnMove(TF.position + new Vector3(1.5f, 1.5f, 0), Quaternion.identity, 0.2f));
                }
                return;
            }
            LevelControl.Ins.SetHint(hint);
            SoundControl.Ins.PlayFX(Fx.Click);
            base.OnClickDown();
        }

        public void PotatoGraterOnDone()
        {
            vfxSplash.Play();
            GameObject potato = graterPotato[0];
            DOVirtual.DelayedCall(1f, () => potato.SetActive(false));
            graterPotato.RemoveAt(0);
            originPotato.RemoveAt(0);
            DOVirtual.DelayedCall(1f, () =>
            {
                if (originPotato.Count > 0)
                    originPotato[0].SetActive(false);
            });
            if (originPotato.Count == 0)
            {
                DOVirtual.DelayedCall(1f, () => ChangeState(Potato.State.DoneGrater));
                return;
            }

            DOVirtual.DelayedCall(1f, () => graterPotato[0].SetActive(true));
        }


        public void ChangeAnim(string animName)
        {
            if (anim == null)
            {
                return;
            }
            anim.Play(animName);
        }
    }
}