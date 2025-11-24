using DG.Tweening;
using Link;
using Link.Turkey;
using Satisgame;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace LawNguyen.CookingGame.LRisottoY
{
    public class WaterSink : ItemIdleBase
    {
        [SerializeField] GameObject waterTap;
        [SerializeField] SpriteRenderer waterbasin;
        Watercover watercover;
        [SerializeField] public bool waterTapOn = false;
        [SerializeField] Color colorClean;
        [SerializeField] Color colorMuseeldirty;
        [SerializeField] Color colorClamdirty;
        [SerializeField] Color currentColor;
        [Header("Dirtyobject")]
        [SerializeField] GameObject dirtymuseel;
        [SerializeField] GameObject dirtyClam;
        [SerializeField] GameObject dirtyRice;
        [SerializeField] GameObject Itemidle;
        [SerializeField] GameObject Itemoving;
        [SerializeField] Towel towel;
        [SerializeField] GameObject Bg_Ban;
        public event Action<UnityEngine.Object> OnCompletewatercoverEvent;

        public bool IsdoneSence1 =false;
        private bool ricedonewash=false;


        public bool watercoverclose => watercover != null && Vector2.Distance(watercover.TF.position, transform.position) < 0.35f;
        public bool Iswater = false;
        int maxcountwash = 4;
        int countwash = 0;
        bool candropitem = false;
        public bool waterdity = false;
        private void Awake()
        {
            ricedonewash = false;
        }
        public void OnOpenWater(bool Ison)
        {
            waterTapOn = Ison;
            waterTap.SetActive(Ison);

            CheckWash();
        }

        public void CheckWash()
        {
            if (waterTapOn && watercoverclose) Iswater = true;
            if (!watercoverclose) Iswater = false;

            if (Iswater)
            {
                waterbasin.DOFade(1f, 1f);

                Collider2D[] colliders = Physics2D.OverlapBoxAll(TF.position, new Vector2(2.2f, 1.8f), 0);
                IItemMoving it;

                foreach (var item in colliders)
                {
                    it = item.GetComponent<IItemMoving>();
                    if (it == null) continue;

                    if (it is Museel museel && it.IsState(Museel.State.dirty))
                    {
                        it.ChangeState(Museel.State.washdity);
                        if (candropitem == true)
                        {
                            museel.Dropactive();
                        }
                        DOVirtual.DelayedCall(1f, () => Changewater(waterbasin, colorMuseeldirty));
                        waterdity = true;
                    }

                    if (it is Museel && it.IsState(Museel.State.musselafterwash))
                    {
                        it.ChangeState(Museel.State.musselbackwatersink);
                    }

                    if (it is Clamm clam && it.IsState(Clamm.State.dirty))
                    {
                        it.ChangeState(Clamm.State.washdity);
                        if (candropitem == true)
                        {
                            clam.Dropactive();
                        }
                        DOVirtual.DelayedCall(1f, () => Changewater(waterbasin, colorClamdirty));
                        DOVirtual.DelayedCall(1f, () => waterdity = true);
                    }

                    if (it is Clamm && it.IsState(Clamm.State.clamafterwash))
                    {
                        it.ChangeState(Clamm.State.clambackwatersink);
                    }

                    if (it is Shrimp shrimp && it.IsState(Shrimp.State.dirty))
                    {
                        it.ChangeState(Shrimp.State.washdity);
                        if (candropitem == true)
                        {
                            shrimp.dropActive();
                        }
                        DOVirtual.DelayedCall(1f, () => Changewater(waterbasin, colorClamdirty));
                        DOVirtual.DelayedCall(1f, () => waterdity = true);
                    }

                    if (it is Shrimp && it.IsState(Shrimp.State.Shrimpafterwash))
                    {
                        it.ChangeState(Shrimp.State.Shrimpbackwatersink);
                    }
                    if (it is Rice rice && rice.IsState(Rice.State.dirty))
                    {



                        it.OnMove(TF.position, Quaternion.identity, 0.2f);
                        it.ChangeState(Rice.State.wash);
                        SetWatercoverBox(false);
                       
                    }
                }
            }
            else if (!watercoverclose)
            {   
               
                waterbasin.DOFade(0f, 0.4f).OnComplete(() =>
                {
                    if (countwash > 0)
                    {
                        waterbasin.color = colorClean;
                        countwash = 0;
                        waterdity = false;
                    }
                });
                candropitem = false;
            }
        }

        public void Changewater(SpriteRenderer watermuseel, Color colordirty)
        {
            countwash = Mathf.Clamp(countwash + 1, 0, maxcountwash);
            float t = (float)countwash / maxcountwash;
            currentColor = Color.Lerp(new Color(1, 1, 1, 1), colordirty, t);
            watermuseel.DOColor(currentColor, 1f);
        }

        void Onlosecover()
        {
            CheckWash();
            OnCompletewatercoverEvent?.Invoke(this);
            if (ricedonewash == true)
            {
                ClearFloatingGrains();
            }

        }

        public override bool OnTake(IItemMoving item)
        {
            if (item is Watercover)
            {
                watercover = item as Watercover;
                if (Vector2.Distance(item.TF.position, transform.position) < 0.35f)
                {
                    item.OnMove(TF.position + new Vector3(0, -0.2f, 0), Quaternion.identity, 0.2f);
                    item.OrderLayer = -10;
                    StartCoroutine(TempCheckWatercoverRemoved());
                    Onlosecover();
                }
                else
                {
                    Onlosecover();
                }
                return true;
            }

            if (item is Museel museel && museel.state != Museel.State.movingplate)
            {
                //if (waterdity && !museel.IsState(Museel.State.musselafterwash))
                //{
                //    item.OnBack();
                //    return false;
                //}
                SoundControl.Ins.PlayFX(SoundFXEnum.Soundname.dropInWater);    
                item.OnMove(TF.position, Quaternion.identity, 0.5f);

                if (Iswater)
                {
                    candropitem = true;
                    CheckWash();
                }

                item.OnSave(0.5f);
                return true;
            }
            else if (!Iswater)
            {
                candropitem = false;
                return true;
            }

            if (item is Clamm clam && clam.state != Clamm.State.clammovinginplate)
            {
                //if (waterdity && !clam.IsState(Clamm.State.clamafterwash))
                //{
                //    item.OnBack();
                //    return false;
                //}
                SoundControl.Ins.PlayFX(SoundFXEnum.Soundname.dropInWater);
                candropitem = true;
                item.OnMove(TF.position, Quaternion.identity, 0.5f);
                if (Iswater)
                {
                    CheckWash();
                }
                item.OnSave(0.5f);
                return true;
            }

            if (item is Shrimp shrimp)
            {
                //if (waterdity && !shrimp.IsState(Shrimp.State.Shrimpafterwash))
                //{
                //    item.OnBack();
                //    return false;
                //}
                SoundControl.Ins.PlayFX(SoundFXEnum.Soundname.dropInWater);
                candropitem = true;
                item.OnMove(TF.position, Quaternion.identity, 0.5f);
                if (Iswater)
                {
                    CheckWash();
                }
                item.OnSave(0.5f);
                return true;
            }
            else if (!Iswater)
            {
                candropitem = false;
            }

            if (item is Rice rice && rice.IsState(Rice.State.dirty))
            {


                SoundControl.Ins.PlayFX(SoundFXEnum.Soundname.dropInWater);
                // Nếu nước đã dơ thì không nhận Rice nữa
                if (waterdity)
                {
                    item.OnBack();
                    Debug.Log("[WaterSink] ⚠️ Nước đã dơ → Không nhận Rice");
                    return false;
                }

                item.OnMove(TF.position, Quaternion.identity, 0.2f);
                item.ChangeState(Rice.State.wash);
                SetWatercoverBox(false);
                return true;
            }


            if (item is Glasssafron glasssafon && item.IsState(Glasssafron.State.idle) && waterTapOn)
            {
                item.SetOrder(26);
                item.OnMove(TF.position + new Vector3(-1f, -0.5f, 0), Quaternion.identity, 1f);

                DOVirtual.DelayedCall(1f, () =>
                {
                    item.ChangeState(Glasssafron.State.havewater);
                    item.OnBack();
                });

                return true;
            }

            return false;
        }

        private void ClearFloatingGrains()
        {
            Collider2D[] colliders = Physics2D.OverlapBoxAll(TF.position, new Vector2(3f, 1.8f), 0);

            foreach (var col in colliders)
            {
                if (col.CompareTag("Grain"))
                {
                    GameObject grain = col.gameObject;

                    DOTween.Kill(grain.transform);
                    SpriteRenderer sr = grain.GetComponent<SpriteRenderer>();
                    if (sr != null)
                    {
                        DOTween.Kill(sr);
                        sr.DOFade(0f, 0.3f);
                    }

                    grain.transform.DOScale(Vector3.zero, 0.3f)
                        .SetEase(Ease.InBack)
                        .OnKill(() => Debug.Log("Tween của hạt đã bị kill hoặc kết thúc"))
                        .OnComplete(() =>
                        {
                            DOTween.Kill(grain.transform);
                            Destroy(grain);
                        });
                }
            }
        }

        private void SetWatercoverBox(bool active)
        {
            if (watercover != null)
            {
                var col = watercover.GetComponent<Collider2D>();
                if (col != null) col.enabled = active;
            }
        }

        public void OnRiceDone()
        {
            SetWatercoverBox(true);
            ricedonewash = true;    
         
        }

        private bool hasStartedCleaning = false;

        public void StartCleaning()
        {
            if (hasStartedCleaning)
            {
                Debug.Log("[WaterSink] 🔁 StartCleaning() đã được gọi rồi → bỏ qua");
                return;
            }

            hasStartedCleaning = true;
            Debug.Log("[WaterSink] 🧽 BẮT ĐẦU quá trình rửa bồn");

            towel.OnMove(TF.position + new Vector3(0, -2f, 0), Quaternion.identity, 2f);
            towel.OnSave(2f);
            Itemidle.SetActive(false);
            Itemoving.SetActive(false);
            watercover.OnDone();
            this.OnDone();

            CameraControl.Instance.OnMove(transform.position + new Vector3(0, 0, -10), 2f, 0f);
            CameraControl.Instance.OnSize(4f, 2f, 0f);

            dirtymuseel.SetActive(true);
            dirtyClam.SetActive(true);
            dirtyRice.SetActive(true);
        }

        public void EndCleaning()
        {
            if (IsdoneSence1)
            {
                Debug.Log("[WaterSink] 🚫 EndCleaning() đã được gọi trước đó → bỏ qua");
                return;
            }

            Debug.Log("[WaterSink] ✅ EndCleaning() → Set IsdoneSence1 = true");
            IsdoneSence1 = true;
            CameraControl.Instance.OnMove(Bg_Ban.transform.position + new Vector3(-0.2f, 0, -10), 1f, 0f);

            CameraControl.Instance.OnSize(6.25f, 1f, 0f);
            DOVirtual.DelayedCall(0.5f, () =>
            {
                Debug.Log("[WaterSink] 🟢 Gọi CheckStep sau khi kết thúc rửa bồn");
               LevelControl.Ins.CheckStep();
            });
        }
        private IEnumerator TempCheckWatercoverRemoved()
        {
            float checkTime = 0.5f;
            float elapsed = 0f;

            while (elapsed < checkTime)
            {
                yield return null;
                elapsed += Time.deltaTime;

                // Nếu người chơi nhấc ra nhanh
                if (!watercoverclose && Iswater)
                {
                    // Nước mất
                    Iswater = false;
                    waterbasin.DOFade(0f, 0.4f).OnComplete(() =>
                    {
                        if (countwash > 0)
                        {
                            waterbasin.color = colorClean;
                            countwash = 0;
                            waterdity = false;
                        }
                    });
                    candropitem = false;
                    break;
                }
            }
        }



        }
}
