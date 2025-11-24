using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Link;
using DG.Tweening;
namespace LawNguyen.CookingGame.LRisottoY
{
    public class Shrimp : ItemMovingBase
    {
        public enum State
        {
            dirty,
            washdity,
            Shrimpbackwatersink,
            Shrimpafterwash,
            Shrimpmoveinbroad,
            Shrimpcleawaterdone,
            Shrimpcutvfx,

            Shrimpmovinginplate
        }
        [SerializeField]  Drop2D drop2D;
        [SerializeField] ParticleSystem sparkelow,vfxaffterwet , sflashvfx ,sflashwater;
        [SerializeField] Animation animation;
        [SerializeField] AnimationClip  Shrimpmoveinbrad;
        [SerializeField] Transform[] waterclean;
     
        [SerializeField] GameObject ShrimpdoneCut ,Shrimpwash;
        [SerializeField] Sprite hintshrimp; 
        float cleanrate = 0;
        public State state;
        public override bool IsCanMove => !IsState(State.Shrimpmoveinbroad) && !IsState(State.Shrimpcleawaterdone);


        public override bool IsState<T>(T t)
        {
            return this.state==(State)(object)t ;
        }

        public void dropActive()
        {

            drop2D.OnActive();  
        }

        public override bool OnTake(IItemMoving item)
        {
            if (item is Knift && IsState(State.Shrimpcleawaterdone))
            {
                item.ChangeState(Knift.State.CutShrimp);
                ChangeState(State.Shrimpcutvfx);
                item.OnMove(TF.position + new Vector3(0, -0.8f, 0), Quaternion.identity, 0.2f);
                DOVirtual.DelayedCall(1f, () => {
                  
                    item.OnBack();
                    });
           
                return true;    
            }

            return false;
        }


        public override void OnClickDown()
        {
            base.OnClickDown();
            if(hintshrimp != null) LevelControl.Ins.SetHint(hintshrimp); 
            if (IsState(State.Shrimpbackwatersink) || IsState(State.washdity))
            {
                ChangeState(State.Shrimpafterwash);

            }
            if (IsState(State.Shrimpcutvfx))
            {
                ChangeState(State.Shrimpmovinginplate);
            }


        }

        public override void ChangeState<T>(T t)
        {
            this.state = (State)(object)t;

            switch (state)
            {
                case State.dirty:
                    break;
                case State.washdity:
                    sflashwater.Play();
                    DOVirtual.DelayedCall(1f, () => { sparkelow.Play();

                        SoundControl.Ins.PlayFX(SoundFXEnum.Soundname.blingbling);
                    });
               

                    break;
                case State.Shrimpafterwash:
                    vfxaffterwet.Play();
                    break;
                 case State.Shrimpbackwatersink:
                    sflashwater.Play();
                    vfxaffterwet.Stop();    
                     break;
                case State.Shrimpmoveinbroad:
                    ChangeAnim(Shrimpmoveinbrad.name);
                    vfxaffterwet.Stop();    
                    sparkelow.Stop();

                    break;
                case State.Shrimpcleawaterdone:
                    SoundControl.Ins.PlayFX(SoundFXEnum.Soundname.blingbling);
                    sparkelow.Play();
                    break;
                case State.Shrimpcutvfx:
                    sflashvfx.Play();
                    Shrimpwash.SetActive(false); 
                    ShrimpdoneCut.SetActive(true);
                    break;
                case State.Shrimpmovinginplate:
                    break;

                default:
                    break;
            }
        }
        public void ChangeAnim(string name)
        {
            animation.Play(name);

        }

        public void AddClean()
        {
            cleanrate += Time.deltaTime;
           
            if (cleanrate > 1)
            {
                ChangeState(State.Shrimpcleawaterdone);

                sparkelow.Play();
            }
            for (int i = 0; i < waterclean.Length; i++)
            {

                waterclean[i].localScale = Vector3.one * (1f - cleanrate);


            }

        }






    }
}
