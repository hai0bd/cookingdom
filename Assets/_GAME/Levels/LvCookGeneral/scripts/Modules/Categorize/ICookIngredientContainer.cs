using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace TidyCooking.Levels
{
    public interface ICookIngredientContainer
    {
        public Transform ContainerTransform { get; }
        public bool IsCanAddIngredientAtPosition(ICookPickableIngredient ingredient);
        public bool AddIngredient(ICookPickableIngredient ingredient);
        public void RemoveIngredient(ICookPickableIngredient ingredient);
        public List<ICookPickableIngredient> IngredientsInside { get; }

        public BoolModifierWithRegisteredSource BlockDragModifiers { get; }
        public BoolModifierWithRegisteredSource BlockThrowInOutModifiers { get; }
    }
}