using System.Collections;
using DG.Tweening;
using UnityEngine;
using Utilities;

namespace TidyCooking.Levels
{
    public interface ICookPickableIngredient
    {
        public Transform ItemTransform { get; }
        public BoolModifierWithRegisteredSource BlockDragModifiers { get; }
        public event System.Action<ICookPickableIngredient> onStartDrag;
        public event System.Action<ICookPickableIngredient> onEndDrag;
        public void SetUpOnChangeContainer(ICookIngredientContainer container);
        public void SetTransformation(IEnumerator ieMove);
        public void SetTransformation(Tween tweenMove);
    }
}