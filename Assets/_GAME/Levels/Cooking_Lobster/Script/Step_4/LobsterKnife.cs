using DG.Tweening;
using MoreMountains.NiceVibrations;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

namespace Link.Cooking.Lobster
{
    public class LobsterKnife : ItemMovingBase
    {
        [SerializeField] Animator anim;
        [SerializeField] Lobster lobster;
        [SerializeField] LobsterPiece piece1, piece2;
        [SerializeField] ParticleSystem hitVFX;
        [SerializeField] ActionBase actionAnim;
        [SerializeField] Power power;

        public enum State { Normal, Done }
        [SerializeField] State state;
        public override bool IsCanMove => IsState(State.Normal);

        public override void OnClickDown()
        {
            base.OnClickDown();
            SoundControl.Ins.PlayFX(LevelStep_1.Fx.Click);
        }

        public override void ChangeState<T>(T t)
        {
            state = (State)(object)t;

            switch (state)
            {
                case State.Normal:
                    break;
                case State.Done:
                    power.OnInit(Hit, Slash);
                    break;
                default:
                    break;
            }
        }
        public override bool IsState<T>(T t)
        {
            return state == (State)(object)t;
        }

        [Button]
        public void Hit()
        {
            hitVFX.Play();
            anim.ResetTrigger("hit");
            anim.SetTrigger("hit");
            SoundControl.Ins.PlayFX(LevelStep_1.Fx.Slash);
            MMVibrationManager.Haptic(HapticTypes.LightImpact); // Vibrate
        }

        [Button]
        public void Slash()
        {
            anim.ResetTrigger("hit");
            anim.SetTrigger("slash");
            DOVirtual.DelayedCall(0.2f, () =>
            {
                lobster.OnDone();
                piece1.gameObject.SetActive(true);
                piece1.TF.DOMoveX(piece1.TF.position.x - 0.07f, 0.1f).OnComplete(() => piece1.ChangeState(LobsterPiece.State.Normal));
                piece2.gameObject.SetActive(true);
                piece2.TF.DOMoveX(piece2.TF.position.x + 0.07f, 0.1f).OnComplete(() => piece2.ChangeState(LobsterPiece.State.Normal));
                DOVirtual.DelayedCall(1f, () => { actionAnim.Active(); OrderLayer = 20; });

                SoundControl.Ins.PlayFX(LevelStep_1.Fx.PowerSlash);
                MMVibrationManager.Haptic(HapticTypes.MediumImpact); // Vibrate
                LevelControl.Ins.SetStep(LevelName.Lobster, Step.LobsterCut);
            });
        }

        public override void OnDrop()
        {
            base.OnDrop();
            OnBack();
        }

    }
}