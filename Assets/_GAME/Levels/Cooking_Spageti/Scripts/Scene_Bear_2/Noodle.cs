using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Link.Cooking.Spageti_Bear
{
    public class Noodle : ItemIdleBase
    {
        [SerializeField] SortingGroup orderlayer;
        [SerializeField] ItemAlpha noodleSprite;

        public override int OrderLayer { get => base.OrderLayer; set => orderlayer.sortingOrder = value; }

        public void SetAlpha(float alpha)
        {
            noodleSprite.SetAlpha(alpha);
        }
    }
}