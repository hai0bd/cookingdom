using DG.Tweening;
using Link;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace HuyThanh.Cooking.Burrito
{

    public class CornCutItem : ItemMovingBase
    {
        public enum State
        {
            Normal,
            Cutting,
            Trashing,
            Spice,
            Done
        }

        public override bool IsCanMove => IsState(State.Normal, State.Spice);

        [SerializeField] State state;
        [SerializeField] Knife knife;
        [SerializeField] ParticleSystem splashVFX;
        [SerializeField] ParticleSystem chopVFX;

        [SerializeField] int CUT_STEP = 6;
        [SerializeField] List<CornSeed> cornSeeds;
        [SerializeField] CornCob cornCob;

        [BoxGroup("Cutting")][SerializeField] Transform startPoint, finishPoint;

        [SerializeField] Animation anim;
        [SerializeField] string animSpice;
        [SerializeField] HintText hintText;


        private float step = 0;
        private int currentSeedIndex = 0;


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
                    this.OnSavePoint();
                    knife.OnMove(startPoint.position, Quaternion.identity, 0.2f);
                    break;
                case State.Spice:
                    anim.Play(animSpice);
                    break;
                case State.Done:
                    SoundControl.Ins.PlayFX(Fx.DoneSomething);
                    hintText.OnActiveHint();
                    LevelControl.Ins.NextHint();
                    break;
            }
        }

        public override void OnClickDown()
        {
            if (IsState(State.Cutting))
            {
                OrderLayer = 1;

                knife.ChangeAnim("KnifeCornCutting");
                SoundControl.Ins.PlayFX(Fx.KnifeCut);

                step += 1f / CUT_STEP;
                Vector2 point = Vector3.Lerp(startPoint.position, finishPoint.position, step);
                knife.TF.position = point;
                knife.OrderLayer = 48;

                cornSeeds[currentSeedIndex].DoFinishMove();
                currentSeedIndex++;

                if (currentSeedIndex >= cornSeeds.Count)
                {
                    step = 0;
                    ChangeState(State.Trashing);
                    knife.ChangeState(Knife.State.Done);
                    cornCob.ActiveCollider();
                }
                return;
            }
            SoundControl.Ins.PlayFX(Fx.Click);
            base.OnClickDown();
        }

        public void OnDoneCornThrow()
        {
            anim.Play(animSpice);
            DOVirtual.DelayedCall(0.5f, () => ChangeState(State.Spice));
        }

        public override void OnDrop()
        {
            base.OnDrop();
            LevelControl.Ins.NextHint();
        }
    }

}