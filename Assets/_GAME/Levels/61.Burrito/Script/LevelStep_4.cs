using Link;
using UnityEngine;

namespace HuyThanh.Cooking.Burrito
{


    public class LevelStep_4 : LevelStepBase
    {

        [SerializeField] PlateDecor plateDecor;
        [SerializeField] BottleItem mayoBottle, chiliBottle;

        [SerializeField] ParticleSystem blinkVFX;

        [SerializeField] Sprite hint_next;

        [SerializeField] BurritoRaw burritoRaw;

        public override void NextHint()
        {
            if (burritoRaw.CheckDone() && (plateDecor.IsState(PlateDecor.State.Vegetable, PlateDecor.State.Tomato, PlateDecor.State.Lemon) == false))
                LevelControl.Ins.SetHint(hint_next);
        }

        public override bool IsDone()
        {
            if (plateDecor.IsState(PlateDecor.State.Done) == false)
                return false;
            if (mayoBottle.IsState(BottleItem.State.Done) == false)
                return false;
            if (chiliBottle.IsState(BottleItem.State.Done) == false)
                return false;

            CameraControl.Instance.OnSize(5f);
            CameraControl.Instance.OnMove(CameraControl.Instance.TF.position + Vector3.up);
            blinkVFX.Play();
            SoundControl.Ins.PlayFX(Fx.DoneSomething);
            return true;
        }
    }

}