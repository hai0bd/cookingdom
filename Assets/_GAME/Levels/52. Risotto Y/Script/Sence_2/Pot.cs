using DG.Tweening;
using Link;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LawNguyen.CookingGame.LRisottoY
{
    public class Pot : ItemIdleBase
    {
        [SerializeField]
        public enum State
        {
            havenull, haverice, havegralic, havecheese, havebutter,
            havewater, tunnel, havewatersafron, havestock, done
        }

        private State state = State.havenull;

        [Header("Visuals")]
        [SerializeField] SpriteRenderer water;
        [SerializeField] GameObject onoff;
        [SerializeField] ParticleSystem waterboling;
        [SerializeField] ParticleSystem smoker;
        [SerializeField] SpriteRenderer potwater;
        [SerializeField] SpriteRenderer stockpour3;
        [SerializeField] SpriteRenderer rice3danau;
        [SerializeField] GameObject ricechuanau;

        [Header("Control")]
        [SerializeField] ClockTimer timer;
        [SerializeField] Potmoving potmoving;

        private bool stoveTapOn = false;
        private bool potlidclose = false;

        public void StoveTapON(bool Ison)
        {
            stoveTapOn = Ison;

            if (onoff != null)
                onoff.SetActive(Ison);

            if (!Ison)
            {
                waterboling?.Stop();
                SoundControl.Ins.StopFX(SoundFXEnum.Soundname.waterboil);
            }
            else
            {
                if (state == State.tunnel || state == State.havewater || state == State.havewatersafron || state == State.havestock)
                {
                    waterboling?.Play();
                    SoundControl.Ins.PlayFX(SoundFXEnum.Soundname.waterboil, true);
                }

                Checkcooking();
            }
        }

        public void Checkcooking()
        {
            if (!stoveTapOn || !potlidclose) return;

            Collider2D[] colliders = Physics2D.OverlapBoxAll(TF.position, new Vector2(2.2f, 2.2f), 0f);
            foreach (var item in colliders)
            {
                IItemMoving it = item.GetComponent<IItemMoving>();
                if (it == null) continue;

                if (it is Butter && IsState(State.havewater))
                    it.ChangeState(Butter.State.done);
                if (it is Chesseshre && IsState(State.havewater))
                    it.ChangeState(Chesseshre.State.done);
                if (it is Gralicmine && IsState(State.havewater))
                    it.ChangeState(Gralicmine.State.done);
            }
        }

        public override bool IsState<T>(T t)
        {
            return t is State s && state == s;
        }

        public override void ChangeState<T>(T t)
        {
            if (t is State s)
                state = s;
            else
                Debug.LogWarning("Pot: Invalid state change attempted.");
        }

        public override bool OnTake(IItemMoving item)
        {
            // PlateRice chứa Rice2
            if (item is PlateRice plateRice && state == State.havenull)
            {
               
                Rice2 childRice = plateRice.GetComponentInChildren<Rice2>();
                childRice.transform.SetParent(null);
                plateRice.OnBack();
                plateRice.ToggleBox(true);     // Bật lại collider/sprite
                      // ✅ Quay PlateRice về chỗ cũ

                if (childRice != null)
                {
                    // Tách Rice2 ra và cho vào Pot
                 
                    childRice.transform.localScale = Vector3.one;

                    childRice.ChangeState(Rice2.State.Cooking);
                    childRice.OnMove(TF.position + new Vector3(0f, 0f, 0f), Quaternion.identity, 0.2f);
                    DOVirtual.DelayedCall(0.2f, () => childRice.mark.transform.localPosition = new Vector3(0f, 0.08f, 0));
                    DOVirtual.DelayedCall(0.2f, () => childRice.OrderLayer = -9);

                    ChangeState(State.haverice);
                    return true;
                }
                else
                {
                    Debug.LogWarning("PlateRice không chứa Rice2.");
                }

                return true;
            }



            if (item is Rice2 rice2 && state == State.havenull)
            {
                rice2.ChangeState(Rice2.State.Cooking);
                //rice2.OnMove(TF.position + new Vector3(0, -0.15f, 0), Quaternion.identity, 0.2f);
                //DOVirtual.DelayedCall(0.2f, () => rice2.mark.transform.localPosition = new Vector3(0f, 0.08f, 0));
                //DOVirtual.DelayedCall(0.2f, () => item.OrderLayer = -9);
                ChangeState(State.haverice);
                return true;
            }

            if (item is Gralicmine && state == State.haverice)
            {
                item.OnMove(TF.position + new Vector3(0.4f, -0.1f, 0), Quaternion.identity, 0.5f);
                DOVirtual.DelayedCall(0.5f, () => item.OrderLayer = -4);
                item.ChangeState(Gralicmine.State.inpot);
                ChangeState(State.havegralic);
                return true;
            }

            if (item is Chesseshre && state == State.havegralic)
            {
                item.OnMove(TF.position + new Vector3(-0.4f, -0.1f, 0), Quaternion.identity, 0.5f);
                DOVirtual.DelayedCall(0.5f, () => item.OrderLayer = -5);
                item.ChangeState(Chesseshre.State.inpot);
                ChangeState(State.havecheese);
                return true;
            }

            if (item is Butter && state == State.havecheese)
            {
                item.OnMove(TF.position + new Vector3(0, 0.4f, 0), Quaternion.identity, 0.5f);
                DOVirtual.DelayedCall(0.5f, () => item.OrderLayer = -6);
                item.ChangeState(Butter.State.inpot);
                ChangeState(State.havebutter);
                return true;
            }

            if (item is Waterjar && state == State.havebutter)
            {
                item.OnMove(TF.position + new Vector3(1f, 1.5f, 0), Quaternion.identity, 0.2f);
                DOVirtual.DelayedCall(0.2f, () => item.OrderLayer = 4);

                item.ChangeState(Waterjar.State.during);

                DOVirtual.DelayedCall(0.5f, () =>
                {
                    water?.DOFade(1f, 1f);
                    CheckvfxOn();
                });

                DOVirtual.DelayedCall(0.8f, () => item.ChangeState(Waterjar.State.end));

                ChangeState(State.havewater);
                return true;
            }

            if (item is Potlid && state == State.havewater && stoveTapOn)
            {
                potlidclose = true;

                item.OnMove(TF.position + new Vector3(0.01f, 0.15f, 0), Quaternion.identity, 0.5f);
                DOVirtual.DelayedCall(0.5f, () => item.OrderLayer = 50);

                LevelControl.Ins?.BlockControl(4f);

                DOVirtual.DelayedCall(0.5f, () =>
                {
                    Checkcooking();
                    if (timer != null)
                    {
                        timer.gameObject.SetActive(true);
                        timer.Show(4f);
                    }
                });

                DOVirtual.DelayedCall(4f, () =>
                {
                    if (potwater != null)
                        potwater.color = Color.yellow;

                    ChangeState(State.tunnel);
                });

                return true;
            }

            if (item is Watersafron && state == State.tunnel)
            {
                item.OnMove(TF.position + new Vector3(1f, 1f, 0), Quaternion.identity, 0.2f);

                DOVirtual.DelayedCall(0.5f, () =>
                {
                    item.ChangeState(Watersafron.State.during);
                    water?.DOColor(new Color(1f, 0.5f, 0f), 1f);
                });

                DOVirtual.DelayedCall(1.2f, () =>
                {
                    item.OnBack();
                    item.OnDone();
                    ChangeState(State.havewatersafron);
                });

                return true;
            }

            if (item is Stock && state == State.havewatersafron)
            {
                item.OnMove(TF.position + new Vector3(1f, 1f, 0), Quaternion.identity, 0.2f);

                DOVirtual.DelayedCall(0.2f, () => item.ChangeState(Stock.State.during));
                DOVirtual.DelayedCall(0.5f, () => stockpour3?.DOFade(1f, 1f));

                DOVirtual.DelayedCall(1.2f, () =>
                {
                    item.OnBack();
                    item.OnDone();
                    ChangeState(State.havestock);
                });

                return true;
            }

            if (item is Potlid && state == State.havestock)
            {
                item.OnMove(TF.position + new Vector3(0.01f, 0.15f, 0), Quaternion.identity, 0.5f);
                DOVirtual.DelayedCall(0.5f, () => item.OrderLayer = 50);

                LevelControl.Ins?.BlockControl(4f);

                DOVirtual.DelayedCall(0.5f, () =>
                {
                    if (timer != null)
                    {
                        timer.gameObject.SetActive(true);
                        timer.Show(4f);
                    }
                });

                DOVirtual.DelayedCall(4f, () =>
                {
                    smoker?.Play();
                    waterboling?.Stop();
                    SoundControl.Ins.StopFX(SoundFXEnum.Soundname.waterboil);
                    if (water != null) water.gameObject.SetActive(false);
                    if (stockpour3 != null) stockpour3.gameObject.SetActive(false);
                    if (ricechuanau != null) ricechuanau.SetActive(false);
                    rice3danau?.DOFade(1f, 1f);

                    ChangeState(State.done);
                    if (potmoving != null)
                        potmoving.iscanmove = true;
                });

                return true;
            }

            return false;
        }

        void CheckvfxOn()
        {
            if (!stoveTapOn)
            {
                waterboling?.Stop();
                SoundControl.Ins.StopFX(SoundFXEnum.Soundname.waterboil);
            }
            else if (stoveTapOn && (state == State.tunnel || state == State.havewater))
            {
                waterboling?.Play();
                SoundControl.Ins.PlayFX(SoundFXEnum.Soundname.waterboil,true);
            }
        }
    }
}
