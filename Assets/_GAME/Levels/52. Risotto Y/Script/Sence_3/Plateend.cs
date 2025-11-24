using UnityEngine;
using Link;
using System.Collections.Generic;
using Satisgame;

namespace LawNguyen.CookingGame.LRisottoY
{
    public class Plateend : ItemIdleBase
    {
        public enum PlateType { Shrimp, Museel, Clam };
        public enum State { needseefood,needpasley,Ondone}
        public State state=State.needseefood;
        [SerializeField] public PlateType plateType;

        private int currentIndex = 0;
        [SerializeField] private List<Transform> seafoodSlots;
        [SerializeField] Drop2D pasley;
        [SerializeField] CircleCollider2D circleCollider;
        [SerializeField] ParticleSystem spankelow;
        public override bool IsDone => state==State.Ondone;

        public Transform GetNextSlot(out int index)
        {
            if (seafoodSlots == null || currentIndex >= seafoodSlots.Count)
            {
                index = -1;
                return null;
            }
            if (currentIndex == 2) { circleCollider.isTrigger = false; ChangeState(State.needpasley); };

            index = currentIndex;

            return seafoodSlots[currentIndex++];
           

        }

        public override bool OnTake(IItemMoving item)
        {
            if(item is Pasley3 pasley3 && state==State.needpasley)
            {
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                pasley3.Droppassley();
                pasley.OnActive();
                pasley3.ONmovePLate();
                ChangeState(State.Ondone);
                spankelow.Play();
                LevelControl.Ins.CheckStep();

                return true;    
            }


            return false;
        }


        public override void ChangeState<T>(T t)
        {
            this.state =(State)(object)t;
      
        }





    }
}
