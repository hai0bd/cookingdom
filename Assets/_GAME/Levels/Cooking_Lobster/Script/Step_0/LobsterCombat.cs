using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Spine.Unity;
using UnityEngine;

namespace Link.Cooking.Lobster
{
    public class LobsterCombat : MonoBehaviour
    {
        [SerializeField] private Animator anim;
        [SerializeField] private Animation animHit;
        [SerializeField] private CustomColorSkeleton customColor;
        [SerializeField] AudioClip attackClip;
        Transform tf;
        public Transform TF => tf = tf != null ? tf : tf = transform;

        void Start()
        {
            customColor.SetColor();
            startPoint = TF.localPosition;
        }

        public void Attack()
        {
            anim.SetTrigger("attack");
            if(time <= 0)
            {
                time = .5f;
                StartCoroutine(IEAttack());
            }
            else
            {
                time = .5f;
            }

            SoundControl.Ins.PlayFX(attackClip);
        }

        public void Stun()
        {
            TF.DOMoveX(1.2f, 0.5f);
            anim.ResetTrigger("attack");
            anim.SetTrigger("stun");
        }

        public void OnHit()
        {
            animHit.Play();
        }

        public void OnInit()
        {
            customColor.DoColor(Color.white, 0.5f);
            TF.DOMoveX(tf.position.x - 0.5f, 0.5f);
        }

        [SerializeField] AnimationCurve atackCurve;
        float time = 0;
        Vector3 startPoint;
        
        private IEnumerator IEAttack()
        {
            while (time > 0)
            {
                time -= Time.deltaTime;
                Vector3 point = atackCurve.Evaluate(time) * Vector3.right ;
                TF.localPosition = Vector3.Lerp(TF.localPosition, startPoint + point, Time.deltaTime * 20f);
                yield return null;
            }
        }
    }
}