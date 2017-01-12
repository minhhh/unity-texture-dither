using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using UBootstrap;

namespace UBootstrap.TextureDithering
{
    class DitherTextureImporter : AssetPostprocessor
    {
        private const string DitherDirectorySuffix = "Dither";

        static bool IsDitherAsset (string assetPath)
        {
            string directory = Path.GetDirectoryName (assetPath);
            return directory.EndsWith (DitherDirectorySuffix);
        }

        void OnPreprocessTexture ()
        {
            var importer = (assetImporter as TextureImporter);

            if (IsDitherAsset (assetPath)) {
                importer.textureFormat = TextureImporterFormat.RGBA32;
            }
        }

        void OnPostprocessTexture (Texture2D texture)
        {
            if (!IsDitherAsset (assetPath)) {
                return;
            }
            var importer = (assetImporter as TextureImporter);

            Type t = ReflectionHelper.GetType (String.Format ("{0}.{1}", this.GetType().Namespace, TextureDitherSetting.Instance.ditheringAlgorithmSetting.type));

            if (t == null) {
                Debug.LogError (this.GetType ().Name + "::OnPostprocessTexture No dithering found " + TextureDitherSetting.Instance.ditheringAlgorithmSetting.type);
                return;
            }
            DitheringBase method = (DitheringBase)Activator.CreateInstance (t, GetFindColorFuncFromString (TextureDitherSetting.Instance.ditheringAlgorithmSetting.targetColorSpace));
            texture.SetPixels (method.DoDithering (texture.GetPixels (), texture.width, texture.height));

            TextureFormat textureFormat = TextureFormat.RGBA4444;
            TextureImporterFormat textureImporterFormat = TextureImporterFormat.Automatic16bit;

            if (TextureDitherSetting.Instance.ditheringAlgorithmSetting.targetColorSpace == DitheringAlgorithmSetting.ColorSpace.TrueColor) {
                textureFormat = TextureFormat.RGBA32;
                textureImporterFormat = TextureImporterFormat.AutomaticTruecolor;
            }
            EditorUtility.CompressTexture (texture, textureFormat, TextureCompressionQuality.Best);
            importer.textureFormat = textureImporterFormat;
        }
            
        private static FindColor GetFindColorFuncFromString (string colorSpace)
        {
            switch (colorSpace) {
            case DitheringAlgorithmSetting.ColorSpace.RGBA4444:
                return TrueColorToRGBA4444;
            case DitheringAlgorithmSetting.ColorSpace.WebSafe:
                return TrueColorToWebSafeColor;
            case DitheringAlgorithmSetting.ColorSpace.TrueColor:
            default:
                return TrueColorToTrueColor;
            }
        }

        private static Color TrueColorToWebSafeColor (Color inputColor)
        {
            Color returnColor = new Color (Mathf.Clamp01 ((Mathf.FloorToInt ((inputColor.r * 255) / 51.0f) * 51) / 255.0f),
                                    Mathf.Clamp01 ((Mathf.FloorToInt ((inputColor.g * 255) / 51.0f) * 51) / 255.0f),
                                    Mathf.Clamp01 ((Mathf.FloorToInt ((inputColor.b * 255) / 51.0f) * 51) / 255.0f),
                                    inputColor.a);
            return returnColor;
        }

        private static Color TrueColorToRGBA4444 (Color inputColor)
        {
            Color returnColor = new Color (Mathf.Clamp01 ((Mathf.FloorToInt ((inputColor.r * 255) / 16.0f) * 16) / 255.0f),
                                    Mathf.Clamp01 ((Mathf.FloorToInt ((inputColor.g * 255) / 16.0f) * 16) / 255.0f),
                                    Mathf.Clamp01 ((Mathf.FloorToInt ((inputColor.b * 255) / 16.0f) * 16) / 255.0f),
                                    Mathf.Clamp01 ((Mathf.FloorToInt ((inputColor.a * 255) / 16.0f) * 16) / 255.0f));

            return returnColor;
        }

        private static Color TrueColorToTrueColor (Color inputColor)
        {
            return inputColor;
        }

    }
}

