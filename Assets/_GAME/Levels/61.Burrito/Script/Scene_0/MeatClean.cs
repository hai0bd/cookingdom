using Link;
using UnityEngine;

namespace HuyThanh.Cooking.Burrito
{


    public class MeatClean : ItemMovingBase
    {
        public enum State
        {
            Dirty, Cleaning, NeedClean, Done
        }

        [SerializeField] public State state;
        [SerializeField] GameObject watersParent;
        [SerializeField] Transform[] waters;

        [SerializeField] ItemAlpha dirtyAlpha;
        [SerializeField] ParticleSystem vfxSplashWater;
        [SerializeField] ParticleSystem vfxCleanWater;
        [SerializeField] ParticleSystem vfxBlink;

        [SerializeField] Animation anim;
        [SerializeField] string animClean;

        [SerializeField] int numberTapToClean;
        [SerializeField] TissueBox tissueBox;
        [SerializeField] HintText meatHintText;


        public override bool IsCanMove => IsState(State.Dirty);

        private float cleanRate = 0;
        private float currentDirtyAlpha = 1f;

        public override bool IsState<T>(T t)
        {
            return state == (State)(object)(t);
        }

        public override void ChangeState<T>(T t)
        {
            state = (State)(object)(t);

            switch (state)
            {
                case State.Cleaning:
                    break;
                case State.Dirty:
                    break;
                case State.NeedClean:
                    OnBack();
                    watersParent.SetActive(true);
                    break;
                case State.Done:
                    LevelControl.Ins.NextHint();
                    tissueBox.DeactiveCollider();
                    meatHintText.OnActiveHint();
                    vfxBlink.Play();
                    SoundControl.Ins.PlayFX(Fx.DoneSomething);
                    LevelControl.Ins.CheckStep(1f);
                    break;
            }
        }

        public void AddClean()
        {
            cleanRate += Time.deltaTime;
            if (cleanRate >= 1)
            {
                ChangeState(State.Done);
            }
            for (int i = 0; i < waters.Length; i++)
            {
                waters[i].localScale = Vector3.one * 0.5f * (1 - cleanRate);
            }
        }

        public override void OnClickDown()
        {

            if (IsState(State.Cleaning))
            {
                SoundControl.Ins.PlayFX(Fx.WaterSplash);
                vfxCleanWater.transform.position = TF.position;
                vfxCleanWater.Play();

                anim.Play(animClean);

                currentDirtyAlpha -= 1f / numberTapToClean;
                dirtyAlpha.DoAlpha(currentDirtyAlpha, 0.1f);

                if (currentDirtyAlpha < 0f)
                {
                    vfxCleanWater.Stop();
                    vfxBlink.transform.position = TF.position;
                    vfxBlink.Play();
                    ChangeState(State.NeedClean);
                }
                return;
            }
            SoundControl.Ins.PlayFX(Fx.Click);
            anim.Play(animClean);
            base.OnClickDown();
        }
    }
}