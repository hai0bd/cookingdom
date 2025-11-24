using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Utilities
{
    [ExecuteAlways]
    public class CutoutMaskUI : Image
    {
        private static readonly int _stencilComp = Shader.PropertyToID("_StencilComp");
        public override Material GetModifiedMaterial(Material baseMaterial)
        {
            var resultMaterial = new Material(base.GetModifiedMaterial(baseMaterial));
            resultMaterial.SetFloat(_stencilComp, Convert.ToSingle(CompareFunction.NotEqual));
            return resultMaterial;
        }
    }
}