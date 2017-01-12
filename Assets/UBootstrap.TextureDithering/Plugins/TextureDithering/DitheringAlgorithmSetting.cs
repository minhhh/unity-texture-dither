using UnityEngine;

namespace UBootstrap
{
    [CreateAssetMenu (menuName = "Settings/DitheringAlgorithmSetting")]
    public class DitheringAlgorithmSetting : ScriptableObject
    {
        public static class ColorSpace
        {
            public const string RGBA4444 = "RGBA4444";
            public const string WebSafe = "WebSafe";
            public const string TrueColor = "TrueColor";
        }

        public string targetColorSpace;
        public string type;
    }
}