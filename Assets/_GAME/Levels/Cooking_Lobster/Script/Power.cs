using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link.Cooking
{
    public class Power : MonoBehaviour
    {
        private const string ANIM_HIT = "hit";
        private const string ANIM_DONE = "done";

        [SerializeField] Transform power;
        [SerializeField] float upSpeed, downSpeed;
        [SerializeField] Vector2 start, finish;
        [SerializeField] SpriteRenderer renderer;
        [SerializeField] Color startColor, finishColor;
        [SerializeField] Animator anim;
        [SerializeField] ParticleSystem hitVFX;

        private Action OnDoneAction, OnHitAction;
        float rate;
        bool active = false;

        public void OnInit(Action hitAction, Action doneAction)
        {
            OnDoneAction = doneAction;
            OnHitAction = hitAction;
            gameObject.SetActive(true);
            rate = 0;
            ChangePower(rate);
            Invoke(nameof(OnStart), 0.5f);
        }

        private void OnStart()
        {
            active = true;
        }

        private void OnDone()
        {
            OnDoneAction?.Invoke();
        }

        private void Update()
        {
            if (!active || !LevelControl.Ins.IsAllowInteract) return; 

            if (Input.GetMouseButtonDown(0))
            {
                rate += upSpeed;

                if (rate >= 1)
                {
                    active = false;
                    anim.SetTrigger(ANIM_DONE);
                    Invoke(nameof(OnDone), 0.5f);
                }
                else
                {
                    OnHitAction?.Invoke();
                    anim.SetTrigger(ANIM_HIT);
                    if (hitVFX != null) hitVFX.Play();
                }
            }
            else
            {
                rate -= Time.deltaTime * downSpeed;
            }

            rate = Mathf.Clamp(rate, 0, 1);
            ChangePower(rate);
        }

        public void ChangePower(float rate)
        {
            power.localPosition = Vector3.Lerp(start, finish, rate);
            renderer.color = Color.Lerp(startColor, finishColor, rate);
        }
    }
}