using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Link.Turkey
{
    public class OvenButton : ItemIdleBase
    {
        [SerializeField] Collider2D collider;
        [SerializeField] Oven oven;
        [SerializeField] GameObject active;

        public override void OnClickDown()
        {
            base.OnClickDown();
            if (active != null)
            {
                active.SetActive(true);
            }
            else
            {
                transform.DORotate(Vector3.forward * -90, 0.4f);
            }
            oven.ClickButton();
            collider.enabled = false;
        }
    }
}