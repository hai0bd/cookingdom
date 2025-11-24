using DG.Tweening;
using Link;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace LawNguyen.CookingGame.LRisottoY
{
    public class PlateRisito : ItemIdleBase
    {
        public enum State
        {
            needrice,
            needpasley,
            needsefood,
            needpassleycut,
            done
        }
        public State state=State.needrice;
        [Header("Ricedone")]
        [SerializeField] SpriteRenderer RisotioDoneSR;


        [Header("Slot hải sản")]
        [SerializeField] private Transform[] TFshrimp;
        [SerializeField] private Transform[] TFMussel;
        [SerializeField] private Transform[] TFClam;
        [Header("Pasley")]
        [SerializeField] private Drop2D dropPassley;
        [Header("PasleySkinle")]
        [SerializeField] ParsleySpinkle[] parsleySpinklesAR;
        [SerializeField] Drop2D[] drop2Ds;
        [Header("SparkElow")]
        [SerializeField] ParticleSystem Spankelow;
        private bool[] usedShrimpSlots;
        private bool[] usedMusselSlots;
        private bool[] usedClamSlots;
        int layerbasse= 5;
        int slotallcurent = 0;
        int numberpaslerkinle = 0;
        private void Awake()
        {
            usedShrimpSlots = new bool[TFshrimp.Length];
            usedMusselSlots = new bool[TFMussel.Length];
            usedClamSlots = new bool[TFClam.Length];
        }

        public override bool OnTake(IItemMoving item)
        {

            if(item is RiceSence4 rice && state == State.needrice)
            {
                item.OnMove(TF.position + new Vector3(1.5f,1.5f,0), Quaternion.identity, 0.2f);
                DOVirtual.DelayedCall(0.2f, () => rice.DropriceAim());
                DOVirtual.DelayedCall(1f, () => {
                    RisotioDoneSR.DOFade(1f, 0.2f);
                    item.OnBack();
                    ChangeState(State.needpasley);
                });
            }

            if (item is Seafoodend seafoodend && state==State.needsefood)
            {
                Transform slot = GetAvailableSlot(seafoodend.seefoodType, out int slotIndex);

                if (slot != null)
                {
                    seafoodend.MoveInPlate(slot);

                    MarkSlotUsed(seafoodend.seefoodType, slotIndex);

                    seafoodend.SetLayer(layerbasse++); // Ví dụ: set Sorting Layer để nằm trên đĩa
                    seafoodend.SetCollider(false); // Không cho tương tác lại
                    slotallcurent++;
                    int AlLSlotcount = TFClam.Length+TFMussel.Length+TFshrimp.Length;
                    if (slotallcurent >= AlLSlotcount)
                    {
                        ChangeState(State.needpassleycut);
                        ActiveboxParrsleySpinkle();
                    }       

                    return true;
                }
            }

            if (item is PasleySence4 pasleySence4 && state==State.needpasley)
            {
                pasleySence4.Dofadepassley();
                dropPassley.OnActive();
                DOVirtual.DelayedCall(0.5f, () => {
                    item.OnBack();
                    item.OnDone();
                });
              
                ChangeState(State.needsefood); 
            }
            if (item is ParsleySpinkle parsleySpinkle && state == State.needpassleycut)
            {
                parsleySpinkle.Deactivegameobject();

                if (numberpaslerkinle < drop2Ds.Length)
                {
                    drop2Ds[numberpaslerkinle].OnActive();
                    numberpaslerkinle++;
                }

                if (numberpaslerkinle >= 3)
                {
                    ChangeState(State.done);
                }
            }






            return false;
        }

        private Transform GetAvailableSlot(Seafoodend.SeefoodType type, out int index)
        {
            index = -1;

            switch (type)
            {
                case Seafoodend.SeefoodType.Shrimp:
                    for (int i = 0; i < usedShrimpSlots.Length; i++)
                    {
                        if (!usedShrimpSlots[i])
                        {
                            index = i;
                            return TFshrimp[i];
                        }
                    }
                    break;

                case Seafoodend.SeefoodType.Museel:
                    for (int i = 0; i < usedMusselSlots.Length; i++)
                    {
                        if (!usedMusselSlots[i])
                        {
                            index = i;
                            return TFMussel[i];
                        }
                    }
                    break;

                case Seafoodend.SeefoodType.Clam:
                    for (int i = 0; i < usedClamSlots.Length; i++)
                    {
                        if (!usedClamSlots[i])
                        {
                            index = i;
                            return TFClam[i];
                        }
                    }
                    break;
            }

            return null;
        }

        private void MarkSlotUsed(Seafoodend.SeefoodType type, int index)
        {
            if (index < 0) return;

            switch (type)
            {
                case Seafoodend.SeefoodType.Shrimp:
                    usedShrimpSlots[index] = true;
                    break;
                case Seafoodend.SeefoodType.Museel:
                    usedMusselSlots[index] = true;
                    break;
                case Seafoodend.SeefoodType.Clam:
                    usedClamSlots[index] = true;
                    break;
            }
        }
        private void ActiveboxParrsleySpinkle()
        {
            foreach (var item in parsleySpinklesAR)
            {
                item.Activebox();   
            }
        }
       
        

        public override void ChangeState<T>(T t)
        {
            this.state=(State)(object)t;
            switch (state)
            {
                case State.done:
                    Spankelow.Play();
                    CameraControl.Instance.OnSize(5f, 2f);
                    CameraControl.Instance.OnMove(transform.position, 2f);
                    break;
            }

        }


    }
}
