using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

namespace Link.Cooking.Spageti_Bear
{
    public class Spoon : ItemMovingBase
    {
        [SerializeField] SortingGroup sortingHiltSpoon;
        [SerializeField] Transform spoonHilt;

        void LateUpdate()
        {
            spoonHilt.SetPositionAndRotation(TF.position, TF.rotation);
            spoonHilt.localScale = TF.localScale;
        }

        public override void OnClickDown()
        {
            base.OnClickDown();
            OrderLayer = 25;
            TF.DORotate(Vector3.forward * 150, 0.2f);
            sortingHiltSpoon.sortingOrder = 40;
        }

        public override void OnDrop()
        {
            base.OnDrop();
            OnBack();
            TF.DORotate(Vector3.forward * -10f, 0.2f);
            sortingHiltSpoon.sortingOrder = 0;

            LevelControl.Ins.CheckStep(0.5f);
        }
    }
}
