using System;
using System.Collections;
using UnityEngine;
using Utilities;

namespace TidyCooking.Levels
{
    public class CookItemIngredientScatter : CookItemAbstract, ICookPickableIngredient
    {
        [Space, Header("Ingredient Config")]
        [SerializeField] private ObjectScatterGroup _scatterControl;
        [SerializeField] private ObjectScatterAnim _scatterAnim;
        [SerializeField] private float _positionExpandMulInsideContainer = 0.5f;
        [SerializeField] private float _positionExpandMulOutside = 1f;
        public Transform ItemTransform => this.transform;

        public event Action<ICookPickableIngredient> onStartDrag;
        public event Action<ICookPickableIngredient> onEndDrag;

        public override void SetUp()
        {
            base.SetUp();
            _scatterAnim.SetUp();
        }

        protected override void OnStartDrag()
        {
            base.OnStartDrag();
            _scatterAnim.SetAnim(ObjectScatterAnim.AnimState.Drag);
            onStartDrag?.Invoke(this);
        }

        protected override void OnEndDrag()
        {
            base.OnEndDrag();
            _scatterAnim.SetAnim(ObjectScatterAnim.AnimState.Static);
            onEndDrag?.Invoke(this);
        }

        public void SetUpOnChangeContainer(ICookIngredientContainer container)
        {
            if (container == null)
            {
                _scatterAnim.PositionExpandMul = _positionExpandMulOutside;
                _scatterAnim.SetAnim(ObjectScatterAnim.AnimState.Static);
            }
            else
            {
                _scatterAnim.PositionExpandMul = _positionExpandMulInsideContainer;
                _scatterAnim.SetAnim(ObjectScatterAnim.AnimState.Floating);
            }
        }
    }
}