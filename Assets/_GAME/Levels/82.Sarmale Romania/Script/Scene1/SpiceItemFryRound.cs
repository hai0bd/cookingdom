using Sirenix.OdinInspector;
using UnityEngine;

namespace HuyThanh.Cooking.SarmaleRomania
{
    public class SpiceItemFryRound : MonoBehaviour
    {
        [SerializeField] Animation anim;
        [SerializeField] SpriteRenderer sprite;
        [SerializeField] Transform itemTF;
        [SerializeField] float rotateSpeed = 5f;

        private float rate;
        private Color color = Color.white;
        [Button]
        public void SetRipeRate(float rate)
        {
            if (!anim.isPlaying)
                anim.Play();
            this.rate = rate;
            color.a = 1 - rate;
            sprite.color = color;

            itemTF.eulerAngles += Vector3.forward * rotateSpeed;
        }

        public void SetDone()
        {
            anim.Stop();
        }

        [Button]
        public void Setup()
        {
            anim = GetComponent<Animation>();
            sprite = transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>();
        }
    }
}