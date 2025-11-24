using Link;
using UnityEngine;

namespace HuyThanh.Cooking.Burrito
{
    public enum DecorItemType
    {
        Vegetable,
        Tomato,
        LeafLeft,
        LeafRight,
        Lemon,
        BurritoLeft,
        BurritoRight,
    }
    public class DecorItem : ItemMovingBase
    {
        [SerializeField] DecorItemType type;
        [SerializeField] PlateDecor plateDecor;

        public override bool IsCanMove => true;

        public bool IsType(DecorItemType t)
        {
            return type == t;
        }

        public void RemoveFromTarget()
        {
            plateDecor.Remove(type);
        }

        public override void OnClickDown()
        {
            base.OnClickDown();
            SoundControl.Ins.PlayFX(Fx.Click);
        }
        public override void OnDrop()
        {
            base.OnDrop();
            SoundControl.Ins.PlayFX(Fx.PutDown);
        }
    }

}