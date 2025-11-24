using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Link;
using DG.Tweening;
namespace LawNguyen.CookingGame.LRisottoY
{
    public class Museel : ItemMovingBase
    {
        public enum State { dirty , washdity,musselbackwatersink,musselafterwash, museelmoveinBroad ,museelcleanwaterdone, movingplate  }
        [SerializeField] Drop2D drop2D; 
        [SerializeField] ParticleSystem vfxafterwet;
        [SerializeField] ParticleSystem sparkelow;
        [SerializeField] ParticleSystem sflashwater;
       public State state = State.dirty;
       [SerializeField]  Animation animation;
        [SerializeField] AnimationClip washmuseel, musselmoveinbrad;
        [SerializeField] Transform[] waterclean;
        [SerializeField] Sprite Hintmuseel;   
        float cleanrate = 0;

        public override bool IsCanMove => !IsState(State.museelmoveinBroad);


        public override void OnBack()
        {
            base.OnBack();
        }
        public override void OnDrop()
        {
            base.OnDrop();
        }

        public override bool IsState<T>(T t)
        {
          return  this.state == (State)(object)t;
              
        }

        public override void ChangeState<T>(T t)
        {
            this.state = (State)(object)t;

            switch (state)
            { case State.dirty :    


                    return; 
               case State.washdity :


                    sflashwater.Play();
                    ChangeAnim(washmuseel.name);
                    DOVirtual.DelayedCall(1f, () =>
                    {
                        SoundControl.Ins.PlayFX(SoundFXEnum.Soundname.blingbling);
                        sparkelow.Play();
                    });


                    return;
               case State.musselbackwatersink :

                    sflashwater.Play();
                    vfxafterwet.Stop();  
                    return;
                case State.musselafterwash :
                  vfxafterwet.Play();   
                    return;
                case State.museelmoveinBroad:
                    sparkelow.Stop();   
                    vfxafterwet.Stop();
                   ChangeAnim(musselmoveinbrad.name);
                    return;

               case State.museelcleanwaterdone :
                    SoundControl.Ins.PlayFX(SoundFXEnum.Soundname.blingbling);
                    sparkelow.Play();
                    return;
                case State.movingplate :    

                    return;


                    default :
                    return; 
            }
        }

        public override void OnClickDown()
        {
            base.OnClickDown();
            if (Hintmuseel != null) LevelControl.Ins.SetHint(Hintmuseel);
            {
                
            }
            if (IsState(State.musselbackwatersink) || IsState(State.washdity))
            {
                ChangeState(State.musselafterwash);

            }
            if (IsState(State.museelcleanwaterdone))
            {
                ChangeState(State.movingplate);
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
          
            if(cleanrate > 1)
            {
                ChangeState(State.museelcleanwaterdone);

                sparkelow.Play();
            }
            for (int i = 0; i < waterclean.Length; i++) 
            {

                waterclean[i].localScale =  Vector3.one*(1f - cleanrate);    


            }

        }


    }
}

