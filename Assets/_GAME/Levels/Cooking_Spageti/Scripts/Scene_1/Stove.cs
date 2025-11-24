using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Link.Cooking
{
    public class Stove : ItemIdleBase
    {
        [SerializeField] private GameObject fireGO, buttonGO;
        public Action<bool> OnStoveFire;
        public override bool IsDone => !buttonGO.activeSelf;

        [SerializeField] private AudioClip fx;

        private bool isOnlyOff = false;
        private bool isInteract = true;


        public override void OnClickDown()
        {
            base.OnClickDown();

            if (!isInteract) return;

            bool isActive = !buttonGO.activeSelf;

            if (isOnlyOff && isActive) return;

            buttonGO.SetActive(isActive);
            fireGO.SetActive(isActive);
            OnStoveFire?.Invoke(isActive);

            if(!isActive)
            {
                LevelControl.Ins.CheckStep(0.5f);
            }

            if(fx != null) SoundControl.Ins.PlayFX(fx);
        }

        public void SetInteract(bool isInteract)
        {
            this.isInteract = isInteract;
        }

        public void SetOnlyTurnOff()
        {
           isOnlyOff = true;
        }
    }
}