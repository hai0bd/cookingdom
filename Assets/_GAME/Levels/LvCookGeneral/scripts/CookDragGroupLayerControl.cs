using System.Collections;
using UnityEngine;
using Utilities;

namespace TidyCooking.Levels
{
    public class CookDragGroupLayerControl : MonoSingleton<CookDragGroupLayerControl>
    {
        [SerializeField] private int _initSortOrder = 0;
        [SerializeField] private int _limitSortOrder = 32767;
        [SerializeField] private float _initPosZ = 5;
        [SerializeField] private float _limitPosZ = -5f;
        [SerializeField] private float _incPosZ = -0.001f;

        public int LastSortOrder { get; private set; } = 0;
        public float LastPosZ { get; private set; } = 0;

        protected override void Awake()
        {
            base.Awake();
            LastSortOrder = _initSortOrder;
            LastPosZ = _initPosZ;
        }

        public void IncreaseSortOrder() => LastSortOrder = (LastSortOrder + 1) % _limitSortOrder;
        public void IncreasePosZ() => LastPosZ = Mathf.Repeat(LastPosZ + _incPosZ, _initPosZ - _limitPosZ) + _limitPosZ;
    }
}