using DG.Tweening;
using Sirenix.OdinInspector;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link.Cooking.Lobster
{
    public class CapyCatchFish : MonoBehaviour
    {
        [SerializeField] private SkeletonAnimation skeletonAnimation;
        [SerializeField] private Animator anim;
        [SerializeField] private AudioClip sfxDive, sfxFish;
        [SerializeField] private Boat boat;
        [SerializeField] private GameObject[] fishes;
        [SerializeField] Collider2D coll2D;

        [Header("Combat")]
        [SerializeField] private Transform tagetCombat;
        [SerializeField] LobsterCombat lobster;
        [SerializeField] GameObject fishGO;
        [SerializeField] CombatControl combatControl;
        [SerializeField] CapyShadow shadowPrefab;
        [SerializeField] Sprite hint;
        [SerializeField] AudioClip attackClip_1, attackClip_2, finishClip;

        private MeshRenderer meshRenderer;
        private int fishIndex = -1;
        public bool IsHaveFish => fishIndex >= 0;

        private Transform tf;
        public Transform Tf => tf ? tf : tf = transform;
        float floatSpeed = 3f, diveDistance = 1f, originAngle = -36f, diveAngle = 70f;

        private Vector3 maxScreenBounds;

        bool isDive = false, isSwim = true;

        private void Start()
        {
            meshRenderer = skeletonAnimation.GetComponent<MeshRenderer>();
            maxScreenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.nearClipPlane));
        }
        private void Update()
        {
            if(!isSwim) return;

            if (Input.GetMouseButtonDown(0))
            {
                if (!LevelControl.Ins.IsAllowInteract) return;
                Dive();
            }
            if(Tf.localPosition.y < 0 && !isDive)
            {
                Tf.localPosition += Vector3.up * floatSpeed * Time.deltaTime;
            }    
            if(Tf.localPosition.y < -.5f)
            {
                meshRenderer.sortingOrder = 15;
            }
            else
            {
                meshRenderer.sortingOrder = 3;
                if (IsHaveFish && !isDive)
                {
                    fishes[fishIndex].SetActive(false);
                    boat.OnCatchFish(fishIndex);

                    fishIndex = -1;
                    coll2D.enabled = true;
                }
            }

            if(Tf.localPosition.y <= -5f)
            {
                if(IsHaveFish)
                {
                    lobster.Attack();
                }else
                {
                    Tf.DOKill();
                    OnMoveToCombat();
                }
            }
        }

        [Button]
        private void Dive()
        {
            Tf.DOKill();
            AudioManager.PlaySFX(sfxDive, .2f);
            isDive = true;
            Tf.DOLocalRotate(new Vector3(0,180f, diveAngle), .3f);
            Tf.DOLocalMoveY(Tf.localPosition.y - diveDistance, .5f).OnComplete(() =>
            {
                isDive = false;
                Tf.DORotate(new Vector3(0,180f, originAngle), .3f);
            }).OnUpdate(() =>
            {
                if (Tf.localPosition.y <= -7f) 
                { 
                    Tf.localPosition = new Vector3(0, -7f, 0); 
                }
            });
        }
        public void OnCatchFish(int index)
        {
            if (IsHaveFish) return;
            coll2D.enabled = false;

            AudioManager.PlaySFX(sfxFish);
            fishIndex = index;
            fishes[fishIndex].SetActive(true);
        }


        public void OnMoveToCombat()
        {
            LevelControl.Ins.SetHint(hint);

            fishGO.SetActive(false);
            coll2D.enabled = false;
            isSwim = false;

            Tf.DORotate(tagetCombat.eulerAngles, 0.5f);
            Tf.DOMove(tagetCombat.position, .5f).OnComplete(() =>
            {
                skeletonAnimation.gameObject.SetActive(false);
                anim.gameObject.SetActive(true);

                CameraControl.Instance.OnMove(new Vector3(0, -2.4f, -10), .5f);
                CameraControl.Instance.OnSize(CameraControl.Instance.orthographicSize - 1, .5f, 0);

                Invoke(nameof(OnReadyCombat), 1.5f);
            });

            lobster.OnInit();

            LevelControl.Ins.SetHintTextDone(1, 1);
        }

        private void OnReadyCombat()
        {
            combatControl.OnInit();
            startPoint = Tf.localPosition;
        }

        public void OnAttack()
        {
            //tan cong
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
            AudioManager.PlaySFX(attackClip_1);
        }

        public void OnAttack2()
        {
            //tan cong
            anim.SetTrigger("attack2");
            AudioManager.PlaySFX(attackClip_2);
        }

        public void OnDoneAttack()
        {
            //dien anim
            StartCoroutine(IEDone());
            LevelControl.Ins.SetHintTextDone(1, 2);
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
                Tf.localPosition = Vector3.Lerp(Tf.localPosition, startPoint + point, Time.deltaTime * 20f);
                yield return null;
            }
        }

        private IEnumerator IEDone()
        {
            tf.DOMoveX(4f, .5f);
            float time = 0.05f;
            float index = 1f/time;

            SoundControl.Ins.PlayFX(finishClip, 0.1f);

            while (time > 0)
            {
                time -= Time.deltaTime;
                if(time <= 0) {
                    time += 0.05f;
                    index--;
                    Instantiate(shadowPrefab, Tf.position, Tf.rotation).OnInit();
                    if(index <= 0)
                    {
                        break;
                    }
                }
                yield return null;
            }

            LevelControl.Ins.CheckStep();
        }

        

    }
}

