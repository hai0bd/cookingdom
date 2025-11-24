using DG.Tweening;
using Link;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
namespace HuyThanh.Cooking.SarmaleRomania
{
    public class SmokerRibs : ItemMovingBase
    {
        public enum State
        {
            Normal,
            Cutting,
            donecut,
            MovingPlate,
            Done
        }
        public override bool IsCanMove => IsState(State.Normal) || IsState(State.MovingPlate);

        [SerializeField] State state;
        [SerializeField] ChoppingCleaver choppingcleaver;
        [SerializeField] List<GameObject> listsmokerdone = new List<GameObject>();
        [SerializeField] Animation anim;
        [SerializeField] string animBonce;
        [BoxGroup("Cutting")][SerializeField] Transform maskSlide, startPoint, finishPoint;

        private float step = 0;
        private int countSmoker = 0;
        private const int CHOP_STEP_cut = 6;
        private Vector2 offset;
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
                    choppingcleaver.OnMove(startPoint.position + new Vector3(0, -0.4f, 0), Quaternion.Euler(0, 0, 0), 0.2f);

                    break;


                case State.Done:
                    anim.Play(animBonce);
                    break;
            }
        }
        public override void OnClickDown()
        {
            if (IsState(State.Cutting))
            {
                OrderLayer = 0;

                choppingcleaver.ChangeAnim("Choping_cut");
                //SoundControl.Ins.PlayFX(Fx.KnifeCutEgg);


                step += 1f / CHOP_STEP_cut;
                Vector2 point = Vector3.Lerp(startPoint.position, finishPoint.position, step);

                maskSlide.position = point;
                choppingcleaver.TF.position = point;
                if (countSmoker < listsmokerdone.Count)
                {
                    countSmoker++;
                    Vector3 chosenOffset = Vector3.zero;

                    // Chọn offset dựa trên index
                    if (countSmoker >= 1 && countSmoker <= 3)
                    {
                        offset = new Vector2(-0.3f, 0); // offset cho 0–2
                    }
                    else if (countSmoker >= 4 && countSmoker <= 6)
                    {
                        offset = new Vector2(-0.85f, 0.1f); // offset cho 3–4
                    }



                    listsmokerdone[countSmoker - 1]
                        .transform.DOMove(point + offset, 0.2f);


                }



                if (step >= 1f)
                {
                    choppingcleaver.ChangeState(ChoppingCleaver.State.Done);
                    step = 0;
                    ChangeState(State.MovingPlate);

                    // Đảm bảo chạy xong move cuối cùng rồi mới dịch tất cả
                    listsmokerdone[listsmokerdone.Count - 1]
                        .transform.DOMove(point + offset, 0.2f)
                        .OnComplete(() =>
                        {
                            foreach (var smoder in listsmokerdone)
                            {
                                smoder.transform.DOMove(
                                    smoder.transform.position + new Vector3(0.5f, 0, 0),
                                    0.2f
                                );
                            }
                        });
                }

                return;
            }
            base.OnClickDown();
        }

    }

}

