using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Link;
using DG.Tweening;
namespace LawNguyen.CookingGame.LRisottoY
{
    public class Onion : ItemMovingBase
    {
        public const float CUT_STEP = 10;
        public enum State { idle, inbroad, Cuting, minece, Sliced, Pieced }
        public State state;
        public override bool IsCanMove => IsState(State.idle) || IsState(State.Pieced);
   
        [SerializeField] Animation animation;
        [SerializeField] ParticleSystem fruitCutVFX;
        [SerializeField] GameObject   idle, slice, piece;
        [SerializeField] string cutAnimName = "KniftMince";
        [SerializeField] Transform maskSlide, maskPiece, startPoint, finishPoint;
        [SerializeField] Sprite HintOnion;
        private float step;
        Knift knife;

        public override bool IsState<T>(T t)
        {
            return this.state == (State)(object)t;
        }
        public override bool OnTake(IItemMoving item)
        {
            if (!(item is Knift knift)) return false; // đảm bảo là Knift

            this.knife = knift;
            if (IsState(State.inbroad))
            {
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                item.ChangeState(Knift.State.CutOnion);
                SoundControl.Ins.PlayFX(SoundFXEnum.Soundname.beardCuting1);
                DOVirtual.DelayedCall(2.3f, () =>
                {
                    idle.SetActive(false);
                    slice.SetActive(true);
                    item.OnBack();
                    ChangeState(State.Cuting);
                }); 

                return true;
            }else if(IsState(State.Cuting))
            {   

                ChangeState(State.minece);
                return true;
            }   
           




            return false;
        }



        public override void ChangeState<T>(T t)
        {
            this.state = (State)(object)t;

            switch (state)
            {
                case State.idle:
                    break;
                case State.inbroad:
                    break;
                case State.minece:
                    StartCoroutine(IECut());
                    break;
                case State.Cuting:
                   
                    break;
                    case State.Sliced:
                    break;   
               
                default:
                    break;
            }

        }


        private IEnumerator IECut()
        {
            //  yield return new WaitForSeconds(0.2f);
            //  knife.ChangeAnim(cutAnimName);
            //  yield return new WaitForSeconds(0.2f);

            //  yield return new WaitForSeconds(0.7f);
            idle.SetActive(false);
            slice.SetActive(true);
            piece.SetActive(true);
            maskSlide.position = startPoint.position;
            maskPiece.position = startPoint.position;
            yield return new WaitForSeconds(0.3f);
             knife.OnMove(startPoint.position + new Vector3(0.5f,0,0), Quaternion.identity, 0.2f);
            ChangeState(State.Sliced);
        }

        public override void OnClickDown()
        {
          
            if (IsCanMove) base.OnClickDown();
            if (HintOnion != null) LevelControl.Ins.SetHint(HintOnion);
            if (IsState(State.Sliced))
            {
                step += 1 / CUT_STEP;
                Vector2 point = Vector3.Lerp(startPoint.position, finishPoint.position, step);
                maskSlide.position = point;
                maskPiece.position = point;
                knife.TF.position = point + new Vector2(0.5f, -0.2f);
                fruitCutVFX.transform.position = point;
                  fruitCutVFX.Play();
                SoundControl.Ins.PlayFX(SoundFXEnum.Soundname.kniftCUt1);
                knife.ChangeAnim(cutAnimName);

                if (step >= 1f)
                {

                    slice.SetActive(false);
                    ChangeState(State.Pieced);
                    knife.ChangeState(Knift.State.moving);
                    DOVirtual.DelayedCall(0.15f, () =>
                   // knife.OnMove(TF.position + new Vector3(1.5f, 1.5f, 0), Quaternion.identity, 0.2f));
                    knife.OnBack());
                }
            }
            else
            {

            }
        }










    }
}
