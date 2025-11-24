using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Link.Cooking
{
    public class ItemFryFlip : MonoBehaviour
    {
        public enum State
        {
            Raw,
            Ripe,
            Overripe,
        }

        private bool isCanFlip = true, active = false;
        [SerializeField] private SpriteRenderer rawSpr, ripeSpr, overripeSpr;
        [SerializeField] Transform square;
        [SerializeField] Vector3 direct;
        [SerializeField] Shake2D shake2D;
        [SerializeField] private ParticleSystem smokeVFX, darkSmokeVFX;
        [SerializeField] private float targetUp = 3.5f, targetDown = 3.5f, overripe = 7f, flipTime = 0.2f;
        [SerializeField] private float ripeUp, ripeDown;
        [SerializeField] GameObject cautionRipe;
        public UnityEvent<ItemFryFlip> OnFlipEvent;
        private bool isUp = true;

        [Button]
        public void OnActive()
        {
            active = true;
        }

        [Button]
        public void OnDespawn()
        {
            active = false;
            shake2D.OnStop();
            cautionRipe.SetActive(false);
        }

        [Button]
        public void Flip()
        {
            //tranh lap flip lien tuc
            if (!isCanFlip || !active) return;
            isCanFlip = false;
            Invoke(nameof(UnFlip), 0.25f);

            isUp = !isUp;

            Vector3 angle = square.localEulerAngles + direct;
            //flip
            square.DOLocalRotate(angle, flipTime).OnComplete(() =>
            {
                OnFlipEvent?.Invoke(this);
            });

            if (isUp)
            {
                ChangeState(ripeUp, targetUp, overripe);
            }
            else
            {
                ChangeState(ripeDown, targetDown, overripe);
            }
        }

        public void UnFlip()
        {
            isCanFlip = true;
        }

        void Update()
        {
            if (active)
            {
                if (isUp)
                {
                    ripeDown += Time.deltaTime;
                    //chia state rung
                    //chia smoke
                    ChangeShake(ripeDown, targetDown, overripe);
                }
                else
                {
                    ripeUp += Time.deltaTime;
                    //chia state rung
                    //chia smoke
                    ChangeShake(ripeUp, targetUp, overripe);
                }
            }
        }

        public bool IsState(State state)
        {
            return GetState() == state;
        }

        private State GetState()
        {
            if (ripeUp >= overripe || ripeDown >= overripe) return State.Overripe;
            if (ripeUp >= targetUp && ripeDown >= targetDown) return State.Ripe;
            return State.Raw;
        }

        public void ChangeState(float rate, float target, float overTarget)
        {
            if (rate <= target)
            {
                Color color = Color.white;
                color.a = 1 - rate / target;
                rawSpr.color = color;

                color = ripeSpr.color;
                color.a = rate / target;
                ripeSpr.color = color;

                overripeSpr.color = Color.clear;
            }
            else
            if (rate > target && rate <= overTarget)
            {
                rawSpr.color = Color.clear;
                ripeSpr.color = Color.white;
                overripeSpr.color = Color.clear;
            }
            else
            {
                rawSpr.color = Color.clear;
                ripeSpr.color = Color.clear;
                overripeSpr.color = Color.white;
            }
        }

        private void ChangeShake(float rate, float target, float overTarget)
        {
            if (rate < 1)
            {
                shake2D.OnStop();
                smokeVFX.gameObject.SetActive(false);
                darkSmokeVFX.gameObject.SetActive(false);
                cautionRipe.SetActive(false);
            }
            else
            if (rate > 1 && rate <= target)
            {
                shake2D.OnActive(SkakeType.Fry);
                smokeVFX.gameObject.SetActive(true);
                darkSmokeVFX.gameObject.SetActive(false);
                cautionRipe.SetActive(false);
            }
            else
            if (rate > target && rate <= overTarget)
            {
                shake2D.OnActive(SkakeType.Riped);
                smokeVFX.gameObject.SetActive(true);
                darkSmokeVFX.gameObject.SetActive(false);
                cautionRipe.SetActive(true);
            }
            else
            {
                shake2D.OnActive(SkakeType.Overripe);
                smokeVFX.gameObject.SetActive(false);
                darkSmokeVFX.gameObject.SetActive(true);
                cautionRipe.SetActive(false);
            }
        }
    }
}