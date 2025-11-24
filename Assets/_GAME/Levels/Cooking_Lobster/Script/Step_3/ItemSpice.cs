using DG.Tweening;
using Link.Cooking.Spageti;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Link.Cooking.Lobster
{
    public class ItemSpice : ItemMovingBase
    {
        public enum State { Origin, Cutting, Cut, Spice, Done }
        [SerializeField] State state;
        public override bool IsCanMove => IsState(State.Origin, State.Cut, State.Spice);

        [field: SerializeField] public DecoreItem.NameType ItemName;

        [SerializeField] GameObject origin, cut;
        [SerializeField] AnimaBase2D animaBase;
        [SerializeField] Transform[] items;
        [SerializeField] SpriteRenderer[] sprites;
        [SerializeField] AnimaBase2D animBurst;

        Ellipse2D[] elips;
        Color color;

        [SerializeField] List<OliuFail> oliuFails;

         public override void ChangeState<T>(T t)
        {
            state = (State)(object)t;

            switch (state)
            {
                case State.Origin:
                    break;
                case State.Cutting:
                    if(ItemName == DecoreItem.NameType.Cucumber)
                    {
                        TF.DOScale(1.4f, 0.5f);
                        cut.SetActive(true);
                        origin.transform.DOLocalMoveY(1f, 0.2f);
                        foreach (var item in oliuFails)
                        {
                            item.OnInit();
                        }
                    }else
                    {
                        DOVirtual.DelayedCall(1.2f, () =>
                        {
                            ChangeState(State.Cut);
                        });
                    }
                    break;
                case State.Cut:
                    origin.SetActive(false);
                    cut.SetActive(true);
                    if(animBurst != null) animBurst.OnActive();
                    break;
                case State.Done:
                    //TODO: Anim
                    break;
                default:
                    break;
            }
        }

        public override bool OnTake(IItemMoving item)
        {
            if (item is Knife && IsState(State.Origin) && LevelControl.Ins.IsHaveObject<CuttingBoard>(TF.position) != null && ItemName != DecoreItem.NameType.Cucumber)
            {
                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                item.ChangeState(Knife.State.Cutting);
                ChangeState(State.Cutting);
                return true;
            }
            return base.OnTake(item);
        }

        public void OnActive(int sorting)
        {
            cut.SetActive(false);
            collider.enabled = false;
            animaBase.gameObject.SetActive(true);
            OrderLayer = sorting;
            SetOrder(sorting);

            animaBase.doneEvent.AddListener(() => {
                elips = new Ellipse2D[items.Length];
                for (int i = 0; i < items.Length; i++)
                {
                    elips[i] = new Ellipse2D(.8f, 0.7f, items[i].localPosition);
                }
            });

            animaBase.OnActive();
        }

        public void SetTime(float time)
        {
            for (int i = 0; i < items.Length; i++)
            {
                items[i].localPosition = elips[i].Evaluate(time);
            }
        }

        public void SetScale(float scale)
        {
            for (int i = 0; i < items.Length; i++)
            {
                items[i].localScale = Vector3.one * scale;
            }
        }

        public void SetAlpha(float alpha)
        {
            foreach (var item in sprites)
            {
                color = item.color;
                color.a = Mathf.Lerp(color.a, 0, alpha * 0.5f);
                item.color = color; 
            }
        }

        public override void OnDone()
        {
            gameObject.SetActive(false);
            base.OnDone();
        }

        public override void OnDrop()
        {
            base.OnDrop();
            OnBack();
        }

        public override bool IsState<T>(T t)
        {
            return state == (State)(object)t;
        }
        public override void OnClickTake()
        {
            base.OnClickTake();
            SoundControl.Ins.PlayFX(LevelStep_1.Fx.Take);
        }

        public override void OnClickDown()
        {
            base.OnClickDown();
            SoundControl.Ins.PlayFX(LevelStep_1.Fx.Click);

        }

        private void OnDrawGizmos()
        {
            if (elips == null) return;
            for (int i = 0; i < elips.Length; i++)
            {
                elips[i].OnDrawGizmos(TF.position);
            }
        }

        public void OnDoneLeaf(OliuFail oliuFail)
        {
            oliuFails.Remove(oliuFail);
            if (oliuFails.Count == 0)
            {
                ChangeState(State.Cut);
            }
        }
    }
}