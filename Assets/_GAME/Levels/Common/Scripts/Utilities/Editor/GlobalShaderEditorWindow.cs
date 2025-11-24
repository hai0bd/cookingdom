using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector;

namespace Utilities
{
    public class GlobalShaderEditorWindow : OdinEditorWindow
    {
        [System.Serializable]
        public abstract class ShaderProperty<T>
        {
            [TableColumnWidth(150)]
            public string name;
            [TableColumnWidth(100)]
            public T value;
            [TableColumnWidth(50)]
            [Button(ButtonSizes.Small), GUIColor(.3f, 1f, .3f)]
            public abstract void Apply();
        }

        [System.Serializable]
        public class ShaderPropertyFloat : ShaderProperty<float>
        {
            public override void Apply()
            {
                Shader.SetGlobalFloat(name, value);
            }
        }

        [System.Serializable]
        public class ShaderPropertyInt : ShaderProperty<int>
        {
            public override void Apply()
            {
                Shader.SetGlobalInt(name, value);
            }
        }

        [System.Serializable]
        public class ShaderPropertyColor : ShaderProperty<Color>
        {
            public override void Apply()
            {
                Shader.SetGlobalColor(name, value);
            }
        }

        [System.Serializable]
        public class ShaderPropertyVector : ShaderProperty<Vector4>
        {
            public override void Apply()
            {
                Shader.SetGlobalVector(name, value);
            }
        }

        [MenuItem("Window/GlobalShaderEditorWindow")]
        public static void OpenWindow()
        {
            EditorWindow.GetWindow<GlobalShaderEditorWindow>().Show();
        }

        [TableList]
        public ShaderPropertyFloat[] propFloats;
        [TableList]
        public ShaderPropertyInt[] propInts;
        [TableList]
        public ShaderPropertyColor[] propColors;
        [TableList]
        public ShaderPropertyVector[] propVectors;

        [Button(ButtonSizes.Large), GUIColor(1, 0.5f, 0)]
        public void ApplyAll()
        {
            foreach (var prop in propFloats)
            {
                prop.Apply();
            }
            foreach (var prop in propInts)
            {
                prop.Apply();
            }
            foreach (var prop in propColors)
            {
                prop.Apply();
            }
            foreach (var prop in propVectors)
            {
                prop.Apply();
            }
        }
    }
}