using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UBootstrap
{
    public class TextureDitherSetting : Setting<TextureDitherSetting>
    {
        [MenuItem ("Settings/TextureDitherSetting")]
        public static void Edit ()
        {
            Selection.activeObject = Instance;
        }

        public DitheringAlgorithmSetting ditheringAlgorithmSetting;
        public string DitherDirectorySuffix = "Dither";
    }
}


