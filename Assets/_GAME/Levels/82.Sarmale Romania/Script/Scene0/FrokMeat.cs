using Link;
using Sirenix.OdinInspector;
using UnityEngine;
namespace HuyThanh.Cooking.SarmaleRomania
{
    public class FrokMeat : ItemMovingBase
    {
        public enum State
        {
            Normal,
            Cutting,
            Piecing,
            MovingPlate,

            Done
        }
        public override bool IsCanMove => IsState(State.Normal, State.MovingPlate);

        [SerializeField] State state;
        [SerializeField] Knife knife;
        [SerializeField] ChoppingCleaver choppingcleaver;

        [SerializeField] Animation anim;
        [SerializeField] string animBeefOnClick;

        [BoxGroup("Cutting")][SerializeField] Transform maskSlide, maskSpice, startPoint, finishPoint;
        [BoxGroup("Pice")][SerializeField] Transform maskSlide2, maskSpice2, startPoint1, finishPoint1;
        [BoxGroup("ProkMeat")]
        [SerializeField] GameObject FrokMeatorigin, FrokMeatdone1, FrorkMeatdone2, Frokmeatpice;

        [SerializeField] ParticleSystem vfxjumpmeat;

        private float step = 0;

        private const int CHOP_STEP_cut = 4;
        private const int CHOP_STEP_Pice = 25;

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
                    OrderLayer = 0;
                    knife.OnMove(startPoint.position + new Vector3(0, -0.3f, 0), Quaternion.Euler(0, 0, 0), 0.2f);
                    break;
                case State.Piecing:
                    choppingcleaver.OnMove(startPoint1.position + new Vector3(0, -0.4f, 0), Quaternion.Euler(0, 0, 0), 0.2f);
                    choppingcleaver.ChangeState(ChoppingCleaver.State.Piceing);
                    break;




                case State.Done:
                    anim.Play(animBeefOnClick);
                    break;
            }
        }

        public override void OnClickDown()
        {
            if (IsState(State.Cutting))
            {
                OrderLayer = 0;

                knife.ChangeAnim("KnifeCut");
                //SoundControl.Ins.PlayFX(Fx.KnifeCutEgg);

                step += 1f / CHOP_STEP_cut;
                Vector2 point = Vector3.Lerp(startPoint.position, finishPoint.position, step);
                //splashVFX.transform.position = point;
                //  splashVFX.Play();
                maskSlide.position = point;
                maskSpice.position = point;
                knife.TF.position = point;

                if (step >= 1f)
                {
                    knife.ChangeState(Knife.State.Done);
                    step = 0; // reset counter step
                    ChangeState(State.Piecing);
                    FrokMeatorigin.SetActive(false);
                    FrokMeatdone1.SetActive(false);
                    FrorkMeatdone2.SetActive(true);
                    Frokmeatpice.SetActive(true);


                }
                return;
            }

            if (IsState(State.Piecing))
            {
                OrderLayer = 0;

                choppingcleaver.ChangeAnim("Choping_cut");
                //SoundControl.Ins.PlayFX(Fx.KnifeCutEgg);

                step += 1f / CHOP_STEP_Pice;
                Vector2 point = Vector3.Lerp(startPoint1.position + new Vector3(0, -0.3f, 0), finishPoint1.position + new Vector3(0, -0.3f, 0), step);
                //splashVFX.transform.position = point;
                //  splashVFX.Play();
                vfxjumpmeat.transform.position = point;
                vfxjumpmeat.Play();
                maskSlide2.position = point;
                maskSpice2.position = point;
                choppingcleaver.TF.position = point;

                anim.Play(animBeefOnClick);
                if (step >= 1f)
                {
                    step = 0; // reset counter step
                    ChangeState(State.MovingPlate);
                    choppingcleaver.ChangeState(ChoppingCleaver.State.Done);

                }
                return;
            }
            base.OnClickDown();
        }

        public override void OnDrop()
        {
            if (IsState(State.Done))
            {
                anim.Play(animBeefOnClick);
            }
            base.OnDrop();
        }

        public override void OnBack()
        {
            base.OnBack();
            anim.Play(animBeefOnClick);

        }
    }
}

