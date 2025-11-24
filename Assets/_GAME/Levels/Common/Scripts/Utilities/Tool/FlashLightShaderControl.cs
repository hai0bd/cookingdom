using System.Collections;
using UnityEngine;

namespace Utilities
{
    public class FlashLightShaderControl : MonoBehaviour
    {
        public Transform trackingLightSource;
        public Vector2 lightScale = Vector2.one;

        private static readonly int ShaderGlobalPropertyID_FlashLightPos = Shader.PropertyToID("_FlashLightSourcePos");
        private static readonly int ShaderGlobalPropertyID_FlashLightSourceScaleXYRotateCacheSinCosZW = Shader.PropertyToID("_FlashLightSourceScaleXYRotateCacheSinCosZW");
        private Vector4 lightSourcePos;
        private Vector4 lightSourceScaleXYRotateCacheSinCosZW;

        private void Awake()
        {
            lightSourcePos = new Vector4(trackingLightSource.position.x, trackingLightSource.position.y, 0, 0);
            lightSourceScaleXYRotateCacheSinCosZW = new Vector4(0, 0, 0, 0);
        }

        private void Update()
        {
            UpdateShaderProp();
        }

        [Sirenix.OdinInspector.Button("Update Shader Prop")]
        private void UpdateShaderProp()
        {
            lightSourcePos.x = trackingLightSource.position.x;
            lightSourcePos.y = trackingLightSource.position.y;
            lightSourceScaleXYRotateCacheSinCosZW.x = lightScale.x;
            lightSourceScaleXYRotateCacheSinCosZW.y = lightScale.y;
            lightSourceScaleXYRotateCacheSinCosZW.z = Mathf.Sin(-trackingLightSource.rotation.eulerAngles.z * Mathf.Deg2Rad);
            lightSourceScaleXYRotateCacheSinCosZW.w = Mathf.Cos(-trackingLightSource.rotation.eulerAngles.z * Mathf.Deg2Rad);
            Shader.SetGlobalVector(ShaderGlobalPropertyID_FlashLightPos, lightSourcePos);
            Shader.SetGlobalVector(ShaderGlobalPropertyID_FlashLightSourceScaleXYRotateCacheSinCosZW, lightSourceScaleXYRotateCacheSinCosZW);
        }
    }
}