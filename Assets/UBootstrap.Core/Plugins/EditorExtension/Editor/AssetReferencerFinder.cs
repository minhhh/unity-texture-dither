using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

namespace UBootstrap.Editor
{
    public class AssetReferencerFinder : EditorWindow
    {
        private Vector2 scrollPos;
        Dictionary<ReferencerAssetParam, List<UnityEngine.Object>> referencesByTypes = new Dictionary<ReferencerAssetParam, List<UnityEngine.Object>> ();

        [MenuItem ("UBootstrap/Rererence Finder")]
        static void Init ()
        {
            AssetReferencerFinder window = EditorWindow.GetWindow <AssetReferencerFinder> ("Reference Finder");
            window.Show ();
        }

        AssetReferencerFinder ()
        {
        }

        void OnGUI ()
        {
            if (Selection.activeObject == null) {
                GUI.color = Color.red;
                GUILayout.Label ("Please select an object", EditorStyles.wordWrappedLabel);
                return;
            }

            GUI.color = Color.white;

            if (GUILayout.Button ("Find")) {
                List<string> objectsPath = ListUpAllObjectPath ();
                List<UnityEngine.Object> referencers = FindReferencersForObject (Selection.activeObject, objectsPath);
                referencesByTypes = AggregateReferencerByAssetType (referencers);
            }

            scrollPos = EditorGUILayout.BeginScrollView (scrollPos);
            EditorGUILayout.BeginVertical ();

            EditorGUILayout.LabelField (string.Format ("Object [{0}]", AssetDatabase.GetAssetPath (Selection.activeObject)));
            EditorGUILayout.Space ();

            GUI.color = Color.red;
            EditorGUILayout.LabelField (string.Format ("Total Referencers: {0}", referencesByTypes.Sum ((x) => x.Value.Count)));

            var keys = referencesByTypes.Keys.ToList ();
            for (int i = 0; i < keys.Count; i++) {
                var e = referencesByTypes [keys [i]];
                var count = e.Count;
                GUI.color = count > 0 ? Color.green : Color.gray;
                EditorGUILayout.LabelField (string.Format ("{0}:{1}", keys [i].assetType, count));
                for (int j = 0; j < e.Count; j++) {
                    EditorGUILayout.ObjectField (e [j], typeof(Object), true);
                }
            }

            EditorGUILayout.EndVertical ();
            EditorGUILayout.EndScrollView ();
        }

        class ReferencerAssetParam
        {
            public string assetDirectory { get; private set; }

            public string assetType { get; private set; }

            public string assetExtension { get; private set; }

            public ReferencerAssetParam (string assetDirectory, string assetType, string assetExtension)
            {
                this.assetDirectory = assetDirectory;
                this.assetType = assetType;
                this.assetExtension = assetExtension;
            }
        }

        static readonly ReferencerAssetParam[] referencerAssetParams = new ReferencerAssetParam[] {
            new ReferencerAssetParam ("Assets", "Scene", ".unity"),
            new ReferencerAssetParam ("Assets", "Prefab", ".prefab"),
            new ReferencerAssetParam ("Assets", "Material", ".mat"),
            new ReferencerAssetParam ("Assets", "Shader", ".shader"),
            new ReferencerAssetParam ("Assets", "ScriptableObject", ".asset"),
            new ReferencerAssetParam ("Assets", "Flare", ".flare"),
            new ReferencerAssetParam ("Assets", "AnimatorController", ".controller"),
            new ReferencerAssetParam ("Assets", "AnimatorOverrideController", ".overrideController"),
            new ReferencerAssetParam ("Assets", "Cubemap", ".cubemap"),
            new ReferencerAssetParam ("Assets", "ComputeShader", ".compute"),
            new ReferencerAssetParam ("Assets", "AvatorMask", ".mask"),
            new ReferencerAssetParam ("Assets", "GUISkin", ".guiskin"),
        };


        private static List<string> ListUpAllObjectPath ()
        {
            List<string> path = new List<string> ();

            foreach (var param in referencerAssetParams) {
                path.AddRange (ListUpAllObjectPathByType (param.assetDirectory, param.assetType, param.assetExtension));
            }

            return path;
        }


        private static List<UnityEngine.Object> GetReferencerByAssetType (string assetType, string assetExtension, List<UnityEngine.Object> referencerObjects)
        {
            var result = new List <UnityEngine.Object> ();

            var assets = referencerObjects.FindAll ((x) => {
                return AssetDatabase.GetAssetPath (x).EndsWith (assetExtension);
            });

            foreach (var o in assets) {
                result.Add (o);
            }


            return result;
        }

        private static Dictionary<ReferencerAssetParam, List<UnityEngine.Object>> AggregateReferencerByAssetType (List<UnityEngine.Object> referencerObjects)
        {
            var result = new Dictionary<ReferencerAssetParam, List<UnityEngine.Object>> ();
            foreach (ReferencerAssetParam param in referencerAssetParams) {
                result [param] = GetReferencerByAssetType (param.assetType, param.assetExtension, referencerObjects);
            }

            return result;
        }

        private static List<UnityEngine.Object> FindReferencersForObject (UnityEngine.Object referencee, List<string> objectPath)
        {
            List<UnityEngine.Object> results = new List<UnityEngine.Object> ();

            int index = 0;
            foreach (string path in objectPath) {
                if (index % 10 == 0) {
                    string progressBarSubTile = string.Format ("[{0}/{1}] : {2}", index + 1, objectPath.Count, path);
                    if (UnityEditor.EditorUtility.DisplayCancelableProgressBar (
                            "find referencer object", progressBarSubTile, (float)(index + 1) / objectPath.Count)) {
                        break;
                    }
                }

                UnityEngine.Object referencer = AssetDatabase.LoadAssetAtPath (path, typeof(UnityEngine.Object));

                if (IsDependentOn (referencer, referencee)) {
                    results.Add (referencer);
                }

                index++;
            }

            Resources.UnloadUnusedAssets ();

            EditorUtility.ClearProgressBar ();

            return results;
        }

        private static bool IsDependentOn (UnityEngine.Object referencer, UnityEngine.Object referencee)
        {
            if (referencee == referencer) {
                return false;
            }

            string referencerPath = AssetDatabase.GetAssetPath (referencer);
            string[] assetPaths = new string[1] {
                referencerPath,
            };

            string referenceePath = AssetDatabase.GetAssetPath (referencee);
            string[] dependencies = AssetDatabase.GetDependencies (assetPaths);

            bool result = false;
            foreach (string depend in dependencies) {
                if (depend == referenceePath) {
                    result = true;
                    break;
                }
            }

            return result;
        }

        private static List<string> ListUpAllObjectPathByType (string directory, string assetType, string assetFileExtension)
        {
            string assetSearchCond = "t:" + assetType + " ";
            string[] assetPaths = new string[1] {
                directory,
            };

            string[] guids = AssetDatabase.FindAssets (assetSearchCond, assetPaths);

            List<string> scenePath = new List<string> ();

            foreach (string guid in guids) {
                string assetPath = AssetDatabase.GUIDToAssetPath (guid);
                if (!assetPath.EndsWith (assetFileExtension)) {
                    continue;
                }
                scenePath.Add (assetPath);
            }

            return scenePath;
        }

    }
}
