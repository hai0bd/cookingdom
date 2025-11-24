using DG.Tweening;
using Link;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEditor.Experimental;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

namespace LawNguyen.CookingGame.LRisottoY
{
    public class Pot3 : ItemIdleBase
    {
        public enum State
        {
            needoil,
            needbutter,
            needgralic,
            needseefood,
            needLid,
            needspcie
        }

        [SerializeField] public State state = State.needoil;

        [Header("Visual Effects")]
        [SerializeField] private ParticleSystem oilboiling;
        [SerializeField] private SpriteRenderer oil;
        [SerializeField] private Transform PointTileEffect;
        [Header("Listpoint_seefoodmove")]
        [SerializeField] private List<Transform> ListpointShrimp = new List<Transform>();
        [SerializeField] private List<Transform> ListpointMuseel = new List<Transform>();
        [SerializeField] private List<Transform> ListpointClam = new List<Transform>();
        [SerializeField] private List<Transform> Listallitemseefood = new List<Transform>();
        [Header("DropSaltandPeper")]
        [SerializeField] Drop2D dropsalt;
        [SerializeField] Drop2D dropPeper;
        [Header("ActionMOveplate")]
        [SerializeField] ActionMove[] ActionmoveAR;
        public bool potIsOn = false;
        public bool hasOil = false ;
        int hasSpice = 0;
        private bool isProcessing = false;
        public GameObject Gralicparent ;

        

      

      
        public void PotIsOn(bool isOn)
        {
            potIsOn = isOn;
            CheckOilBoilingPlay();

        
           
        }

        

        private void CheckOilBoilingPlay()
        {
            if (oilboiling == null) return;

            if ( potIsOn && hasOil)
                oilboiling.Play();
            else
                oilboiling.Stop();
        }

        public void ShowOil()
        {
            if (oil == null) return;
            DOTween.Kill(oil.transform);
            oil.DOFade(1f, 0.5f);
            oil.transform.DOScale(new Vector3(0.55f,0.75f,0.51f), 1.2f);
            hasOil = true;
            CheckOilBoilingPlay();
        }

        public void HideOil()
        {
            if (oil == null) return;
            DOTween.Kill(oil.transform);
            oil.DOFade(0f, 0.5f).SetEase(Ease.Linear).SetLink(oil.gameObject);
            oil.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).SetLink(oil.gameObject);
            hasOil=false;
            CheckOilBoilingPlay();  
        }

        public override bool IsState<T>(T t)
        {
            return state == (State)(object)t;
        }

        public override bool OnTake(IItemMoving item)
        {
            if (isProcessing) return false;

            // Nhận dầu
            if (item is Oil oilItem && IsState(State.needoil)&& potIsOn==true)
            {
                isProcessing = true;
                hasOil = true;

                item.OnMove(TF.position + new Vector3(1f, 1f, 0), Quaternion.identity, 0.2f);
                oilItem.ChangeState(Oil.State.during);

                DOVirtual.DelayedCall(0.5f, () => ShowOil());

                DOVirtual.DelayedCall(1f, () =>
                {

                    ChangeState(State.needbutter);

                    item.OnBack();
                    item.OnDone();  
                    isProcessing = false;
               
                });

                return true;
            }

            // Nhận bơ
            if (item is PlateButter platebutter && IsState(State.needbutter))
            {
                item.SetOrder(-5);
                item.OnMove(TF.position + new Vector3(1f, 1f, 0), Quaternion.identity, 0.5f);

                DOVirtual.DelayedCall(0.5f, () =>
                {
                    platebutter.MoveButterInPot(transform.position, Gralicparent.transform);
                }).SetLink(gameObject).SetUpdate(true);

                DOVirtual.DelayedCall(1f, () =>
                {
                    item.OnBack();
                    item.OnDone();  
                    ChangeState(State.needgralic);
                }).SetLink(gameObject).SetUpdate(true);

                return true;
            }

            // Nhận tỏi
            if (item is Plategralic plategralic && IsState(State.needgralic))
            {
                isProcessing = true;

                item.OnMove(TF.position + new Vector3(-1f, 1f, 0), Quaternion.identity, 0.2f);
                plategralic.ScatterGralicToPot(transform,Gralicparent.transform);

                DOVirtual.DelayedCall(1.2f, () =>
                {
                    plategralic.OnBack();
                    item.OnDone();
                    ChangeState(State.needseefood);
                    isProcessing = false;
                }).SetLink(gameObject).SetUpdate(true);

                return true;
            }

            // Nhận hải sản
            if (item is PlateSeefood plate && IsState(State.needseefood) )
            {
                isProcessing = true;
                plate.OnMove(TF.position+ new Vector3(-1f,1f,0),Quaternion.identity,0.3f);
                DOVirtual.DelayedCall(0.3f, () =>
                {
                    plate.MoveAndDropToPot(ListpointShrimp,ListpointMuseel,ListpointClam,Listallitemseefood);
                   
                }).SetLink(gameObject).SetUpdate(true);
                DOVirtual.DelayedCall(1f, () =>
                {
                    item.OnBack();
                    item.OnDone();
                    if (Listallitemseefood.Count >= 9) 
                    {
                        ChangeState(State.needspcie);
                    };
                    isProcessing = false;
                }).SetLink(gameObject).SetUpdate(true);



                return true;
            }
            if(item is spicecontaine spice && IsState(State.needspcie))
            {   
                isProcessing=true;
                item.OnMove(TF.position + new Vector3(1f,1f,0), Quaternion.identity, 0.5f);
                DOVirtual.DelayedCall(0.4f, () => {

                    spice.Pour();
                   
                });
                DOVirtual.DelayedCall(0.9f, () => {

                  
                    if (spice.typeSpice == spicecontaine.TypeSpice.salt)
                    {
                        dropsalt.OnActive();
                    }
                    if (spice.typeSpice == spicecontaine.TypeSpice.peper)
                    {
                        dropPeper.OnActive();
                    }

                });



                DOVirtual.DelayedCall(1.3f, () => {

                    item.OnBack();
                    item.OnDone();  
                    hasSpice++;
                    isProcessing = false;
                    if (hasSpice >= 2)
                    {
                        ChangeState(State.needLid);
                    }
                    
                });


                return true;    
            }

            if(item is Lid lid && IsState(State.needLid))
            {   isProcessing=true;  
                item.OnMove(TF.position, Quaternion.identity, 1f);
                DOVirtual.DelayedCall(1.1f, () =>
                {
                    lid.TimerCooking();
                    lid.iscanmove = false;   
                    foreach (var itemsea in Listallitemseefood)
                    {
                        itemsea.gameObject.GetComponent<SeafoodStirEffect>().DoneCooking();

                    }
                    HideOil();
                    hideGralicparent();
                    Hidedrop2d();
                }
                );
               
              

                DOVirtual.DelayedCall(6f, () => {
                    MOveplateENdSence();
                    isProcessing = false;
                    lid.iscanmove = true;

                    item.OnBack();
                     item.OnDone();  
                   
                });
                return true;
            }
                    




            return false;
        }

         void hideGralicparent()
        {
            Gralicparent.SetActive(false);
        }
        void Hidedrop2d()
        {
            dropPeper.gameObject.SetActive(false);
            dropsalt.gameObject.SetActive(false);
        }
        private void MOveplateENdSence()
        {
          foreach(var item in ActionmoveAR)
            {
                item.Active();
            }   

        }


        public override void ChangeState<T>(T t)
        {
            state = (State)(object)t;

            Debug.Log($"Pot state changed: {state}");
        }
    }
}
