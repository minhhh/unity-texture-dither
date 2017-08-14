using UnityEngine;
using System.Collections;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UBootstrap
{
    public class ConstantGeneratorSetting : Setting<ConstantGeneratorSetting>
    {
        #if UNITY_EDITOR
        [MenuItem ("Settings/ConstantGeneratorSetting")]
        public static void Edit ()
        {
            Selection.activeObject = Instance;
        }
        #endif

        public string Location = ""; // folder in Assets folder where the classes will be stored
        public string Namespace;
        public string TagsClassName;
        public string LayersClassName;
        public string SortingLayersClassName;
        public string ScenesClassName;
    }
}