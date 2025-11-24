using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Link;
namespace LawNguyen.CookingGame.LRisottoY
{
    public class Clamm : ItemMovingBase
    {

        public enum State
        {
            dirty,
            washdity,
            clambackwatersink,
            clamafterwash,
            clammoveinbroad,
            clamcleawaterdone,
            clammovinginplate

        }

        [SerializeField] Animation animation;
        [SerializeField] AnimationClip clamwash,claminbroad;
        [SerializeField] ParticleSystem sparkelow;
        [SerializeField] ParticleSystem vfxafterwet;
        [SerializeField] ParticleSystem Sflashwater;
        [SerializeField] Drop2D drop2D;
        [SerializeField] Transform[] waterclean;
        [SerializeField] Sprite HintClam;
        public override bool IsCanMove => !IsState(State.clammoveinbroad);

        public State state = State.dirty;
        float cleanrate = 0;


        public override void OnBack()
        {
            base.OnBack();
        }
        
        public override bool IsState<T>(T t)
        {
            return this.state==(State)(object)t;
        }

        public override void ChangeState<T>(T t)
        {
          this.state =(State)(object)t;

            switch (state) 
            {
                case State.dirty:

                    break;
            case State.washdity:
                    Sflashwater.Play();
                    ChangeAnim(clamwash.name);
                    SoundControl.Ins.PlayFX(SoundFXEnum.Soundname.blingbling);
                    sparkelow.Play();   

                    break;
            case State.clambackwatersink:
                    Sflashwater.Play();
                    vfxafterwet.Stop();
                    break;
             case State.clamafterwash:
                    vfxafterwet.Play();
                    break;
             case State.clammoveinbroad:

                    ChangeAnim(claminbroad.name);
                    vfxafterwet.Stop();
                    break;
             case State.clamcleawaterdone:
                    SoundControl.Ins.PlayFX(SoundFXEnum.Soundname.blingbling);
                    sparkelow.Play();
                 break; 

                case State.clammovinginplate:
                

              
                    break; 
                    default:
                        break;
            
            
            }
        }

        public override void OnClickDown()
        {
            base.OnClickDown();
            if (HintClam != null) LevelControl.Ins.SetHint(HintClam);
            if (IsState(State.washdity)|| IsState(State.clambackwatersink)) 
            { 
               ChangeState(State.clamafterwash);
            }

            if (IsState(State.clamcleawaterdone))
            {
                ChangeState(State.clammovinginplate);
            }


        }

        public void Dropactive()
        {
            drop2D.OnActive();
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
                ChangeState(State.clammovinginplate);

                sparkelow.Play();
            }
            for (int i = 0; i < waterclean.Length; i++)
            {

                waterclean[i].localScale = Vector3.one * (1f - cleanrate);


            }

        }










    }
}

