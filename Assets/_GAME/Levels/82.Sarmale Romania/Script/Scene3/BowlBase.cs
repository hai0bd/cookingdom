using Link;
using UnityEngine;

namespace HuyThanh.Cooking.SarmaleRomania
{


    public class BowlBase : ItemIdleBase
    {
        [SerializeField] Transform itemTF;
        [SerializeField] Animation anim;
        [SerializeField] CircleCollider2D circleCollider2D;
        [SerializeField] string animGetItem;

        [SerializeField] float[] percentScale;

        private Vector3 itemScale;
        private float radius;
        private int takeTime = 0;
        private int numberOfTake = 6;

        private void Start()
        {
            itemScale = itemTF.localScale;
            radius = circleCollider2D.radius;
        }

        public void PlayTakeAnim()
        {
            anim.Play(animGetItem);
        }

        public void TakeSpice()
        {
            anim.Play(animGetItem);
            itemTF.localScale -= itemScale * percentScale[takeTime];
            circleCollider2D.radius -= radius * percentScale[takeTime];
            takeTime++;
        }

        public bool IsCanTake()
        {
            return takeTime < numberOfTake;
        }
    }
}