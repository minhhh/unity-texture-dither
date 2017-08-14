using UnityEngine;

namespace UBootstrap
{
    [CreateAssetMenu (menuName = "Settings/DitheringAlgorithmSetting")]
    public class DitheringAlgorithmSetting : ScriptableObject
    {
        public enum ColorSpace
        {
            RGBA4444 = 1,
            WebSafe = 2,
            TrueColor = 3
        }

        public ColorSpace targetColorSpace;
        public string type;
    }
}