using UnityEditor;
using System;
using UnityEditorInternal;
using System.Reflection;
using System.Text;
using UBootstrap;
using System.IO;
using System.Collections.Generic;

namespace UBootstrap.Editor
{
    public class ConstantGenerator
    {
        [MenuItem ("UBootstrap/Constant/Generate All")]
        public static void GenerateAll ()
        {
            GenerateTags ();
            GenerateLayers ();
            GenerateSortingLayers ();
            GenerateScenes ();
        }

        [MenuItem ("UBootstrap/Constant/Generate Tags")]
        public static void GenerateTags ()
        {
            Generate (ConstantGeneratorSetting.Instance.Namespace, ConstantGeneratorSetting.Instance.TagsClassName, ConstantGeneratorSetting.Instance.Location, UnityEditorInternal.InternalEditorUtility.tags);
        }

        [MenuItem ("UBootstrap/Constant/Generate Layers")]
        public static void GenerateLayers ()
        {
            Generate (ConstantGeneratorSetting.Instance.Namespace, ConstantGeneratorSetting.Instance.LayersClassName, ConstantGeneratorSetting.Instance.Location, UnityEditorInternal.InternalEditorUtility.layers);
        }

        [MenuItem ("UBootstrap/Constant/Generate Sorting Layers")]
        public static void GenerateSortingLayers ()
        {
            Generate (ConstantGeneratorSetting.Instance.Namespace, ConstantGeneratorSetting.Instance.SortingLayersClassName, ConstantGeneratorSetting.Instance.Location, GetSortingLayerNames ());
        }

        [MenuItem ("UBootstrap/Constant/Generate Scenes")]
        public static void GenerateScenes ()
        {
            Generate (ConstantGeneratorSetting.Instance.Namespace, ConstantGeneratorSetting.Instance.ScenesClassName, ConstantGeneratorSetting.Instance.Location, GetSceneNames ());
        }

        private static void Generate (string namespaceName, string className, string location, string[] constants)
        {
            StringBuilder sb = new StringBuilder ();
            sb.AppendFormat ("namespace {0}\n", namespaceName);
            sb.Append ("{\n");
            sb.AppendFormat ("    public partial class {0}\n", className);
            sb.Append ("    {\n");
            foreach (var c in constants) {
                sb.AppendFormat ("        public const string {0} = \"{1}\";\n", c.Replace (" ", string.Empty), c);
            }
            sb.Append ("    }\n");
            sb.Append ("}\n");

            FileSystemHelper.SaveToFileInAssets (sb.ToString (), Path.Combine (location, className + ".cs"));

        }

        private static string[] GetSortingLayerNames ()
        {
            Type internalEditorUtilityType = typeof(InternalEditorUtility);
            PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty ("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
            var sortingLayers = (string[])sortingLayersProperty.GetValue (null, new object[0]);
            return sortingLayers;

        }

        private static string[] GetSceneNames ()
        {
            List<string> scenes = new List<string> ();
            foreach (var scene in EditorBuildSettings.scenes) {
                if (!scene.enabled) {
                    continue;
                }
                scenes.Add (Path.GetFileNameWithoutExtension (scene.path));
            }
            return scenes.ToArray ();
        }

    }

}