using DG.Tweening;
using Link;
using UnityEngine;

namespace HuyThanh.Cooking.Burrito
{
    public class PanBurrito : ItemMovingBase
    {
        public enum State
        {
            Normal,
            HeatOn,
            Cooked,
            Done
        }

        [SerializeField] State state;
        [SerializeField] Crust crustTarget;
        [SerializeField] PanBurritoButton panButton;
        [SerializeField] ItemAlpha heatAlpha;
        [SerializeField] Transform burritoParent;

        private float maxTemp = 100f;
        private float currentHeatTemp = 0f;

        private bool isDrop = true;

        private Vector3 lastPos;

        private Crust currentCrust;

        public bool IsDrop => isDrop;

        private void OnEnable()
        {
            isCanControl = false;
        }

        public override bool IsState<T>(T t)
        {
            return state == (State)(object)t;
        }

        public override void ChangeState<T>(T t)
        {
            state = (State)(object)t;

            switch (state)
            {
                case State.Normal:
                    break;
                case State.HeatOn:
                    break;
                case State.Cooked:

                    break;
                case State.Done:
                    TF.DOKill();
                    LevelControl.Ins.CheckStep(1f);
                    break;
            }
        }

        public bool IsDoneCrust(Crust crust)
        {
            return crust == crustTarget;
        }

        public bool CanUseButton()
        {
            return IsState(State.Normal) || IsState(State.Cooked);
        }
        private void LateUpdate()
        {
            if (LevelControl.Ins.IsHaveObject<PanBurritoButton>(TF.position) && panButton.IsOn)
            {
                currentHeatTemp += Time.deltaTime * 30f;
            }
            else
            {
                currentHeatTemp -= Time.deltaTime * 15f;
            }
            FixHeatAlpha();
            CrustCook();
        }

        private void FixHeatAlpha()
        {
            currentHeatTemp = Mathf.Clamp(currentHeatTemp, 0f, 100f);
            heatAlpha.SetAlpha(Mathf.Clamp(currentHeatTemp / maxTemp, 0f, 1f));
        }

        private void CrustCook()
        {
            if (currentHeatTemp >= maxTemp - 0.5f && currentCrust != null)
            {
                currentCrust.AddHeat(Time.deltaTime);
            }

            Vector3 velocity = (TF.position - lastPos) / Time.deltaTime;

            if (currentCrust != null && currentCrust.IsCanFlip)
            {
                if (velocity.y > 30f && isCanControl == true)
                {
                    currentCrust.DoFlip();
                    SetControl(false);
                }
            }

            lastPos = TF.position;
        }

        public void OnButtonClick(bool isOn)
        {
            if (isOn && IsState(State.Normal))
            {
                ChangeState(State.HeatOn);
                return;
            }

            if (!isOn && IsState(State.Cooked))
            {
                SoundControl.Ins.StopFX(Fx.SpoonFry);
                ChangeState(State.Done);
                return;
            }
        }

        public override bool OnTake(IItemMoving item)
        {
            if (item is Crust crust && IsState(State.HeatOn) && item.IsState(Crust.State.DoneMix) && (currentCrust == null || currentCrust.IsState(Crust.State.Done)))
            {
                currentCrust = crust;
                crust.TF.SetParent(burritoParent);
                crust.TF.DOLocalMove(Vector3.zero, 0.2f);
                crust.ChangeState(Crust.State.OnPan);
                return true;
            }
            return base.OnTake(item);
        }

        public override void OnClickDown()
        {
            base.OnClickDown();
            isDrop = false;
        }

        public override void OnDrop()
        {
            base.OnDrop();
            isDrop = true;
        }

        public void DoShake()
        {
            TF.DOShakePosition(5, 0.005f, 10, 10, fadeOut: false).SetLoops(-1);
        }
    }
}
