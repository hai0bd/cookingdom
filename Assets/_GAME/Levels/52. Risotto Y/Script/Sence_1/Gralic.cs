using DG.Tweening;
using Link;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace LawNguyen.CookingGame.LRisottoY
{
    [RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
    public class Gralic : ItemMovingBase
    {
        public const float CUT_STEP = 10;

        public enum State { idle, inbroad, Cuting, Peel, minece, Sliced, Pieced, done }
        public State state;

        public override bool IsCanMove => IsState(State.idle) || IsState(State.done) || IsState(State.Pieced);

        int clickCount = 0;
        int clickThreshold = 5;
        [SerializeField] Animation animation;
        [SerializeField] AnimationClip Cuting, peel;
        [SerializeField] ParticleSystem vfxgraliccut;
        [SerializeField] GameObject idle, slice, piece;
        [SerializeField] string cutAnimName = "KniftMince";
        [SerializeField] Transform maskSlide, maskPiece, startPoint, finishPoint;
        [SerializeField] Sprite HintGralic;
        private float step;
        private Knift knife;
        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        //private void Update()
        //{
        //    if (Input.GetMouseButtonDown(0) && IsState(State.Cuting))
        //    {
        //        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //        Collider2D hit = Physics2D.OverlapCircle(mousePos, 0.2f); // thay thế

        //        if (hit != null && hit.transform == transform)
        //        {
                   
        //            OnCutClick();
        //        }
        //    }
        //}
        public override bool IsState<T>(T t)
        {
            return this.state == (State)(object)t;
        }

        public override bool OnTake(IItemMoving item)
        {
            if (!(item is Knift knift)) return false;

            this.knife = knift;

            if (IsState(State.inbroad) || IsState(State.Peel))
            {
                if (IsState(State.inbroad))
                {
                    item.OnMove(TF.position, Quaternion.identity, 0.2f);
                    item.ChangeState(Knift.State.CutGralic);

                    DOVirtual.DelayedCall(1.8f, () =>
                    {
                        item.OnBack();
                        ChangeState(State.Cuting);
                    });
                }
                else if (IsState(State.Peel))
                {
                    item.OnMove(TF.position + new Vector3(0.3f, -0.2f, 0), Quaternion.identity, 0.2f);
                    ChangeState(State.minece);
                }

                return true;
            }

            return false;
        }

        public override void ChangeState<T>(T t)
        {
            this.state = (State)(object)t;

            switch (state)
            {
                case State.Cuting:
                    ChangeAnim(Cuting.name);
                    spriteRenderer.sortingOrder = 20;
                    break;

                case State.Peel:
                    ChangeAnim(peel.name);
                    spriteRenderer.sortingOrder = 15;
                    break;

                case State.minece:
                    StartCoroutine(IECut());
                    break;

                case State.done:
                    OnDone();
                    break;
            }
        }

        private void ChangeAnim(string name)
        {
            animation.Play(name);
        }

        private IEnumerator IECut()
        {
            idle.SetActive(false);
            slice.SetActive(true);
            piece.SetActive(true);
            maskSlide.position = startPoint.position;
            maskPiece.position = startPoint.position;
            yield return new WaitForSeconds(0.3f);

            ChangeState(State.Sliced);
        }

        public void OnCutClick()
        {
           // if (!IsState(State.Cuting)) return;

            clickCount++;
            transform.DOShakeScale(0.2f, 0.3f, 10, 90f, false);
            SoundControl.Ins.PlayFX(SoundFXEnum.Soundname.graliccracking);
            if (clickCount >= clickThreshold)
            {
                transform.DOScale(Vector3.one, 0.2f);
                ChangeState(State.Peel);
            }
        }

        public override void OnClickDown()
        {
            if (IsCanMove) base.OnClickDown();
            if (HintGralic != null) LevelControl.Ins.SetHint(HintGralic);

            if (IsState(State.Cuting))
            {
                OnCutClick();
            }
            if (IsState(State.Sliced))
            {
                step += 1 / CUT_STEP;
                Vector2 point = Vector3.Lerp(startPoint.position, finishPoint.position, step);
                maskSlide.position = point;
                maskPiece.position = point;
                knife.TF.position = point + new Vector2(0.5f, 0);
                vfxgraliccut.transform.position =point;
                vfxgraliccut.Play();
                SoundControl.Ins.PlayFX(SoundFXEnum.Soundname.kniftCUt1);
                knife.ChangeAnim(cutAnimName);

                if (step >= 1f)
                {
                    slice.SetActive(false);
                    ChangeState(State.Pieced);
                    knife.ChangeState(Knift.State.moving);

                    DOVirtual.DelayedCall(0.15f, () => knife.OnBack());
                }
            }
        }
    }
}
